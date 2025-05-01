using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalSettings", menuName = "Settings/GlobalSettings")]
public class GlobalSettings : ScriptableObject
{
    public event Action<GlobalSettings> OnSettingsChanged;

    private float _screenWidth;
    private float _screenHeight;

    public void UpdateSettings()
    {
        // Update settings
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;

        

        // Notify subscribers with the updated settings
        OnSettingsChanged?.Invoke(this);
    }
}
