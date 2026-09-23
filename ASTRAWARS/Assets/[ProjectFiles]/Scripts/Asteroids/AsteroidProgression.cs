using UnityEngine;

public class AsteroidProgression : MonoBehaviour
{
    public float baseInterval = 3.5f;
    public float minInterval = 0.1f;
    public float decreaseInterval = 0.2f;
    public int stopDecreaseLvl = 33;

    public AsteroidSpawner spawner;
    public PlayerProgress playerProgress;

    void Start()
    {
        if (spawner == null)
            spawner = FindAnyObjectByType<AsteroidSpawner>();

        if (playerProgress == null)
            playerProgress = FindAnyObjectByType<PlayerProgress>();

        if (playerProgress != null)
            playerProgress.OnLvlUp += UpdateProgression;

        UpdateProgression();
    }

    void UpdateProgression()
    {
        if (spawner == null || playerProgress == null) return;

        int currentLvl = playerProgress.currentLvl;

        float newInterval = baseInterval;

        if (currentLvl <= stopDecreaseLvl)
        {
            int oddLevels = Mathf.FloorToInt((currentLvl + 1) / 2);
            newInterval = baseInterval - (oddLevels * decreaseInterval);
            newInterval = Mathf.Max(minInterval, newInterval);
            newInterval = Mathf.Round(newInterval * 10f) / 10f;
        }
        else
            newInterval = minInterval;

        spawner.spawnInterval = newInterval;

        int evenSteps = Mathf.FloorToInt(currentLvl / 2);

        int simple = 100;
        int splinter = 0;
        int adamantine = 0;

        int stage1Steps = Mathf.Min(evenSteps, 8);
        simple -= stage1Steps * 2;
        splinter += stage1Steps * 2;

        int stage2Steps = Mathf.Max(0, evenSteps - 8);
        simple -= stage2Steps * 2;
        splinter += stage2Steps * 1;
        adamantine += stage2Steps * 1;

        simple = Mathf.Max(0, simple);

        spawner.simpleWeight = simple;
        spawner.splinterWeight = splinter;
        spawner.adamantineWeight = adamantine;
        spawner.totalWeight = simple + splinter + adamantine;

        spawner.CancelInvoke(nameof(spawner.SpawnAsteroid));

        spawner.InvokeRepeating(
            nameof(spawner.SpawnAsteroid), 
            spawner.spawnInterval,
            spawner.spawnInterval
            );
    }
}
