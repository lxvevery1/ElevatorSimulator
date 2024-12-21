using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Instantiate floors <summary>

public class ElevatorShaftContructor : MonoBehaviour
{
    [SerializeField]
    private SpawnSet _spawnItems;
    private float _floorGap = 20f;
    private float _approachGap = 9.5f;


    private void Awake()
    {
        Spawn(_spawnItems, Vector3.zero, Quaternion.identity, 1);
        Spawn(_spawnItems, Vector3.up, Quaternion.identity, 2, true, FloorType.TOP_LIMIT);
        Spawn(_spawnItems, Vector3.down, Quaternion.identity, 3, true, FloorType.BOT_LIMIT);
    }

    private void Spawn(SpawnSet spawnSet, Vector3 position, Quaternion rotation,
            int id, bool isLimit = false, FloorType floorType = FloorType.BOTH)
    {
        spawnSet.Floor.name += id.ToString();
        var appFloor = spawnSet.ApproachFloor.GetComponent<Floor>();
        appFloor.FloorId = id;
        appFloor.name += id.ToString();
        appFloor.IsLimit = isLimit;

        var floor = spawnSet.Floor.GetComponent<Floor>();
        floor.FloorId = id;
        floor.IsLimit = isLimit;

        var approachPos1 = new Vector3(position.x,
                position.y - _approachGap,
                position.z);
        var approachPos2 = new Vector3(position.x,
                position.y + _approachGap,
                position.z);

        Instantiate(spawnSet.Floor, position, rotation);
        switch (floorType)
        {
            case FloorType.BOTH:
                Instantiate(spawnSet.ApproachFloor, approachPos1, rotation);
                Instantiate(spawnSet.ApproachFloor, approachPos2, rotation);
                break;
            case FloorType.TOP_LIMIT:
                Instantiate(spawnSet.ApproachFloor, approachPos1, rotation);
                break;
            case FloorType.BOT_LIMIT:
                Instantiate(spawnSet.ApproachFloor, approachPos2, rotation);
                break;
        }
    }
}

/// <summary> Items to spawn <summary>
[Serializable]
public struct SpawnSet
{
    public GameObject Floor;
    public GameObject ApproachFloor;
};


/// <summary> Floor (Limit) type <summary>
public enum FloorType
{
    TOP_LIMIT,
    BOT_LIMIT,
    BOTH
};
