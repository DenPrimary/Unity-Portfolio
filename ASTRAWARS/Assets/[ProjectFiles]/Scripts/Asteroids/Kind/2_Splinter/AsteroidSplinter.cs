using System.Collections;
using UnityEngine;

public class AsteroidSplinter : Asteroid
{
    [Header("Splinter Settings")]
    public int minFragmentsCount = 3;
    public int maxFragmentsCount = 5;
    public float fragmentSpeed = 10f;
    public float targetCircle = 8f;
    public float fragmentsSizeMultiplier = 0.45f;
    public float spawnSpread = 3f;

    public float floatDuration = 1.5f;

    public GameObject fragmentPrefab;

    protected override float GetHealthMultiplier() 
    {
        return 2f;
    }

    protected override void Die() 
    {
        Transform player = PlayerFinder.Player;
        if (player == null) return;

        int count = Random.Range(minFragmentsCount, maxFragmentsCount + 1);

        int expFromSplinter = Mathf.RoundToInt(GetCalculatedHealth());
        FragmentGroup group = new FragmentGroup(expFromSplinter, count);
        
        for (int i = 0; i < count; i++) 
            SpawnFragment(group);
    }

    void SpawnFragment(FragmentGroup group) 
    {
        if (fragmentPrefab == null) 
        {
            Debug.LogError("fragmentPrefab is not found!");
            return;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnSpread, spawnSpread),
            0,
            Random.Range(-spawnSpread, spawnSpread)
            );

        Vector3 spawnPosition = transform.position + randomOffset;

        GameObject fragment = Instantiate(fragmentPrefab, spawnPosition, Quaternion.identity);

        fragment.transform.rotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );

        float fragmentSize = currentSize * fragmentsSizeMultiplier;
        fragmentSize = Mathf.Max(0.2f, fragmentSize);

        fragment.transform.localScale = Vector3.one * fragmentSize;

        Fragment frag = fragment.GetComponent<Fragment>();
        if (frag != null)
        {
            frag.SetHealth(1f);
            frag.SetGroup(group);
        }

        FragmentBehaviour behaviour = fragment.GetComponent<FragmentBehaviour>();
        if (behaviour != null) 
        {
            behaviour.SetSpeed(fragmentSpeed);
            behaviour.SetTargetCircle(targetCircle);
            behaviour.SetFloatDuration(floatDuration);
            behaviour.SetBigCircleRadius(45f);
        }

        Debug.Log($"Fragment spawned! Size: {fragmentSize:F2}");
    }
}
