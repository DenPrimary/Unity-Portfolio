using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] private GaussBullet bulletPrefab;
    [SerializeField] private int initialSize = 100;

    private ObjectPool<GaussBullet> pool;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        if (bulletPrefab == null) {
            Debug.LogError("BulletPrefab is not assigned!");
            return;
        }

        pool = new ObjectPool<GaussBullet>(bulletPrefab, initialSize, transform);
    }

    public GaussBullet GetBullet(Vector3 position, Quaternion rotation) {
        if (pool == null) return null;

        GaussBullet bullet = pool.Get();
        bullet.transform.SetPositionAndRotation(position, rotation);
        return bullet;
    }

    public void ReturnBullet(GaussBullet bullet) {
        if (pool == null || bullet == null) return;
        pool.Return(bullet);
    }

    public int CountPool => pool?.CountPool ?? 0;
}
