using UnityEngine;

[CreateAssetMenu]
public class BuildableObject : ScriptableObject
{
    public enum BuildableCategory
    {
        Road, House, Decoration
    }
    /// <summary>The main name of what the object is. Shown in build menu and stuff</summary>
    public string Name;
    /// <summary>The text that is shown in a tooltip/description box in the GUI</summary>
    public string Description;
    /// <summary>The category that this object is in</summary>
    public BuildableCategory Category;
    /// <summary>How much the object will take to build</summary>
    public float Cost;
    /// <summary>The prefab that is instantiated when placed</summary>
    public GameObject Prefab;
    /// <summary>This sprite is what is shown in the build menu and possibly other places in GUI</summary>
    public Sprite UISprites;
    
    public string ID; 

    public float CircleInfluence;
    public float SquareInfluence;
    public float TriangleInfluence;
}
