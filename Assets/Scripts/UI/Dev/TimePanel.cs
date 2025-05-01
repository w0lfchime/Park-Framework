using TMPro;
using UnityEngine;

public class TimePanel : MonoBehaviour
{
    [Header("UI Text References")]
    public GameObject wholeTimePanel;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI timeScaleText;
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI appSubStateTimeText;
    public TextMeshProUGUI unscaledGameTimeText;

    private float fpsUpdateInterval = 0.5f;
    private float fpsTimer = 0f;
    private int frameCount = 0;

    private float appStateStartTime = 0f;
    private float subStateStartTime = 0f;

    private bool panelIsOpen = false; 

    private void Start()
    {
        // Initialize AppState and SubState times
        appStateStartTime = Time.unscaledTime;
        subStateStartTime = Time.unscaledTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            panelIsOpen = !panelIsOpen;
            wholeTimePanel.SetActive(panelIsOpen);
        }
        UpdateFPS();
        UpdateTimeScale();
        UpdateGameTime();
        UpdateAppSubStateTime();
        UpdateUnscaledGameTime();
    }

    private void UpdateFPS()
    {
        frameCount++;
        fpsTimer += Time.unscaledDeltaTime;

        if (fpsTimer >= fpsUpdateInterval)
        {
            int fps = Mathf.RoundToInt(frameCount / fpsTimer);
            fpsText.text = $"FPS: {fps}";
            frameCount = 0;
            fpsTimer = 0f;
        }
    }

    private void UpdateTimeScale()
    {
        timeScaleText.text = $": {Time.timeScale:F2}";
    }

    private void UpdateGameTime()
    {
        float scaledGameTime = Time.time;
        gameTimeText.text = $": {scaledGameTime:F2}s";
    }

    private void UpdateAppSubStateTime()
    {
        float timeSinceAppStateSwitch = Time.unscaledTime - appStateStartTime;
        float timeSinceSubStateSwitch = Time.unscaledTime - subStateStartTime;
        appSubStateTimeText.text = $"S->AppState: {timeSinceAppStateSwitch:F2}\nS->AppState: {timeSinceSubStateSwitch:F2}";
    }


    private void UpdateUnscaledGameTime()
    {
        float unscaledGameTime = Time.unscaledTime;
        unscaledGameTimeText.text = $"REAL: {unscaledGameTime:F2}s";
    }

    public void ResetAppStateTime()
    {
        appStateStartTime = Time.unscaledTime;
    }

    public void ResetSubStateTime()
    {
        subStateStartTime = Time.unscaledTime;
    }
}
