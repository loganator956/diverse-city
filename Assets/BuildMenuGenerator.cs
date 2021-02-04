using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuGenerator : MonoBehaviour
{
    public GUIManager guiManager;
    public GameObject menuItemTemplate;
    List<RectTransform> menuObjects = new List<RectTransform>();
    List<BuildableObject> menuBuildables = new List<BuildableObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateMenu()
    {
        foreach (RectTransform t in menuObjects)
        {
            Destroy(t.gameObject);
        }
        BuildableObject[] buildables = Camera.main.transform.GetComponent<CameraController>().buildables;
        int x = 0;
        int y = 0;
        foreach (BuildableObject buildable in buildables)
        {
            GameObject newItem = Instantiate(menuItemTemplate, transform);
            newItem.GetComponent<RectTransform>().localPosition = new Vector3((x * 134.5f) + (x * 4), -1 * ((y * 150f) + (y * 4)));
            newItem.transform.Find("Image").GetComponent<Image>().sprite = buildable.UISprites;
            Text text = newItem.transform.Find("Text").GetComponent<Text>();
            string txt = text.text;
            txt = txt.Replace("%name%", buildable.Name);
            txt = txt.Replace("%price%", buildable.Cost.ToString());
            text.text = txt;
            newItem.GetComponent<Button>().onClick.AddListener(delegate { ButtonOnClick(buildable); });
            menuObjects.Add(newItem.GetComponent<RectTransform>());
            menuBuildables.Add(buildable);
            x++;
            if (x > 4)
            {
                x = 0;
                y++;
            }
        }
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, menuObjects[menuObjects.Count - 1].GetComponent<RectTransform>().localPosition.y + menuObjects[menuObjects.Count - 1].sizeDelta.y);
    }

    public void ButtonOnClick(BuildableObject obj)
    {
        Camera.main.GetComponent<CameraController>().SelectBlueprintAndBuild(obj);
        guiManager.ChangeWindow("!close");
    }
}
