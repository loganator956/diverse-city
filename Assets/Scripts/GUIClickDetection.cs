using UnityEngine;
using UnityEngine.EventSystems;

public class GUIClickDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData e)
    {
        GUIClick.currentHoveredGUI = this;
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (GUIClick.currentHoveredGUI == this) { GUIClick.currentHoveredGUI = null; };
    }
}

public static class GUIClick
{
    public static GUIClickDetection currentHoveredGUI;
    public static bool IsOverGUI()
    {
        if (currentHoveredGUI == null) { return false; } else if (!currentHoveredGUI.gameObject.activeInHierarchy) { return false; } else { return true; };
    }
}