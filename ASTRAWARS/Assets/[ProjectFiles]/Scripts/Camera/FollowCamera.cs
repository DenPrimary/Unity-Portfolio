using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Follow")]
    public Vector3 offset = new Vector3(0, 10, 0);
    public float smoothSpeed = 1f;
    public float horizontalMargin = 27f;
    public float verticalMargin = 15f;


    [Header("Speed Offset")]
    public float maxOffsetDistance = 12f;
    public float offsetSmoothSpeed = 3f;
    

    private Transform target;
    private float minX, maxX, minZ, maxZ;

    private Vector3 currentOffset = Vector3.zero;

    void Start()
    {
        PlayerSpawner.OnPlayerSpawned += SetTarget;

        GameObject bg = GameObject.Find("background");
        if (bg != null)
        {
            Renderer rend = bg.GetComponent<Renderer>();
            if (rend != null)
            {
                Bounds bounds = rend.bounds;
                minX = bounds.min.x + horizontalMargin;
                maxX = bounds.max.x - horizontalMargin;
                minZ = bounds.min.z + verticalMargin;
                maxZ = bounds.max.z - verticalMargin;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        PlayerController controller = target.GetComponent<PlayerController>();
        Vector3 moveDirection = Vector3.zero;

        if (controller != null) 
        {
            moveDirection = controller.CurrentMoveDirection;

            float speed = controller.currentVelocity.magnitude;
            float speedNormalized = Mathf.Clamp01(speed / 12f);

            Vector3 targetOffset = moveDirection * speedNormalized * maxOffsetDistance;
            targetOffset.y = 0;

            currentOffset = Vector3.Lerp(currentOffset, targetOffset, offsetSmoothSpeed * Time.deltaTime);

            Vector3 desiredPosition = target.position + offset + currentOffset;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    void SetTarget(Transform playerTransform)
    {
        Transform pivot = playerTransform.Find("PivotPoint");
        if (pivot != null)
            target = pivot;
        else 
            target = playerTransform;
    }
}
