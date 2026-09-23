using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FragmentBehaviour : MonoBehaviour
{
    public float speed = 10f;
    public float targetCircle = 8f;
    public float floatDuration = 1.5f;

    public float bigCircleRadius = 45f;
    public float timeOutsideBigRadius = 10f;

    private Rigidbody rb;
    private Transform playerTransform;
    private bool isLaunched = false;
    private Tween rotationTween;
    private float timeOutside = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) 
            return;

        playerTransform = PlayerFinder.Player;

        float targetAngular = Random.Range(10f, 20f);
        rotationTween = DOVirtual.Float(0f, targetAngular, floatDuration, (value) => 
        { 
            if (rb != null && !isLaunched)
                rb.angularVelocity = Random.insideUnitSphere * value;
        }).SetEase(Ease.OutQuad);

        StartCoroutine(LaunchAfterDelay());
    }

    void Update() 
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > bigCircleRadius) 
        {
            timeOutside += Time.deltaTime;
            if (timeOutside >= timeOutsideBigRadius) 
            {
                Object.Destroy(gameObject);
                return;
            }
        }
        else
            timeOutside = 0f;
    }

    IEnumerator LaunchAfterDelay() 
    {
        yield return new WaitForSeconds(floatDuration);

        if (playerTransform == null) 
            yield break;

        Launch();
    }

    void Launch() 
    {
        if (isLaunched) return;
        isLaunched = true;

        rotationTween?.Kill();

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 targetPoint = playerTransform.position + new Vector3(
            Mathf.Cos(angle) * targetCircle,
            0,
            Mathf.Sin(angle) * targetCircle
        );

        Vector3 direction = (targetPoint - transform.position).normalized;
        direction += new Vector3(Random.Range(-0.15f, 0.15f), 0, Random.Range(-0.15f, 0.15f));
        direction.Normalize();

        transform.rotation = Quaternion.LookRotation(direction);

        rb.AddForce(direction * speed, ForceMode.Impulse);
    }

    private void OnDestroy()
    {
        rotationTween?.Kill();
    }

    public void SetSpeed(float value) => speed = value;
    public void SetTargetCircle(float value) => targetCircle = value;
    public void SetFloatDuration(float value) => floatDuration = value;
    public void SetBigCircleRadius(float value) => bigCircleRadius = value;
}
