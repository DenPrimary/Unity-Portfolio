using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;

public class DrawZoneController : MonoBehaviour
{
    [Header("Draw Settings")]
    public int drawHitsThreshold = 6;
    public float maxHorizontalAngle = 10f;

    private int currentHitsCount = 0;
    private bool isActive = false;
    private BallController ballInZone;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            ballInZone = other.GetComponent<BallController>();
            Debug.Log("Ball entered draw zone!");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ball") && ballInZone != null)
        {
            CheckDrawConditions(ballInZone);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            ResetDraw();
            ballInZone = null;
            Debug.Log("Ball exited draw zone!");
        }
    }

    void CheckDrawConditions(BallController ball)
    {
        Vector2 velocity = ball.GetVelocity();
        float angle = Vector2.Angle(velocity, Vector2.right);
        bool isHorizontal = (angle <= maxHorizontalAngle) || (angle >= 180 - maxHorizontalAngle);

        if (isHorizontal && !isActive)
        {
            isActive = true;
            currentHitsCount = 0;
            Debug.Log("Draw zone activated! Horizontal movement detected. Angle now: " + angle);
        }
        else if (!isHorizontal && isActive)
        {
            ResetDraw();
        }
    }

    void ResetDraw()
    {
        isActive = false;
        currentHitsCount = 0;
        Debug.Log("Draw zone deactivated!");
    }

    public void RegisterPaddleHit()
    {
        if (isActive && ballInZone != null)
        {
            currentHitsCount++;
            Debug.Log($"Draw hits: {currentHitsCount}/{drawHitsThreshold}");

            if (currentHitsCount >= drawHitsThreshold)
            {
                Debug.Log("Draw declared!");
                if (gameManager != null)
                {
                    gameManager.DeclareDraw();
                }
                ResetDraw();
            }
        }
    }
}
