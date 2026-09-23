using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Border : MonoBehaviour
{
    public float asteroidPush = 12f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 velocity = rb.linearVelocity;
                velocity.x = 0f;
                velocity.z = 0f;
                rb.linearVelocity = velocity;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Asteroid")) 
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null) 
            { 
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                Transform player = PlayerFinder.Player;
                if (player != null)
                {
                    Vector3 pushDirection = (player.transform.position - collision.transform.position).normalized;
                    pushDirection.y = 0;

                    rb.linearVelocity = pushDirection * asteroidPush;
                }
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(collision.gameObject);
        }
    }
}
