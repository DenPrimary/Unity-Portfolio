using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour
{
    public Transform ballTransform;
    public float speed = 5.0f;
    public float topPoint = 4.21f;
    public float bottomPoint = -4.21f;
    public bool isBeingReset = false;

    void Start()
    {
        if (ballTransform == null)
            ballTransform = GameObject.FindGameObjectWithTag("Ball").transform;
    }

    void Update()
    {
        if (isBeingReset || ballTransform == null)
            return;

        float deltaTime = (DemoGameManager.Instance != null) ?
            Time.unscaledDeltaTime : Time.deltaTime;

        Vector3 targetPosition = new Vector3(
            transform.position.x,
            ballTransform.position.y,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * deltaTime
        );

        Vector3 clampedPosition = transform.position;
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, bottomPoint, topPoint);
        transform.position = clampedPosition;
    }

    public void SetResetState(bool resetting)
    {
        isBeingReset = resetting;
    }
}
