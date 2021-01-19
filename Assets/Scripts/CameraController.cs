using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    WorldManager world;
    public BuildableObject[] buildables;
    public float cameraSpeed = 10f;
    [HideInInspector]
    public GameObject gridObject;

    BuildableObject currentBuildableObject;
    public bool isBuilding { get; private set; }
    public bool isDestroying { get; private set; }

    public Vector3 CamBounds;

    // Start is called before the first frame update
    void Start()
    {
        currentBuildableObject = buildables[0];
        world = FindObjectOfType<WorldManager>();

        world.Money = 1000f;
    }

    // Update is called once per frame
    void Update()
    {
        bool moving = false;
        #region cam movement
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * Time.deltaTime * cameraSpeed;
            moving = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * cameraSpeed;
            moving = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * Time.deltaTime * cameraSpeed;
            moving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * Time.deltaTime * cameraSpeed;
            moving = true;
        }
        #endregion

        #region boundaries
        Vector3 pos = transform.position;
        if (pos.x < 0) { pos.x = 0; } else if (pos.x > CamBounds.x) { pos.x = CamBounds.x; };
        if (pos.y < 0) { pos.y = 0; } else if (pos.y > CamBounds.y) { pos.y = CamBounds.y; };
        transform.position = pos;
        #endregion

        #region Positioning grid
        if (moving)
        {
            Vector3 v3 = transform.position;
            gridObject.transform.position = new Vector3(Mathf.Floor(v3.x), Mathf.Floor(v3.y), gridObject.transform.position.z);
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isBuilding) { isBuilding = false; }
            // else for other things
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            world.Money += 200;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuilding = false;
            isDestroying = !isDestroying;
        }

        #region Clicking
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!GUIClick.IsOverGUI())
            {
                // Debug.Log("Clicked on world");
                if (isBuilding)
                {
                    // build tool enabled
                    // when building remember to ignore the z axis, put it on a particular one
                    // Debug.Log($"Attempting to build {currentBuildableObject.Name} at {MathsStuff.RoundVector3(Camera.main.ScreenToWorldPoint(Input.mousePosition))}");
                    world.AttemptBuildAt(MathsStuff.RoundVector3(Camera.main.ScreenToWorldPoint(Input.mousePosition)), currentBuildableObject);
                }
                else if (isDestroying)
                {
                    world.AttemptRemoveAt(MathsStuff.RoundVector3(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                }
            }
            else
            {
                // Debug.Log("Clicked on GUI");
            }
        }
        #endregion
    }

    public void SelectBlueprintAndBuild(BuildableObject blueprint)
    {
        isBuilding = true;
        currentBuildableObject = blueprint;
    }
}

public static class MathsStuff
{
    public static Vector3 RoundVector3(Vector3 v3)
    {
        //return new Vector3((int)v3.x, (int)v3.y, (int)v3.z);
        return new Vector3(Mathf.Round(v3.x), Mathf.Round(v3.y), v3.z);
    }

    public static Vector3 AverageVector3s(Vector3[] v3s)
    {
        return AvgV3s(v3s);
    }
    public static Vector3 AverageVector3s(Vector3 v31, Vector3 v32)
    {
        Vector3[] v3s = new Vector3[2];
        v3s[0] = v31;
        v3s[1] = v32;
        return AvgV3s(v3s);
    }

    private static Vector3 AvgV3s(Vector3[] vector3s)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        for (int i = 0; i < vector3s.Length; i++)
        {
            x += vector3s[i].x;
            y += vector3s[i].y;
            z += vector3s[i].z;
        }
        x /= (float)vector3s.Length;
        y /= (float)vector3s.Length;
        z /= (float)vector3s.Length;
        return new Vector3(x, y, z);
    }
}