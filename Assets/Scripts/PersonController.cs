using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour
{
    string personName;
    public string PersonName
    {
        get { return name; }
    }
    // 0 - Circle
    // 1 - Square
    // 2 - Triangle
    public Sprite[] sprites;
    private Vector3 target;
    public Vector3 Target
    {
        get { return target; }
        set
        {
            target = value;
            CalculatePath();
        }
    }
    private HouseScript.HouseOccupant type;
    public HouseScript.HouseOccupant OccupantType
    {
        get { return type; }
        set
        {
            type = value;
            switch (value)
            {
                case HouseScript.HouseOccupant.Circle:
                    GetComponent<SpriteRenderer>().sprite = sprites[0];
                    break;
                case HouseScript.HouseOccupant.Square:
                    GetComponent<SpriteRenderer>().sprite = sprites[1];
                    break;
                case HouseScript.HouseOccupant.Triangle:
                    GetComponent<SpriteRenderer>().sprite = sprites[2];
                    break;
            }
        }
    }

    public enum CharacterStatus
    {
        Idle, MovingAlongPath
    }

    private CharacterStatus status;
    public CharacterStatus CurrentCharacterStatus { get { return status; } }

    List<Vector3> MovePoints = new List<Vector3>();
    public HouseScript Home;
    public WorldManager world;

    bool destinationReached = true; // shouldn't be modified directly since that bypasses the change detection
    bool DestinationReached // change this variable instead
    {
        get { return destinationReached; }
        set
        {
            if (value && destinationReached != value) // if changed to true
            {
                destinationReached = value;
                OnDestinationReach();
                // Debug.LogError("Reached Destination");
            }
            else if (!value && destinationReached != value) // if changed to false
            {
                destinationReached = value;
                OnBeginMoving();
                // Debug.Log("Began Moving");
            }
        }
    }

    private void OnBeginMoving()
    {
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = 1;
        GetComponent<SpriteRenderer>().color = c;
    }

    private void OnDestinationReach()
    {
        if (Vector3.Distance(transform.position, Target) < 1f)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 0;
            GetComponent<SpriteRenderer>().color = c;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldManager>();
        personName = "Cool guy"; // replace with random names
    }

    // Update is called once per frame
    void Update()
    {
        // if (Home == null)
        // {
        //     Home = world.RequestHome();
        // }
        if (Input.GetKeyDown(KeyCode.I)) { SendToBed(); } else if (Input.GetKeyDown(KeyCode.U)) { SendToWorkOrPlay(); };

        if (CurrentCharacterStatus == CharacterStatus.MovingAlongPath && MovePoints.Count != 0)
        {
            // face character to the next point
            // move towards it. 
            Vector3 nextPos = MovePoints[0];
            // nextPos.y = transform.position.y; // making the character not change height cos that be bad
            // transform.LookAt(nextPos); // rotate to look at the next position
            Vector3 directionToNext = (nextPos - transform.position).normalized;
            transform.position += new Vector3(nextPos.x - transform.position.x, nextPos.y - transform.position.y, 0).normalized * 2f * Time.deltaTime; // move towards the next position
            if (Vector3.Distance(transform.position, nextPos) < 0.05f)
            {
                MovePoints.RemoveAt(0); // if within range of the next position, remove it and move towards the next one
                if (MovePoints.Count == 0) // if no more move points remaining
                {
                    status = CharacterStatus.Idle; // set idle
                    DestinationReached = true; // mark destination reached to true
                }
            }
        }
    }

    public void SendToBed()
    {
        Target = Home.transform.position;
    }

    public void SendToWorkOrPlay()
    {
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < 0.5f)
        {
            // go play
            /* Find the nearest least busy park
             * Calculate path there
             * Walk there
             * Disappear
             */
            Vector3 closestPark = Vector3.zero;
            float closestDistance = 438479147847f;
            foreach (Vector3 deco in world.DecorationTiles)
            {
                if (Vector3.Distance(deco, transform.position) < closestDistance)
                {
                    closestPark = deco;
                    closestDistance = Vector3.Distance(deco, transform.position);
                }
            }
            Target = closestPark;
        }
    }
    bool[,] walkableTiles;
    float[,] tileScores;
    public void CalculatePath()
    {
        Debug.Log(world == null ? "world is null" : "world is not null");
        walkableTiles = world.GetWalkables();
        tileScores = new float[world.WorldSize, world.WorldSize];
        DestinationReached = false; // resetting the destination reached to false
        Debug.Log($"CalculatePath : Working out path from {transform.position} to {Target}");
        for (int x = 0; x < tileScores.GetLength(0); x++) { for (int y = 0; y < tileScores.GetLength(1); y++) { tileScores[x, y] = -1f; }; }; // resetting the values
        tileScores[(int)Target.x, (int)Target.y] = 0; // setting the target square to 0
        AssignTileVals(new Vector2Int((int)Target.x, (int)Target.y)); // beginning the score assignment cycle

        Debug.Log("CalculatePath : Generated Tile Values");

        Vector2Int currentPos = new Vector2Int((int)transform.position.x, (int)transform.position.y); // work out what tile index the player is currently on
        float currentVal = tileScores[currentPos.x, currentPos.y]; // work out the current value of the tile index

        List<Vector3> movePointList = new List<Vector3>();
        int lastTileX = currentPos.x; // keeping track of the last tile in the path
        int lastTileY = currentPos.y;
        for (int i = (int)currentVal; i >= 0; i--)
        {
            #region Choosing tiles
            // pick which tile matches this value
            try
            {
                // checking left
                if (tileScores[lastTileX - 1, lastTileY] == i)
                {
                    movePointList.Add(new Vector3(lastTileX - 1, lastTileY, 0));
                    lastTileX -= 1;
                    continue; // finishes this iteration
                }
            }
            catch (IndexOutOfRangeException) { };
            try
            {
                // checking right
                if (tileScores[lastTileX + 1, lastTileY] == i)
                {
                    movePointList.Add(new Vector3(lastTileX + 1, lastTileY, 0));
                    lastTileX += 1;
                    continue;
                }
            }
            catch (IndexOutOfRangeException) { };
            try
            {
                // checking down
                if (tileScores[lastTileX, lastTileY - 1] == i)
                {
                    movePointList.Add(new Vector3(lastTileX, lastTileY - 1, 0));
                    lastTileY -= 1;
                    continue;
                }
            }
            catch (IndexOutOfRangeException) { };
            try
            {
                // checking up
                if (tileScores[lastTileX, lastTileY + 1] == i)
                {
                    movePointList.Add(new Vector3(lastTileX, lastTileY + 1, 0));
                    lastTileY += 1;
                    continue;
                }
            }
            catch (IndexOutOfRangeException) { };
            #endregion
        }

        MovePoints = movePointList; // assigning the movepoints

        status = CharacterStatus.MovingAlongPath; // changing the current character status

        for (int i = 0; i < MovePoints.Count; i++)
        {
            MovePoints[i] += new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f), 0);
        }

        // used for debugging
        List<string> moveListString = new List<string>();
        foreach (Vector3 point in MovePoints)
        {
            moveListString.Add(point.ToString());
        }
        File.WriteAllLines("movelist.txt", moveListString.ToArray());


        List<string> linesToPrint = new List<string>();
        for (int y = 0; y < tileScores.GetLength(1); y++)
        {
            string line = "";
            for (int x = 0; x < tileScores.GetLength(0); x++)
            {
                line += $"{tileScores[x, y]},";
            }
            linesToPrint.Add(line);
        }
        File.WriteAllLines("path.csv", linesToPrint.ToArray());
    }

    private void AssignTileVals(Vector2Int centreTile)
    {
        // work out values of above, below, left and right tiles and then run AssignTileVals for all of them
        int x = centreTile.x;
        int y = centreTile.y;
        float centreScore = tileScores[x, y];
        AssignTileVal(new Vector2Int(x - 1, y), centreScore);
        AssignTileVal(new Vector2Int(x + 1, y), centreScore);
        AssignTileVal(new Vector2Int(x, y - 1), centreScore);
        AssignTileVal(new Vector2Int(x, y + 1), centreScore);
    }

    /// <summary>This function is to be called by the AssignTileVals.</summary>
    void AssignTileVal(Vector2Int tile, float centreScore)
    {
        try
        {
            float newTileScore = centreScore;
            if (walkableTiles[tile.x, tile.y]) // checking if the tile is walkable
            {
                newTileScore += 1f; // calculating the new score
            }
            else
            {
                newTileScore += 4000f;
            }

            if (newTileScore < tileScores[tile.x, tile.y] || tileScores[tile.x, tile.y] == -1f) // checking if the new score is lower than the other score (Meaning it is a quicker path)
            {
                tileScores[tile.x, tile.y] = newTileScore; // overwriting the score
                AssignTileVals(new Vector2Int(tile.x, tile.y)); // repeating the process to cover the whole map
            }
        }
        catch (IndexOutOfRangeException) { }; // handles out of range exceptions (Occurs alot at map edges)
    }
}
