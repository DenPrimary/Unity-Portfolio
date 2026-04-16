using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance;

    [Header("Pause Settings")]
    public float pauseAfterGoal = 0.8f;
    public float pauseAfterDraw = 1.2f;
    public float pauseAfterGameEnd = 2f;
    public bool pauseOnlyPhysics = true;

    public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Debug.LogWarning("Multiple GameTimeManager instances found!");
            Destroy(gameObject);
        }
    }

    private IEnumerator SetPauseForDuration(float duration) 
    {
        Time.timeScale = pauseOnlyPhysics ? 0f : 1f;
        isPaused = true;
        Debug.Log("Game paused (physics only: " + pauseOnlyPhysics + ")");

        float pauseEndTime = Time.realtimeSinceStartup + duration;

        while(Time.realtimeSinceStartup < pauseEndTime)
            yield return null;

        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game resumed");
    }

    public IEnumerator PauseAfterGoal() 
    {
        Debug.Log("Pause after goal: " + pauseAfterGoal + "s");
        yield return StartCoroutine(SetPauseForDuration(pauseAfterGoal));
    }

    public IEnumerator PauseAfterDraw() 
    {
        Debug.Log("Pause after draw: " + pauseAfterDraw + "s");
        yield return StartCoroutine(SetPauseForDuration(pauseAfterDraw));
    }

    public IEnumerator PauseAfterGameEnd() 
    {
        Debug.Log("Pause after game end: " + pauseAfterGameEnd + "s");
        yield return StartCoroutine(SetPauseForDuration(pauseAfterGameEnd));
    }

    public void SetPause(bool pause) 
    { 
        Time.timeScale = pause ? 0f : 1f;
        isPaused = pause;
        Debug.Log(pause ? "Game manually paused" : "Game manually resumed");
    }

    public bool IsPaused() => isPaused;
}
