using UnityEngine;

public class FloatingFloatsSpawner : MonoBehaviour
{
    public static FloatingFloatsSpawner Instance { get; private set; }

    public GameObject floatingFloatPrefab;

    public float spawnHeight = 2f;

    private Transform playerTransform;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void OnPlayerSpawned(Transform player) {
        playerTransform = player;
        Debug.Log("Player found!");
    }

    private void OnDestroy() { 
        PlayerSpawner.OnPlayerSpawned -= OnPlayerSpawned;
    }

    public void Spawn(string text, Vector3 worldPosition) 
    {
        if (floatingFloatPrefab == null)
                return;

        if (playerTransform == null) return;

        Vector3 spawnPos = worldPosition + Vector3.up * spawnHeight;

        GameObject go = Instantiate(floatingFloatPrefab);
        FloatingFloats ff = go.GetComponent<FloatingFloats>();

        if (ff != null)
            ff.Setup(text, spawnPos, playerTransform);
    }
}
