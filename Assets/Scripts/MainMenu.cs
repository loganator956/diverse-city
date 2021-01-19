using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
        }
    }
}
