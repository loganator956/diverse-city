using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIDropDownResolution : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Resolution res in Screen.resolutions)
        {
            Dropdown.OptionData data = new Dropdown.OptionData($"{res.width}x{res.height}");
            GetComponent<Dropdown>().options.Add(data);
        }
    }
}
