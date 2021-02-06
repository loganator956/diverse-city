using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Canvas MainMenuCanvas;
    public Canvas SettingsCanvas;
    public Canvas SettingsGraphicsCanvas;
    public Dropdown SettingsGraphicsWindowModeDropdown;
    public void MenuButton(string button)
    {
        switch (button.ToLower())
        {
            case "quit":
                Application.Quit();
                break;
            case "play":
                // this should be where I send the player onto a new page where they can pick what save they want to load
                SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
                break;
            case "settings":
                CloseAllCanvas();
                SettingsCanvas.enabled = true;
                break;
            case "settings:graphics":
                CloseAllCanvas();
                SettingsGraphicsCanvas.enabled = true;
                break;
            case "settings:back":
                CloseAllCanvas();
                MainMenuCanvas.enabled = true;
                break;
            case "settings:graphics:back":
                CloseAllCanvas();
                SettingsCanvas.enabled = true;
                break;
        }
    }

    public void GraphicsResSelectChange(int selected)
    {
        Debug.Log($"Selected {selected}");
    }

    public void GraphicsWindowModeSelectChange(int selected)
    {
        Debug.Log($"Selected {selected}");
        SettingsManager.SetSettingValue("player_fullscreenmode", SettingsGraphicsWindowModeDropdown.options[selected].text);
    }

    private void CloseAllCanvas()
    {
        MainMenuCanvas.enabled = false;
        SettingsCanvas.enabled = false;
        SettingsGraphicsCanvas.enabled = false;
    }
}
