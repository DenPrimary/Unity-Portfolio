using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Prefabs")]
    public GameObject asteroidSimple;
    public GameObject asteroidSplinter;
    public GameObject asteroidAdamantine;

    [Header("Circles")]
    public float targetCircle = 8f;
    public float spawnCircle = 32.5f;
    public Color targetCirсleColor = Color.green;
    public int circleSegments = 32;

    [Header("Spawn Weights")]
    public int simpleWeight = 100;
    public int splinterWeight = 0;
    public int adamantineWeight = 0;

    [Header("Spawn Settings")]
    public float spawnInterval = 3.5f;
    public float minSpeed = 1f;
    public float maxSpeed = 10f;
    public float minSize = 0.5f;
    public float maxSize = 5f;
    public float baseHealth = 1;

    private Transform player;
    private Vector3 targetPoint;
    private Vector3 asteroidSpawnPoint;

    private float minX, maxX, minZ, maxZ;
    private bool hasBackground = false;
    public int totalWeight = 100;

    void Start()
    {
        totalWeight = simpleWeight + splinterWeight + adamantineWeight;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else 
            return;

        GameObject bg = GameObject.Find("background");
        if (bg != null) 
        { 
            Renderer rend = bg.GetComponent<Renderer>();
            if (rend != null) 
            { 
                Bounds bounds = rend.bounds;
                minX = bounds.min.x;
                maxX = bounds.max.x;
                minZ = bounds.min.z;
                maxZ = bounds.max.z;
                hasBackground = true;
            }
        }

        InvokeRepeating(nameof(SpawnAsteroid), 0f, spawnInterval);
    }

    bool IsInsideBackground(Vector3 pos)
    {
        if (!hasBackground) return true;
        return pos.x >= minX && pos.x <= maxX && pos.z >= minZ && pos.z <= maxZ;
    }

    Vector3 FindRandomSpawnPoint(Vector3 center, float radius) 
    {
        if (!hasBackground) 
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            return center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
                );
        }
        
        for (int attempt = 0; attempt < 200; attempt++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 candidate = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
                );
            if (IsInsideBackground(candidate))
                return candidate;
        }

        return center;
    }

    GameObject GetRandomAsteroidPrefab()
    {
        if (totalWeight <= 0) return asteroidSimple;

        int roll = Random.Range(0, totalWeight);

        if (roll < simpleWeight)
            return asteroidSimple;
        else if (roll < simpleWeight + splinterWeight)
            return asteroidSplinter;
        else
            return asteroidAdamantine;
    }

    private Asteroid GetAsteroidInstance(GameObject prefab, Vector3 position, Quaternion rotation) {
        if (prefab == asteroidSimple && AsteroidSimplePool.Instance != null) {
            AsteroidSimple simple = AsteroidSimplePool.Instance.Get(position, rotation);
            if (simple != null)
                return simple;
        }

        GameObject go = Instantiate(prefab, position, rotation);
        return go.GetComponent<Asteroid>();
    }

    public void SpawnAsteroid()
    {
        if (player == null) return;

        GameObject prefab = GetRandomAsteroidPrefab();
        if (prefab == null) return;

        Vector3 playerPos = player.position;

        targetPoint = FindRandomSpawnPoint(playerPos, targetCircle);
        asteroidSpawnPoint = FindRandomSpawnPoint(playerPos, spawnCircle);

        //GameObject asteroid = Instantiate(prefab, asteroidSpawnPoint, Quaternion.identity);

        Vector3 direction = (targetPoint - asteroidSpawnPoint).normalized;
        direction += new Vector3(Random.Range(-0.15f, 0.15f), 0, Random.Range(-0.15f, 0.15f));
        direction.Normalize();

        Quaternion rotation = Quaternion.LookRotation(direction);

        Asteroid ast = GetAsteroidInstance(prefab, asteroidSpawnPoint, rotation);
        if (ast == null) return;

        float size = Mathf.Round(Random.Range(minSize, maxSize));
        size = Mathf.Max(1f, size);


        ast.baseHealth = Mathf.RoundToInt(baseHealth);
        ast.baseSpeed = Mathf.Round(Random.Range(minSpeed, maxSpeed) * 10f) / 10f;
        ast.bigCircleRadius = spawnCircle;
        ast.timeOutsideBigRadius = 10f;
        ast.SetSize(size);
        ast.Activate();
    }

    void OnDrawGizmos() 
    {
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    player = playerObj.transform;
            }

            if (player == null) return;

            Vector3 center = player.position;

            Gizmos.color = targetCirсleColor;
            DrawCircle(center, targetCircle);

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            DrawCircle(center, spawnCircle);

            Gizmos.DrawSphere(center, 0.3f);
    }

    void DrawCircle(Vector3 center, float radius) 
    { 
        Vector3 prevPoint = center + new Vector3(
                Mathf.Cos(0) * radius,
                0,
                Mathf.Sin(0) * radius
            );

        for (int i = 1; i <= circleSegments; i++) 
        { 
            float angle = (float)i / circleSegments * 2f * Mathf.PI;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
                );

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
