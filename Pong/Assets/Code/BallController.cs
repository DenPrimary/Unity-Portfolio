using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{

    [Header("Anti-Stuck Settings")]
    public float minHorizontalSpeed = 2.0f;

    [Header("Launch Settings")]
    public float minLaunchAngle = 15f;
    public float maxLaunchAngle = 55f;
    public float minStartSpeed = 6.5f;
    public float maxStartSpeed = 7.5f;

    private Rigidbody2D rb;
    private BallAccelerator ballAccelerator;
    private DrawZoneController currentDrawZone;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballAccelerator = gameObject.AddComponent<BallAccelerator>();
        ballAccelerator.Initialize(rb);

        LaunchBall();
    }

    void FixedUpdate()
    {
        PreventHorizontalStuck();
    }

    private void LaunchBall()
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        ballAccelerator.ResetSpeed();

        float directionX;

        if (GameManager.Instance == null)
            directionX = Random.Range(0, 2) == 0 ? -1 : 1;
        else if (!GameManager.Instance.IsGameStarted())
            directionX = Random.Range(0, 2) == 0 ? -1 : 1;
        else if (ScoreManager.Instance == null ||
                (ScoreManager.Instance.PlayerScore == 0 && ScoreManager.Instance.EnemyScore == 0))
            directionX = Random.Range(0, 2) == 0 ? -1 : 1;
        else
            directionX = GameManager.Instance.lastScoredPlayer ? 1 : -1;

        float randomAngle = UnityEngine.Random.Range(minLaunchAngle, maxLaunchAngle);
        float randomRadians = randomAngle * Mathf.Deg2Rad;
        float randomSpeed = UnityEngine.Random.Range(minStartSpeed, maxStartSpeed);

        Vector2 direction = new Vector2(
            directionX * Mathf.Cos(randomRadians),
            Mathf.Sin(randomRadians) * (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1)
            ).normalized;
        
        Vector2 force = direction * randomSpeed;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ballAccelerator.ApplySpeedBoost(0.75f);
            if (currentDrawZone != null)
            {
                currentDrawZone.RegisterPaddleHit();
            }
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            ballAccelerator.ApplySpeedBoost(0.25f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"TRIGGER with: {other.gameObject.name}, tag: {other.gameObject.tag}");

        if (other.CompareTag("LeftWall"))
        {
            if (DemoGameManager.Instance != null)
            {
                // Демо-режим в меню
                DemoGameManager.Instance.DemoRightGoal();
            }
            else if (GameManager.Instance != null)
            {
                // Обычный игровой режим
                ScoreManager.Instance.EnemyScored();
            }
        }
        else if (other.CompareTag("RightWall"))
        {
            if (DemoGameManager.Instance != null)
            {
                // Демо-режим в меню
                DemoGameManager.Instance.DemoLeftGoal();
            }
            else if (GameManager.Instance != null)
            {
                // Обычный игровой режим
                ScoreManager.Instance.PlayerScored();
            }
        }
        else if (other.CompareTag("DrawZone"))
        {
            currentDrawZone = other.GetComponent<DrawZoneController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DrawZone"))
        {
            currentDrawZone = null;
        }
    }

    private void PreventHorizontalStuck()
    {
        Vector2 velocity = rb.linearVelocity;

        if (Mathf.Abs(velocity.x) < minHorizontalSpeed)
        {
            velocity.x = Mathf.Sign(velocity.x) * minHorizontalSpeed;
            rb.linearVelocity = velocity.normalized * ballAccelerator.GetCurrentSpeed();
        }
    }

    public void ResetBall()
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        ballAccelerator.ResetSpeed();
        LaunchBall();
    }

    public Vector2 GetVelocity() => rb.linearVelocity;
}