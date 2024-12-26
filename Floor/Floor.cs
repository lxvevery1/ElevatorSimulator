using UnityEngine;

public class Floor : MonoBehaviour
{
    public int FloorId;
    public bool IsLimit => FloorType != FloorType.MID;
    public FloorType FloorType = FloorType.MID;
}

/// <summary> Floor (Limit) type <summary>
public enum FloorType
{
    TOP_LIMIT,
    BOT_LIMIT,
    MID
};
