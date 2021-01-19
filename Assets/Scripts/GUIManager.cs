using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public GUIWindow[] windows;
    public GUIWindow openWindow;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeWindow(string windowName)
    {
        if (windowName == "!close")
        {
            foreach (GUIWindow window in windows)
            {
                window.WinTransform.gameObject.SetActive(false);
            }
            return;
        }
        bool foundMatch = false;
        foreach (GUIWindow w in windows)
        {
            if (w.WindowName.ToLower() == windowName.ToLower())
            {
                // Debug.Log("Showing window " + windowName);
                foundMatch = true;
                openWindow = w;
                // found the chosen window to open
                // close all other windows
                foreach (GUIWindow window in windows)
                {
                    window.WinTransform.gameObject.SetActive(false);
                }
                w.WinTransform.gameObject.SetActive(true);
                break;
            }
        }
        if (!foundMatch)
        {
            throw new WindowNameNotFoundException($"Couldn't find window of name: {windowName}");
        }
    }
}

[System.Serializable]
public class GUIWindow
{
    public string WindowName;
    public bool IsVisible;
    public Transform WinTransform;
}

[System.Serializable]
public class WindowNameNotFoundException : System.Exception
{
    public WindowNameNotFoundException() { }
    public WindowNameNotFoundException(string message) : base(message) { }
    public WindowNameNotFoundException(string message, System.Exception inner) : base(message, inner) { }
    protected WindowNameNotFoundException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}