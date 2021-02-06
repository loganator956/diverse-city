using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_ToolDisplay : MonoBehaviour
{
    // this is a temporary solution to get things working. 
    public Sprite[] sprites;
    WorldManager wm = null;
    // Update is called once per frame
    void Update()
    {
        if (wm == null) { wm = GameObject.FindObjectOfType<WorldManager>(); };
    }

    public void RefreshDisplay()
    {
        if (wm != null)
        {
            switch (wm.GetCurrentTool())
            {
                case "Building":
                    GetComponent<Image>().enabled = true;
                    GetComponent<Image>().sprite = sprites[0];
                    break;
                case "Destroying":
                    GetComponent<Image>().enabled = true;
                    GetComponent<Image>().sprite = sprites[1];
                    break;
                case "none":
                    GetComponent<Image>().enabled = false;
                    break;
            }
        }
    }
}
