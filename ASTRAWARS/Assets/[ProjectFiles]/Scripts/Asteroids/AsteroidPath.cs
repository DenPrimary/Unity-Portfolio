using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AsteroidPath : MonoBehaviour
{
    public float correctionStrength = 0.5f;
    public float maxAngleChange = 5f;
    public float maxDistance = 50f;
    public float minDistance = 8f;

    public float sineAmplitude = 0.3f;
    public float sineFrequency = 0.5f;

    private Transform playerTransform;
    private Dictionary<int, float> asteroidOffsets = new Dictionary<int, float>();
    private List<Asteroid> asteroids = new List<Asteroid>();

    void Start()
    {
        playerTransform = PlayerFinder.Player;
    }

    void Update()
    {
        if (playerTransform == null) return;

        var asteroids = FindObjectsByType<Asteroid>()
            .Where(ast => ast != null)
            .Select(ast => new {
                asteroid = ast,
                body = ast.GetComponent<Rigidbody>(),
                distance = Vector3.Distance(ast.transform.position, playerTransform.position)
            })
            .Where(x => x.body != null)
            .Where(x => x.distance < maxDistance && x.distance > minDistance)
            .OrderBy(x => x.distance)
            .ToList();

        foreach (var ast in asteroids) 
            CorrectAsteroid(ast.asteroid, 
                ast.body, 
                playerTransform.position - ast.asteroid.transform.position
            );
    }

    private void CorrectAsteroid(Asteroid asteroid, Rigidbody rb, Vector3 toPlayer) 
    {
        Vector3 currentDirection = rb.linearVelocity.normalized;
        if (currentDirection.magnitude < 0.1f) return;

        Vector3 targetDirection = toPlayer.normalized;
        targetDirection.y = 0;

        int id = asteroid.GetHashCode();
        if (!asteroidOffsets.ContainsKey(id))
            asteroidOffsets[id] = Random.Range(0f, 100f);

        float offset = asteroidOffsets[id];
        float timeOffset = Time.time * sineFrequency + offset;
        float sineOffset = Mathf.Sin(timeOffset) * sineAmplitude;

        Vector3 sideDirection = Vector3.Cross(targetDirection, Vector3.up).normalized;
        Vector3 curvedTarget = (targetDirection + sideDirection * sineOffset).normalized;

        float correction = correctionStrength * Time.deltaTime;
        Vector3 newDirection = Vector3.Slerp(currentDirection, curvedTarget, correction);

        float angleDelta = Vector3.Angle(currentDirection, newDirection);
        if (angleDelta > 0.001f && angleDelta > maxAngleChange * Time.deltaTime)
            newDirection = Vector3.Slerp(currentDirection, newDirection, maxAngleChange * Time.deltaTime / angleDelta);

        float currentSpeed = rb.linearVelocity.magnitude;
        rb.linearVelocity = newDirection * currentSpeed;

        if (newDirection.sqrMagnitude > 0.001f) 
        {
            asteroid.transform.rotation = Quaternion.Slerp(
                asteroid.transform.rotation,
                Quaternion.LookRotation(newDirection),
                Time.deltaTime * 2f
            );
        }
    }

    public void ClearOffset(int id) {
        asteroidOffsets.Remove(id);
    }
}
