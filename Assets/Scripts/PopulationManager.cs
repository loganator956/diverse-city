using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public GameObject basicPerson;
    public List<PersonController> people = new List<PersonController>();
    public float populationTickInterval = 10f;
    [Range(0f, 1f)]
    public float populationTickVariation = 0.1f;
    [Range(0f, 1f)]
    public float growthProbability = 0.4f;

    public float[] Populations { get; private set; }

    int scheduleIndex = 32;
    public int ScheduleIndex
    {
        get { return scheduleIndex; }
        set
        {
            if (scheduleIndex != value)
            {
                // schedule index has changed
                scheduleIndex = value;
                if (scheduleIndex > 1) { scheduleIndex = 1; } // loops back around
                ScheduleChange();
            }
            else
            {
                // schedule index has not changed
            }
        }
    }

    WorldManager world;

    float interval = 0f;
    float timer = 0f;
    void Start()
    {
        interval = Random.Range(populationTickInterval - (populationTickVariation * populationTickInterval), populationTickInterval + (populationTickVariation * populationTickInterval));
        world = GetComponent<WorldManager>();
        // if (world == null) { Debug.Log("world is null"); };
        Populations = new float[3];
    }

    void Update()
    {
        // Debug.Log($"timer {timer} | interval {interval}");
        timer += Time.deltaTime;
        if (timer > populationTickInterval)
        {
            timer = 0f;
            interval = Random.Range(populationTickInterval - (populationTickVariation * populationTickInterval), populationTickInterval + (populationTickVariation * populationTickInterval));
            OnPopulationTick();
        }
    }

    void OnPopulationTick()
    {
        // Debug.Log($"Population Tick. There are {world.Houses.Count} houses");

        foreach (HouseScript house in world.Houses)
        {
            float randomGrowth = Random.Range(0f, 1f);
            // Debug.Log("PopulationTick: " + randomGrowth + " chance of growing");
            if (randomGrowth < growthProbability)
            {
                if (house.Occupant == HouseScript.HouseOccupant.Empty)
                {
                    //  foreach surrounding tile
                    //      total up the PopulationInfluenceFactors foreach shape
                    //      work out ratios of percentages of the influence factors. EG: 0.3:0.2:0.5 (Circle:Square:Triangle)
                    //      then Random.Range(0, 1f) and see what what shape range it is in (0:0.3, 0.3:0.5, 0.5:1.0)
                    float circleInfluence = 0f;
                    float squareInfluence = 0f;
                    float triangleInfluence = 0f;
                    for (int x = (int)house.transform.position.x - 5; x < house.transform.position.x + 5; x++)
                    {
                        for (int y = (int)house.transform.position.y - 5; y < house.transform.position.y + 5; y++)
                        {
                            if (x < 0 || x >= world.WorldSize || y < 0 || y >= world.WorldSize || world.Tiles[x, y] == null)
                            {
                                // Debug.Log("OnPopulationTick() is Out of Range");
                            }
                            else
                            {
                                if (world.Tiles[x, y].BuiltObject != null)
                                {
                                    // Debug.Log(world.Tiles[x, y].BuiltObject == null ? "builtobject is null" : "built object is not null");
                                    circleInfluence += world.Tiles[x, y].BuiltObject.CircleInfluence;
                                    squareInfluence += world.Tiles[x, y].BuiltObject.SquareInfluence;
                                    triangleInfluence += world.Tiles[x, y].BuiltObject.TriangleInfluence;
                                }
                            }
                        }
                    }
                    float total = circleInfluence + squareInfluence + triangleInfluence;
                    float c = circleInfluence / total;
                    float s = squareInfluence / total;
                    float t = triangleInfluence / total;
                    // Debug.Log($"{c}, {s}, {t}");

                    HouseScript.HouseOccupant newOccupant = HouseScript.HouseOccupant.Empty;
                    float r = Random.Range(0f, 1f);
                    if (r < c) { newOccupant = HouseScript.HouseOccupant.Circle; }
                    else if (r < c + s) { newOccupant = HouseScript.HouseOccupant.Square; }
                    else if (r < c + s + t) { newOccupant = HouseScript.HouseOccupant.Triangle; };
                    house.Occupant = newOccupant;

                    switch (newOccupant)
                    {
                        case HouseScript.HouseOccupant.Circle:
                            Populations[0]++;
                            break;
                        case HouseScript.HouseOccupant.Square:
                            Populations[1]++;
                            break;
                        case HouseScript.HouseOccupant.Triangle:
                            Populations[2]++;
                            break;
                    }

                    GameObject newPerson = Instantiate(basicPerson);
                    newPerson.transform.position = world.GetExternalConnection();
                    PersonController pc = newPerson.GetComponent<PersonController>();
                    people.Add(pc);
                    pc.Home = house;
                    pc.OccupantType = newOccupant;
                    pc.world = world;
                    pc.Target = house.transform.position;
                }
            }
        }
    }

    public string GetPopulations()
    {
        return ($"{Populations[0]}c, {Populations[1]}s, {Populations[2]}t");
    }

    private void ScheduleChange()
    {
        switch (scheduleIndex)
        {
            case 0:
                Debug.Log("Sleep time");
                foreach (PersonController person in people)
                {
                    person.SendToBed();
                }
                break;
            case 1:
                Debug.Log("Work/play time");
                foreach (PersonController person in people)
                {
                    person.SendToWorkOrPlay();
                }
                break;
        }
    }
}
