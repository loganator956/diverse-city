using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldManager : MonoBehaviour
{
    PopulationManager populationManager;
    public float DayProgress { get; private set; }
    public float DayLength = 300f;
    public List<Vector3> DecorationTiles = new List<Vector3>();
    public List<HouseScript> Houses = new List<HouseScript>();
    public Sprite externalSourceRoad;
    public Sprite[] gridBorderSprites;
    private float money;
    public float Money
    {
        get { return money; }
        set
        {
            money = value;
            foreach (InformationPanelGUI inf in FindObjectsOfType<InformationPanelGUI>())
            {
                inf.RefreshGUI();
            }
        }
    }
    [Header("World")]
    public int WorldSize = 10;
    public TileInfo[,] Tiles { get; private set; }
    [Header("Grid")]
    /// <summary>The Sprite used as the grid
    public Sprite gridSprite;
    public Color gridColour = new Color(64, 64, 64);
    public Color gridBorder = new Color(12, 12, 12);
    [Space]
    Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;

        mainCam.GetComponent<CameraController>().CamBounds = new Vector3(WorldSize, WorldSize, 0);
        Tiles = new TileInfo[WorldSize, WorldSize];
        CreateGrid();

        CreateExternalConnection(new Vector3(-1, 4, 0), Vector3.left, 10);

        populationManager = GetComponent<PopulationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        DayProgress += Time.deltaTime;
        if (DayProgress > DayLength)
        {
            DayProgress = 0f;
        }
        float progDec = DayProgress / DayLength;
        if (progDec < 0.3f)
        {
            populationManager.ScheduleIndex = 0;
        }
        else
        {
            populationManager.ScheduleIndex = 1;
        }
    }

    Transform Grid;

    public Vector2Int[] GetNeighbours(bool isRoad, Vector2Int centre)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        int cx = centre.x;
        int cy = centre.y;
        if (isRoad)
        {
            if (ValidateTile(new Vector2Int(cx - 1, cy), true)) { neighbours.Add(new Vector2Int(cx - 1, cy)); }
            if (ValidateTile(new Vector2Int(cx, cy + 1), true)) { neighbours.Add(new Vector2Int(cx, cy + 1)); }
            if (ValidateTile(new Vector2Int(cx + 1, cy), true)) { neighbours.Add(new Vector2Int(cx + 1, cy)); }
            if (ValidateTile(new Vector2Int(cx, cy - 1), true)) { neighbours.Add(new Vector2Int(cx, cy - 1)); }
        }
        return neighbours.ToArray();
    }

    public bool ValidateTile(Vector2Int tile, bool includeExternal)
    {
        if (tile.x >= 0 && tile.x < Tiles.GetLength(0) && tile.y >= 0 && tile.y < Tiles.GetLength(1)) { return true; }
        // also check if it is part of the list of external thingies
        if (includeExternal)
        {
            if (tile == new Vector2Int(-1, 4)) { return true; }
        }
        return false;
    }

    // updates values as you change them in the inspector
    void OnValidate()
    {
        if (Grid != null)
        {
            foreach (SpriteRenderer renderer in Grid.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = gridColour;
            }
        }
    }

    public Vector3 GetExternalConnection()
    {
        return new Vector3(0, 4, 0);
    }

    void CreateGrid()
    {
        float camSizeY = mainCam.orthographicSize * 2;
        float camSizeX = camSizeY * mainCam.aspect;
        GameObject gridParentGO = new GameObject("Grid Parent");
        Transform gridParentTransform = gridParentGO.transform;
        Grid = gridParentTransform;
        Camera.main.GetComponent<CameraController>().gridObject = Grid.gameObject;

        for (float x = Mathf.Floor(-camSizeX); x < Mathf.Ceil(camSizeX); x++)
        {
            for (float y = Mathf.Floor(-camSizeY); y < Mathf.Ceil(camSizeY); y++)
            {
                GameObject newGridObj = new GameObject($"Grid ({x}, {y}");
                newGridObj.transform.SetParent(gridParentTransform);
                newGridObj.transform.localPosition = new Vector3(x, y, 0);

                SpriteRenderer render = newGridObj.AddComponent<SpriteRenderer>();
                render.sprite = gridSprite;
                render.color = gridColour;
            }
        }
        GameObject bgo = new GameObject("border");
        Transform bt = bgo.transform;
        for (int i = 0; i < WorldSize; i++)
        {
            CreateBorderObj(new Vector3(0, i, 0), 0, gridBorder, bt);
            CreateBorderObj(new Vector3(i, 0, 0), 1, gridBorder, bt);
            CreateBorderObj(new Vector3(WorldSize - 1, i, 0), 2, gridBorder, bt);
            CreateBorderObj(new Vector3(i, WorldSize - 1, 0), 3, gridBorder, bt);
        }
    }

    private void CreateBorderObj(Vector3 pos, int spriteIndex, Color color, Transform parent)
    {
        GameObject b = new GameObject("Border");
        SpriteRenderer bsr = b.AddComponent<SpriteRenderer>();
        bsr.transform.position = pos;
        bsr.sprite = gridBorderSprites[spriteIndex];
        bsr.color = color;
    }

    public bool AttemptBuildAt(Vector3 pos, BuildableObject blueprint)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (Tiles[x, y] == null) { Tiles[x, y] = new TileInfo(); };
        // Debug.LogError($"{x}, {y}");
        if (Tiles[x, y].BuiltObject != null)
        {
            return false;
        }
        // else if (Money < blueprint.Cost)
        // {
        //     return false;
        // }
        else
        {
            GameObject newObj = Instantiate(blueprint.Prefab);
            newObj.transform.position = new Vector3(x, y, 0);
            Tiles[x, y].BuiltObject = blueprint;
            Tiles[x, y].BuiltTransform = newObj.transform;
            Money -= blueprint.Cost;
            if (blueprint.Category == BuildableObject.BuildableCategory.Road)
            {
                Tiles[x, y].BuiltTransform.GetComponent<Road>().world = this;
                for (int rx = x - 3; rx <= x + 3; rx++)
                {
                    for (int ry = y - 3; ry <= y + 3; ry++)
                    {
                        if (rx < 0 || rx >= Tiles.GetLength(0) || ry < 0 || ry >= Tiles.GetLength(1) || Tiles[rx, ry] == null) { }

                        else if (Tiles[rx, ry].BuiltObject.Category == BuildableObject.BuildableCategory.Road)
                        {
                            Tiles[rx, ry].BuiltTransform.GetComponent<Road>().RefreshSprite();
                        }
                    }
                }
            }
            else if (blueprint.Category == BuildableObject.BuildableCategory.House)
            {
                Houses.Add(Tiles[x, y].BuiltTransform.GetComponent<HouseScript>());
            }
            else if (blueprint.Category == BuildableObject.BuildableCategory.Decoration)
            {
                DecorationTiles.Add(new Vector3(x, y, 0));
            }
            return true;
        }
    }

    public string GetCurrentTool()
    {
        CameraController camControl = Camera.main.GetComponent<CameraController>();
        if (camControl.IsBuilding) { return "Building"; }
        else if (camControl.IsDestroying) { return "Destroying"; }
        else { return "none"; };
    }

    public bool[,] GetWalkables()
    {
        bool[,] walkables = new bool[WorldSize, WorldSize];
        for (int x = 0; x < WorldSize; x++)
        {
            for (int y = 0; y < WorldSize; y++)
            {
                // Debug.Log(Tiles[x,y] == null ? "tiles is null" : "tiles is not null");
                if (Tiles[x, y] == null || Tiles[x, y].BuiltObject == null) { walkables[x, y] = false; }
                else if (Tiles[x, y].BuiltObject.Category == BuildableObject.BuildableCategory.Road) { walkables[x, y] = true; }
                else { walkables[x, y] = false; };
            }
        }
        return walkables;
    }

    public HouseScript RequestHome()
    {
        foreach (HouseScript h in Houses)
        {
            if (h.Occupant == HouseScript.HouseOccupant.Empty) { return h; };
        }
        return null;
    }

    public bool AttemptRemoveAt(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        if (Tiles[x, y] == null)
        {
            return false;
        }
        else
        {
            Destroy(Tiles[x, y].BuiltTransform.gameObject);
            Money += Tiles[x, y].BuiltObject.Cost * 0.7f; // gets 70% of the money back
            switch (Tiles[x, y].BuiltObject.Category)
            {
                case BuildableObject.BuildableCategory.House:
                    Houses.Remove(Tiles[x, y].BuiltTransform.GetComponent<HouseScript>());
                    break;
                case BuildableObject.BuildableCategory.Decoration:
                    DecorationTiles.Remove(Tiles[x, y].BuiltTransform.position);
                    break;
                case BuildableObject.BuildableCategory.Road:
                    Tiles[x, y] = null; // important, roads check if the thingy is not null to see if it a neighbour road. 
                    foreach (Vector2Int v2i in GetNeighbours(true, new Vector2Int(x, y)))
                    {
                        try { if (Tiles[v2i.x, v2i.y].BuiltObject.Category == BuildableObject.BuildableCategory.Road) { Tiles[v2i.x, v2i.y].BuiltTransform.GetComponent<Road>().RefreshSprite(); }; } catch (NullReferenceException nullExcept) { Debug.LogWarning($"Caught Exception: {nullExcept.Message}"); };
                    }
                    break;
            }
            Tiles[x, y] = null;
            return true;
        }
    }

    private void CreateExternalConnection(Vector3 startPos, Vector3 direction, int amount)
    {
        GameObject pgo = new GameObject("ExternalConnection");
        Transform pt = pgo.transform;
        for (int i = 0; i < amount; i++)
        {
            GameObject newTile = new GameObject();
            newTile.transform.position = startPos + (direction * i);
            newTile.transform.SetParent(pt);
            newTile.AddComponent<SpriteRenderer>().sprite = externalSourceRoad;
        }
    }
}

public class TileInfo
{
    public BuildableObject BuiltObject;
    public Transform BuiltTransform;
    // public float CirclePopulationInfluenceFactor = 0f;
    // public float SquarePopulationInfluenceFactor = 0f;
    // public float TrianglePopulationInfluenceFactor = 0f;
}