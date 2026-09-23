using UnityEngine;

public class AsteroidSimplePool : MonoBehaviour
{
    public static AsteroidSimplePool Instance { get; private set; }

    [SerializeField] private AsteroidSimple prefab;

    [SerializeField] private int initialSize = 75;

    private ObjectPool<AsteroidSimple> pool;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        if (prefab == null)
            return;

        pool = new ObjectPool<AsteroidSimple>(prefab, initialSize, transform);
    }

    public AsteroidSimple Get(Vector3 position, Quaternion rotation) {
        if (pool == null) return null;

        AsteroidSimple asteroid = pool.Get();
        asteroid.transform.SetPositionAndRotation(position, rotation);

        return asteroid;
    }

    public void Return(AsteroidSimple asteroid) {
        if (pool == null || asteroid == null) return;
        pool.Return(asteroid);
    }

    public int CountPool => pool?.CountPool ?? 0;
}
