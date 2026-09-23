using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class YLockComponent : MonoBehaviour
{
    private Rigidbody rb;

    void Awake() 
    { 
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (rb.position.y != 0f)
            rb.MovePosition(new Vector3(rb.position.x, 0f, rb.position.z));

        if (rb.linearVelocity.y != 0f) 
        { 
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
        }
    }
}
