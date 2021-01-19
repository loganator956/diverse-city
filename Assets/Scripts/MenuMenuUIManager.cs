using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMenuUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MenuButton(string button)
    {
        switch (button.ToLower())
        {
            case "quit":
                Debug.Log("Quitting to menu");
                // Application.Quit();
                SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
                break;
            case "save":
                Debug.Log("Saving game progress");
                break;
            case "settings":
                Debug.Log("Show settings menu");
                break;
        }
    }
}
