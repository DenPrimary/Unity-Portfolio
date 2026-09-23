using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 12f;
    public float acceleration = 4f;
    public float braking = 6f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;
    public float shipRadius = 2.2f;

    public Vector3 CurrentMoveDirection { get; private set; }
    public Vector3 currentVelocity = Vector3.zero;

    public bool IsControlEnabled { get; private set; } = false;

    private Camera mainCam;
    private Vector2 moveInput;
    private Vector2 mousePosition;
    private PlayerControls controls;
    private Rigidbody rb;

    private float borderX = 250f;
    private float borderZ = 250f;

    private bool hasMouseMoved = false;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Look.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;

        rb = GetComponent<Rigidbody>();
        if (rb != null) 
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        Application.targetFrameRate = 165;
        QualitySettings.vSyncCount = 0;

        GameObject bg = GameObject.Find("background");
        if (bg != null)
        {
            Renderer rend = bg.GetComponent<Renderer>();
            if (rend != null)
            {
                Bounds bounds = rend.bounds;
                borderX = bounds.extents.x - 3f;
                borderZ = bounds.extents.z - 3f;
            }
        }
    }

    void Update()
    {
        if (!IsControlEnabled) return;
        RotateToMouse();
    }

    void FixedUpdate() 
    {
        Move();
    }

    private void LateUpdate()
    {
        if (!IsControlEnabled) return;
        ClampPlayerToMap();
    }

    void Move()
    {
        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
        if (inputDirection.magnitude > 1f)
            inputDirection.Normalize();

        if (!IsControlEnabled) 
        {
            ApplyMovement();
            return;
        }

        Vector3 fwd = transform.forward;
        Vector3 right = transform.right;
        fwd.y = 0;
        right.y = 0;
        fwd.Normalize();
        right.Normalize();

        float currentFwd = Vector3.Dot(currentVelocity, fwd);
        float currentSide = Vector3.Dot(currentVelocity, right);

        float inputFwd = inputDirection.z;
        if (Mathf.Abs(inputFwd) > 0.1f) 
        {
            float targetFwdSpeed = inputFwd * maxSpeed;

            if ((currentFwd > 0 && inputFwd < 0) || (currentFwd < 0 && inputFwd > 0))
            {
                currentFwd = Mathf.MoveTowards(currentFwd, 0, braking * Time.fixedDeltaTime);
                if (Mathf.Abs(currentFwd) < 0.01f)
                    currentFwd = Mathf.MoveTowards(currentFwd, targetFwdSpeed, acceleration * Time.fixedDeltaTime);
            }
            else
                currentFwd = Mathf.MoveTowards(currentFwd, targetFwdSpeed, acceleration * Time.fixedDeltaTime);
        }

        float inputSide = inputDirection.x;
        if (Mathf.Abs(inputSide) > 0.1f) 
        { 
            float targetSideSpeed = inputSide * maxSpeed;

            if ((currentSide > 0 && inputSide < 0) || (currentSide < 0 && inputSide > 0))
            {
                currentSide = Mathf.MoveTowards(currentSide, 0, braking * Time.fixedDeltaTime);
                if (Mathf.Approximately(currentSide, 0f))
                    currentSide = Mathf.MoveTowards(currentSide, targetSideSpeed, acceleration * Time.fixedDeltaTime);
            }
            else
                currentSide = Mathf.MoveTowards(currentSide, targetSideSpeed, acceleration * Time.fixedDeltaTime);
        }

        currentVelocity = fwd * currentFwd + right * currentSide;
        currentVelocity.y = 0;

        ApplyMovement();
    }

    void ApplyMovement() 
    { 
        if (currentVelocity.magnitude > maxSpeed)
            currentVelocity = currentVelocity.normalized * maxSpeed;

        if (currentVelocity.magnitude > 0.01f)
            CurrentMoveDirection = currentVelocity.normalized;
        else
            CurrentMoveDirection = Vector3.zero;

        Vector3 newPos = transform.position + currentVelocity * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    void RotateToMouse()
    {
        if (!IsControlEnabled) return;

        if (mousePosition.x < 0 || mousePosition.x > Screen.width ||
            mousePosition.y < 0 || mousePosition.y > Screen.height)
            return;

        if (!hasMouseMoved) 
        { 
            hasMouseMoved = true;
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            targetPoint.y = 0;

            Vector3 direction = targetPoint - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void ClampPlayerToMap()
    {
        if (rb == null) return;

        Vector3 pos = rb.position;
        pos.x = Mathf.Clamp(pos.x, -borderX, borderX);
        pos.z = Mathf.Clamp(pos.z, -borderZ, borderZ);
        rb.MovePosition(pos);
    }

    public void SetInitialVelocity(float speed) 
    {
        currentVelocity = Vector3.forward * speed;
    }

    public void EnableControl() 
    { 
        IsControlEnabled = true;
        hasMouseMoved = false;
    }

    public bool IsFiring()
    {
        return controls.Player.Fire.IsPressed();
    }

    public bool IsShielding()
    {
        return controls.Player.Shield.IsPressed();
    }
}