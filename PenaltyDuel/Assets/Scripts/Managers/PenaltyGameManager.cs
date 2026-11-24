using Unity.Netcode;
using UnityEngine;

public class PenaltyGameManager : NetworkBehaviour
{
    public static PenaltyGameManager Instance { get; private set; }

    [Header("Refs")]
    public GameObject ballPrefab;
    public Vector3 ballSpawnPosition = new Vector3(7.5f, 0.3f, -3.5f);
    public RectTransform[] targets = new RectTransform[9];
    public GoalkeeperAI goalkeeperAI;
    public Transform goalkeeperHome;

    [Header("Shot")]
    public float shotForce = 25f;
    public float maxOffset = 3f;
    public float spinFactor = 5f;
    public float roundDelay = 2f;

    [Header("Net State")]
    public NetworkVariable<int> roundNumber = new(1,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> goals = new(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> saves = new(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> lastResult = new(-1,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> revealedAttackerZone = new(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> revealedGoalkeeperZone = new(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> perfectZone = new(0,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> score = new(0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);

    public NetworkVariable<int> shotsTaken = new(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private const int MAX_SHOTS = 5;

    private Transform _ball;
    private int _attackerZone = 0;
    private float _attackerTiming = 0f;
    private float _attackerAccuracy = 0.5f;
    private float _attackerComposure = 0.5f;
    private bool _roundResolved = false;
    private bool _ballInGoal = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        if (ballPrefab != null)
        {
            GameObject ballInstance = Instantiate(ballPrefab, ballSpawnPosition, Quaternion.identity);
            ballInstance.GetComponent<NetworkObject>().Spawn();
            _ball = ballInstance.transform;
            Debug.Log($"Ball spawned at {ballSpawnPosition}");
        }
        else
        {
            Debug.LogError("ballPrefab is not assigned in PenaltyGameManager!");
        }

        perfectZone.Value = Random.Range(1, 10);

        foreach (var target in FindObjectsByType<TargetElement>())
        {
            target.SetPerfect(target.zoneNumber == perfectZone.Value);
        }

        ResetBallAndGoalkeeper();
    }

    [Rpc(SendTo.Server)]
    public void SubmitShotRpc(int zone, float timing, float accuracy, float composure, RpcParams rpcParams = default)
    {
        if (!IsServer) return;
        if (_roundResolved) return;
        if (zone < 1 || zone > 9) return;

        _attackerZone = zone;
        _attackerTiming = timing;
        _attackerAccuracy = accuracy;
        _attackerComposure = composure;
        _roundResolved = true;
        _ballInGoal = false;

        ResolveRound();
    }

    private void ResolveRound()
    {
        if (goalkeeperAI == null)
        {
            Debug.LogError("goalkeeperAI is not assigned!");
            return;
        }

        int gkZone = goalkeeperAI.ChooseSaveZone(perfectZone.Value);

        revealedAttackerZone.Value = _attackerZone;
        revealedGoalkeeperZone.Value = gkZone;

        float accuracyFactor = Mathf.Lerp(1.5f, 0.3f, _attackerAccuracy);
        float composureFactor = Mathf.Lerp(1.5f, 0.3f, _attackerComposure);

        float error = Mathf.Abs(_attackerTiming - 0.5f) * 2f;
        error = error * error * composureFactor;

        float perfectBonus = (_attackerZone == perfectZone.Value) ? 0.3f : 1.0f;

        Vector2 randomOffset = Random.insideUnitCircle * error * maxOffset * accuracyFactor * perfectBonus;

        if (targets[_attackerZone - 1] == null)
        {
            Debug.LogError($"targets[{_attackerZone - 1}] is null!");
            return;
        }

        Vector3 aimPos = targets[_attackerZone - 1].position;

        if (_ball != null)
        {
            Rigidbody rb = _ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                Vector3 toTarget = aimPos - _ball.position;
                Vector3 horizontal = new Vector3(toTarget.x, 0f, toTarget.z);
                float distance = horizontal.magnitude;
                float heightDiff = toTarget.y;
                float gravity = Mathf.Abs(Physics.gravity.y);
                float speed = shotForce;

                float v2 = speed * speed;
                float v4 = v2 * v2;
                float discriminant = v4 - gravity * (gravity * distance * distance + 2f * heightDiff * v2);

                if (discriminant < 0f)
                {
                    Debug.LogWarning($"Target unreachable at speed={speed}. Shooting straight.");
                    rb.linearVelocity = toTarget.normalized * speed;
                }
                else
                {
                    float angle = Mathf.Atan((v2 - Mathf.Sqrt(discriminant)) / (gravity * distance));

                    Vector3 horizontalDir = horizontal.normalized;
                    Vector3 velocity = horizontalDir * Mathf.Cos(angle) * speed;
                    velocity.y = Mathf.Sin(angle) * speed;

                    velocity.z += randomOffset.x * 5f;
                    velocity.y += randomOffset.y * 5f;

                    rb.linearVelocity = velocity;

                    Vector3 spin = new Vector3(0f, randomOffset.x * spinFactor, randomOffset.y * spinFactor);
                    rb.angularVelocity = spin;

                    Debug.Log($"Ball shot: dist={distance:F2}, heightDiff={heightDiff:F2}, angle={angle * Mathf.Rad2Deg:F1}°, offset=({randomOffset.x:F2}, {randomOffset.y:F2}), spin=({spin.y:F1}, {spin.z:F1}), acc={_attackerAccuracy:F2}, comp={_attackerComposure:F2}");
                }
            }
        }
        else
        {
            Debug.LogError("_ball is null!");
        }

        Invoke(nameof(FinishRound), roundDelay);
    }

    private void FinishRound()
    {
        if (!IsServer) return;

        bool gkGuessed = (revealedAttackerZone.Value == revealedGoalkeeperZone.Value);
        bool isGoal = _ballInGoal && !gkGuessed;

        Debug.Log($"FinishRound: ballInGoal={_ballInGoal}, gkGuessed={gkGuessed}");

        if (isGoal)
        {
            int points = revealedAttackerZone.Value == perfectZone.Value ? 300 : 100;

            score.Value += points;
            lastResult.Value = 0;

            Debug.Log($"GOAL! +{points} points. Total score: {score.Value}");
        }
        else
        {
            lastResult.Value = 1;

            Debug.Log("SAVED! +0 points.");
        }

        shotsTaken.Value++;

        goalkeeperAI.RegisterHit(revealedAttackerZone.Value);

        Debug.Log($"Shot {shotsTaken.Value}/{MAX_SHOTS}. Score: {score.Value}");

        if (shotsTaken.Value >= MAX_SHOTS)
        {
            Debug.Log($"SESSION FINISHED! Final score: {score.Value}");
            return;
        }

        roundNumber.Value = shotsTaken.Value + 1;

        Invoke(nameof(ResetRound), 1f);
    }

    private void ResetRound()
    {
        if (!IsServer) return;

        ResetBallAndGoalkeeper();
        lastResult.Value = -1;
        revealedAttackerZone.Value = 0;
        revealedGoalkeeperZone.Value = 0;
        _attackerZone = 0;
        _attackerTiming = 0f;
        _roundResolved = false;
        _ballInGoal = false;
    }

    private void ResetBallAndGoalkeeper()
    {
        if (_ball != null)
        {
            Rigidbody rb = _ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            _ball.position = ballSpawnPosition;
        }

        if (goalkeeperAI != null)
            goalkeeperAI.ResetPosition();
    }

    public void OnBallEnteredGoal()
    {
        _ballInGoal = true;
        Debug.Log("BALL ENTERED GOAL!");
    }
}