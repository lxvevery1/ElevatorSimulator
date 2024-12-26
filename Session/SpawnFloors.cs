using System;
using UnityEngine;

/// <summary> Instantiate floors <summary>

public class ElevatorShaftContructor : MonoBehaviour
{
    [SerializeField]
    private SpawnSet _spawnItems;
    private float _floorGap = 20f;
    private float _approachGap = 9.5f;
    [SerializeField]
    private int _floorSpawnCount = 5;
    private const float _distanceBtwnFloors = 20f;

    private void Awake()
    {
        InitialSpawn();
    }

    private void InitialSpawn()
    {
        for (int i = 0; i < _floorSpawnCount; i++)
        {
            Vector3 position = i * _distanceBtwnFloors * Vector3.up;
            FloorType floorType = i == 0 ? FloorType.BOT_LIMIT : i == _floorSpawnCount - 1 ? FloorType.TOP_LIMIT : FloorType.MID;
            Spawn(_spawnItems, position, Quaternion.identity, i + 1, floorType);
        }
    }

    private void Spawn(SpawnSet spawnSet, Vector3 position, Quaternion rotation, int id, FloorType floorType = FloorType.MID)
    {
        Debug.Log($"Spawning floor {id} at {position} with type {floorType}");

        var floor = spawnSet.Floor.GetComponent<Floor>();

        var approachPos1 = new Vector3(position.x, position.y - _approachGap, position.z);
        var approachPos2 = new Vector3(position.x, position.y + _approachGap, position.z);

        var floorInstance = Instantiate(spawnSet.Floor, position, rotation);
        floorInstance.name = $"Floor_{id}_{floorType}";
        SetFloorProperties(floorInstance, id, floorType);

        switch (floorType)
        {
            case FloorType.BOT_LIMIT:
                SpawnApproachFloor(spawnSet, approachPos2, rotation, id, floorType, "Bot");
                break;
            case FloorType.MID:
                SpawnApproachFloor(spawnSet, approachPos1, rotation, id, floorType, "Mid_1");
                SpawnApproachFloor(spawnSet, approachPos2, rotation, id, floorType, "Mid_2");
                break;
            case FloorType.TOP_LIMIT:
                SpawnApproachFloor(spawnSet, approachPos1, rotation, id, floorType, "Top");
                break;
        }
    }

    private void SpawnApproachFloor(SpawnSet spawnSet, Vector3 position, Quaternion rotation, int id, FloorType floorType, string suffix)
    {
        var approachFloorInstance = Instantiate(spawnSet.ApproachFloor, position, rotation);
        approachFloorInstance.name = $"ApproachFloor_{id}_{suffix}";
        SetFloorProperties(approachFloorInstance, id, floorType);
    }

    private void SetFloorProperties(GameObject floorObject, int id, FloorType floorType)
    {
        var floorComponent = floorObject.GetComponent<Floor>();
        floorComponent.FloorId = id;
        floorComponent.FloorType = floorType;
    }


    /// <summary> Items to spawn <summary>
    [Serializable]
    public struct SpawnSet
    {
        public GameObject Floor;
        public GameObject ApproachFloor;
    };
}
