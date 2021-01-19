using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanelGUI : MonoBehaviour
{
    public float refreshRate = 0.5f;
    WorldManager world;
    PopulationManager population;
    Text text;
    string informationFormat = "";
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        informationFormat = text.text;
    }

    float timer = 0f;
    // Update is called once per frame
    void Update()
    {
        if (world == null) { world = FindObjectOfType<WorldManager>(); };
        if (population == null) { population = FindObjectOfType<PopulationManager>(); };
        timer += Time.deltaTime;
        if (timer > refreshRate)
        {
            timer = 0;
            RefreshGUI();
        }
    }

    public void RefreshGUI()
    {
        // refresh GUI
        string newText = informationFormat;
        newText = newText.Replace("/m", $"£{world.Money.ToString()}");
        newText = newText.Replace("/f", "N/A");
        newText = newText.Replace("/p", population.GetPopulations());
        newText = newText.Replace("/t", $"{world.DayProgress}/{world.DayLength}");
        newText = newText.Replace("/s", world.GetCurrentTool());
        //Debug.Log(newText);
        text.text = newText;
    }
}
