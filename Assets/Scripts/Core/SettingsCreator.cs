using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsCreator : MonoBehaviour
{
    
    /* This script is here to create and load settings as soon as the game is loaded. 
     */
    void Awake()
    {
        SettingsManager.OnSettingsLoaded.AddListener(OnSettingsLoaded);
        InitSettings();
    }

    void InitSettings()
    {
        Debug.Log("InitSettings");
        // player
        SettingsManager.CreateSetting("player_resolution_width", Screen.width);
        SettingsManager.CreateSetting("player_resolution_height", Screen.height);
        SettingsManager.CreateSetting("player_fullscreenmode", "FullScreenWindow"); // https://docs.unity3d.com/ScriptReference/FullScreenMode.html

        SettingsManager.LoadSettings();
    }
    void OnSettingsLoaded()
    {
        Debug.Log("Settings loaded");
    }
}
