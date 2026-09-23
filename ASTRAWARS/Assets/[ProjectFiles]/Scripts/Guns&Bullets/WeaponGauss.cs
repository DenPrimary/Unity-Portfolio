using UnityEngine;

public class WeaponGauss : MonoBehaviour
{
    public float fireRate = 2f;
    public float bulletSpeed = 35f;
    public float damage = 1f;

    public Transform firePoint;

    private float nextFireTime = 0f;

    private void Start() {
        if (firePoint == null)
            firePoint = transform.Find("FirePoint");
    }

    public void Shoot()
    {
        if (firePoint == null) return;
        if (Time.time < nextFireTime) return;
        if (BulletPool.Instance == null) return;

        nextFireTime = Time.time + fireRate;

        GaussBullet bullet = BulletPool.Instance.GetBullet(firePoint.position, 
            firePoint.rotation);
        if (bullet == null) return;

        bullet.damage = damage;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = firePoint.forward * bulletSpeed;
    }
}
