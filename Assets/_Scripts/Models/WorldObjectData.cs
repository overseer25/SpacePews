
/// <summary>
/// Wraps a world object with a percent chance. Used for the pseudo-random generation of world chunks.
/// </summary>
[System.Serializable]
public class WorldObjectData
{
    public WorldObjectBase worldObject;
    public float rangeMin;
    public float rangeMax;
}
