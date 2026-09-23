using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float explotionForce = 0.005f;
    public float explotionUpward = 0.001f;

    public float restartDelay = 5f;

    private bool isDead = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Asteroid")) 
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        ExplodeShip();
        DisableShipSystem();

        StartCoroutine(RestartAfterDelay(restartDelay));
    }

    private void ExplodeShip() 
    {
        Renderer[] renders = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renders) 
        {
            if (!rend.enabled) continue;

            Transform child = rend.transform;
            child.parent = null;

            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            Vector3 randomDir = Random.insideUnitSphere.normalized;
            randomDir.y = Mathf.Abs(randomDir.y) * explotionUpward;

            rb.AddForce(randomDir * explotionForce * 0.5f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

            Destroy(child.gameObject, 3f);
        }
    }

    private void DisableShipSystem() 
    { 
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        WeaponManager wm = GetComponent<WeaponManager>();
        if (wm != null) wm.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    private IEnumerator RestartAfterDelay(float delay) 
    {
        yield return new WaitForSecondsRealtime(delay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
