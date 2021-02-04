using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuUIManager : MonoBehaviour
{
    public BuildMenuGenerator buildmenuGenerator;
    // Start is called before the first frame update
    void Start()
    {
        buildmenuGenerator.GenerateMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePage(string pageName)
    {
        switch (pageName.ToLower())
        {
            case "standard":
                Debug.Log("standard");
                break;
            case "utility":
                Debug.Log("utility");
                break;
            case "decoration":
                Debug.Log("decoration");
                break;
        }
    }

    public void SelectBlueprint(string id)
    {
        CameraController cam = FindObjectOfType<CameraController>();
        foreach(BuildableObject b in cam.buildables)
        {
            if (b.ID == id)
            {
                cam.SelectBlueprintAndBuild(b);
                GUIClick.currentHoveredGUI = null;
                gameObject.SetActive(false);
            }
        }
    }
}
