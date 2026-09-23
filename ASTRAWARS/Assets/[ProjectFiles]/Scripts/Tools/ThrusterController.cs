using UnityEngine;
using UnityEngine.Android;

public class ThrusterController : MonoBehaviour
{
    public ParticleSystem back1;
    public ParticleSystem back2;
    public ParticleSystem left1;
    public ParticleSystem left2;
    public ParticleSystem right1;

    public ParticleSystem right2;
    public ParticleSystem front1;
    public ParticleSystem front2;

    public float maxEmission = 30f;
    public float minEmission = 5f;

    private PlayerController playerController;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        StopAllThrusters();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController == null) return;

        float forwardInput = Input.GetAxis("Vertical");
        float sideInput = Input.GetAxis("Horizontal");

        float speed = rb != null ? rb.linearVelocity.magnitude : 0f;
        float speedFactor = Mathf.Clamp01(speed / 10f);

        if (forwardInput > 0.1f)
        {
            float emission = maxEmission * (0.5f + forwardInput * 0.5f) * (1f + speedFactor * 0.3f);
            SetThruster(back1, true, emission);
            SetThruster(back2, true, emission);
        }
        else
        {
            SetThruster(back1, false);
            SetThruster(back2, false);
        }

        if (forwardInput < -0.1f)
        {
            float emission = maxEmission * 0.3f * (1f - forwardInput);
            SetThruster(front1, true, emission);
            SetThruster(front2, true, emission);
        }
        else
        {
            SetThruster(front1, false);
            SetThruster(front2, false);
        }

        if (sideInput < -0.1f)
        {
            float emission = maxEmission * Mathf.Abs(sideInput) * (0.5f + speedFactor * 0.5f);
            SetThruster(right1, true, emission);
            SetThruster(right2, true, emission);
        }
        else 
        {
            SetThruster(right1, false);
            SetThruster(right2, false);
        }

        if (sideInput > 0.1f)
        {
            float emission = maxEmission * sideInput * (0.5f + speedFactor * 0.5f);
            SetThruster(left1, true, emission);
            SetThruster(left2, true, emission);
        }
        else 
        {
            SetThruster(left1, false);
            SetThruster(left2, false);
        }
    }

    void SetThruster(ParticleSystem thruster, bool active, float emissionRate = 0f) 
    {
        if (thruster == null) return;

        if (active)
        {
            if (!thruster.isPlaying)
                thruster.Play();

            var emission = thruster.emission;
            emission.rateOverTime = Mathf.Max(minEmission, emissionRate);
        }
        else 
        {
            if (thruster.isPlaying)
                thruster.Stop();
        }
    }

    void StopAllThrusters() 
    {
        SetThruster(back1, false);
        SetThruster(back2, false);
        SetThruster(left1, false);
        SetThruster(left2, false);
        SetThruster(right1, false);
        SetThruster(right2, false);
        SetThruster(front1, false);
        SetThruster(front2, false);
    }
}
