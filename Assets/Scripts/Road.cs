using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    /// <summary> the road sprites
    /// 0 - Default (No connections)
    /// 1 - Deadend     (1)
    /// 2 - Straight    (2)
    /// 3 - 90 Bend     (2)
    /// 4 - T-Junction  (3)
    /// 5 - Crossroad   (4)
    /// </summary>
    public Sprite[] roadSprites;
    public WorldManager world;
    void Start()
    {

    }

    public void RefreshSprite()
    {
        // get local index
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);
        int connectionCount = 0; // to identify which road type it is
        List<Vector3> directions = new List<Vector3>(); // to identify orientation of roads
        #region Errors
        if (world == null) { throw new Exception("WorldManager not set"); };
        if (world.Tiles == null) { throw new Exception("WorldManager.Tiles is null, Not Generated world yet?"); };
        #endregion
        foreach (Vector2Int v2i in world.GetNeighbours(true, new Vector2Int(x, y)))
        {
            if (!world.ValidateTile(v2i, false))
            {
                connectionCount++;
                directions.Add(new Vector3(v2i.x - x, v2i.y - y));
            }
            else if (world.Tiles[v2i.x, v2i.y] != null)
            {
                if (world.Tiles[v2i.x, v2i.y].BuiltObject.Category == BuildableObject.BuildableCategory.Road)
                {
                    connectionCount++;
                    directions.Add(new Vector3(v2i.x - x, v2i.y - y));
                }
            }
        }
        int rotation = 0;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        switch (connectionCount)
        {
            case 0:
                renderer.sprite = roadSprites[0];
                break;
            case 1:
                renderer.sprite = roadSprites[1];
                rotation = GetRotationFromDirection(directions[0]);
                break;
            case 2:
                Vector3 avgV3 = MathsStuff.AverageVector3s(directions[0], directions[1]);
                if (avgV3 == Vector3.zero)
                {
                    rotation = GetRotationFromDirection(directions[0]);
                    renderer.sprite = roadSprites[2];
                }
                else
                {
                    renderer.sprite = roadSprites[3];
                    if (avgV3 == new Vector3(-0.5f, -0.5f)) { rotation = 3; }
                    else if (avgV3 == new Vector3(-0.5f, 0.5f)) { rotation = 2; }
                    else if (avgV3 == new Vector3(0.5f, 0.5f)) { rotation = 1; }
                    else if (avgV3 == new Vector3(0.5f, -0.5f)) { rotation = 0; }
                    else { throw new System.Exception("Cannot find the matching 90 degree bend rotation"); };
                }
                break;
            case 3:
                Vector3 avgDir = MathsStuff.AverageVector3s(directions.ToArray());
                avgDir = Vector3.Normalize(avgDir);
                rotation = GetRotationFromDirection(avgDir);
                rotation = 3 - rotation;
                renderer.sprite = roadSprites[4];
                break;
            case 4:
                renderer.sprite = roadSprites[5];
                break;
        }
        transform.rotation = Quaternion.Euler(0, 0, rotation * 90f);
        //Debug.Log(transform.position.ToString() + " | Refreshed Sprite " + connectionCount + " | " + rotation);
    }

    public int GetRotationFromDirection(Vector3 direction)
    {
        if (direction == Vector3.left) { return 0; }
        else if (direction == Vector3.up) { return 1; }
        else if (direction == Vector3.right) { return 2; }
        else if (direction == Vector3.down) { return 3; }
        else { throw new System.Exception($"Cannot convert {direction.ToString()} to a rotation"); };
    }
}
