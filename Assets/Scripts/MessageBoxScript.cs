using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MessageBoxScript : MonoBehaviour
{
    public enum MessageType
    {
        YesNo, OK
    }
    public MessageType Type;
    UnityEvent OnYesClickEvent = new UnityEvent();
    UnityEvent OnNoClickEvent = new UnityEvent();

    public Button yesButton { get; private set; }
    public Button noButton { get; private set; }

    void Start()
    {
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            switch (b.gameObject.name.ToLower())
            {
                case "yesbutton":
                    yesButton = b;
                    Debug.Log("Applied yes button");
                    break;
                case "nobutton":
                    noButton = b;
                    Debug.Log("Applied no button");
                    break;
            }
        }
    }



    public void FireClickEvent(string button)
    {
        switch (button.ToLower())
        {
            case "yes":
                OnYesClickEvent.Invoke();
                break;
            case "no":
                OnNoClickEvent.Invoke();
                break;
        }
    }
}
