using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private float targetTimeScale = 1f;
    private float transitionSpeed = 1f; // How quickly to transition time scale.
    private bool isTransitioning = false;

    public float CurrentTimeScale => Time.timeScale;
    public float DeltaTime => Time.deltaTime * Time.timeScale;
    public float UnscaledDeltaTime => Time.unscaledDeltaTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //dd on load ?
        }
    }

    private void Update()
    {
        if (isTransitioning)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, transitionSpeed * Time.unscaledDeltaTime);
            if (Mathf.Abs(Time.timeScale - targetTimeScale) < 0.01f)
            {
                Time.timeScale = targetTimeScale;
                isTransitioning = false;
            }
        }
    }

    // Set global time scale instantly
    public void SetTimeScale(float scale)
    {
        Time.timeScale = Mathf.Clamp(scale, 0f, 10f);
        targetTimeScale = scale;
        isTransitioning = false;
    }

    // Smoothly transition to a new time scale
    public void SmoothSetTimeScale(float scale, float speed)
    {
        targetTimeScale = Mathf.Clamp(scale, 0f, 10f);
        transitionSpeed = speed;
        isTransitioning = true;
    }

    // Pause the game
    public void PauseGame()
    {
        SetTimeScale(0f);
    }

    // Resume the game
    public void ResumeGame()
    {
        SetTimeScale(1f);
    }

    // Get the current time since game start (scaled or unscaled)
    public float GetTime(bool scaled = true)
    {
        return scaled ? Time.time : Time.unscaledTime;
    }

    // Run a coroutine-based timer (useful for delays or countdowns)
    public void StartTimer(float duration, Action callback)
    {
        StartCoroutine(TimerCoroutine(duration, callback));
    }

    private IEnumerator TimerCoroutine(float duration, Action callback)
    {
        float startTime = GetTime(false); // Use unscaled time to ignore slow-motion.
        while (GetTime(false) - startTime < duration)
        {
            yield return null;
        }
        callback?.Invoke();
    }
}
