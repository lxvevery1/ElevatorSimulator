using System;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public int FloorId;
    public bool IsLimit;


    private Floor()
    {
        FloorId = -1;
        IsLimit = false;
    }

    private Floor(int floorId)
    {
        FloorId = floorId;
        IsLimit = false;
    }

    public Floor(float floorId)
    {
        FloorId = (int)Math.Floor(floorId);
    }

    private Floor(int floorId, bool isLimit)
    {
        FloorId = floorId;
        IsLimit = isLimit;
    }
}
