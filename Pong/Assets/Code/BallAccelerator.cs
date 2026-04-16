using UnityEngine;
using System.Collections;

public class BallAccelerator : MonoBehaviour
{
    [Header("Speed Settings")]
    public float minStartSpeed = 6.5f;
    public float maxStartSpeed = 7.5f;
    public float maxSpeed = 20f;

    private float currentSpeed;
    private Rigidbody2D ballRigidbody;

    public void ResetSpeed()
    {
        currentSpeed = UnityEngine.Random.Range(minStartSpeed, maxStartSpeed);
    }
        
    public void Initialize(Rigidbody2D rb) {
        ballRigidbody = rb;
        ResetSpeed();
    }

    public void ApplySpeedBoost(float boostAmount) {
        currentSpeed = Mathf.Min(currentSpeed + boostAmount, maxSpeed);

        if (ballRigidbody != null)
            ballRigidbody.linearVelocity = ballRigidbody.linearVelocity.normalized * currentSpeed;
    }

    public float GetCurrentSpeed() => currentSpeed;
}
