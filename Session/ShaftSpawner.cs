using System;
using UnityEngine;

/// <summary> Instantiate floors <summary>

public class ElevatorShaftContructor : MonoBehaviour
{
    [SerializeField]
    private SpawnSet _spawnSetItems;
    private float _floorGap = 20f;
    private float _approachGap = 9.5f;
    private float _limitGap = 30f;
    [SerializeField]
    private int _floorSpawnCount = 5;
    private const float _distanceBtwnFloors = 20f;


    private void Awake()
    {
        InitialSpawn();
        SpawnElevatorAtRandomPosition();
    }

    private void InitialSpawn()
    {
        for (int i = 0; i < _floorSpawnCount; i++)
        {
            Vector3 position = i * _distanceBtwnFloors * Vector3.up;
            FloorType floorType = i == 0 ? FloorType.BOT_LIMIT :
                i == _floorSpawnCount - 1 ? FloorType.TOP_LIMIT : FloorType.MID;
            Spawn(_spawnSetItems, position, Quaternion.identity, i + 1, floorType);
        }
    }

    private void Spawn(SpawnSet spawnSet, Vector3 position, Quaternion rotation, int id,
            FloorType floorType = FloorType.MID)
    {
        var parentObject = new GameObject("Floor_Set_" + id);

        var floor = spawnSet.Floor.GetComponent<Floor>();

        var approachPos1 = new Vector3(position.x, position.y - _approachGap, position.z);
        var approachPos2 = new Vector3(position.x, position.y + _approachGap, position.z);


        switch (floorType)
        {
            case FloorType.BOT_LIMIT:
                var floorInstance = Instantiate(spawnSet.Floor,
                        new Vector3(position.x,
                            position.y - _limitGap,
                            position.z),
                        rotation);
                floorInstance.name = $"Floor_{id}_{floorType}";
                SetFloorProperties(floorInstance, parentObject.transform, id, floorType);

                SpawnApproachFloor(spawnSet, new Vector3(approachPos2.x,
                            approachPos2.y - _limitGap,
                            approachPos2.z),
                        rotation, id, floorType, "Bot", parentObject.transform);
                break;
            case FloorType.MID:
                var floorInstanceMid = Instantiate(spawnSet.Floor, position, rotation);
                floorInstanceMid.name = $"Floor_{id}_{floorType}";
                SetFloorProperties(floorInstanceMid, parentObject.transform, id,
                        floorType);

                SpawnApproachFloor(spawnSet, approachPos1, rotation, id, floorType,
                        "Mid_1", parentObject.transform);
                SpawnApproachFloor(spawnSet, approachPos2, rotation, id, floorType,
                        "Mid_2", parentObject.transform);
                break;
            case FloorType.TOP_LIMIT:
                var floorInstanceTop = Instantiate(spawnSet.Floor,
                        new Vector3(position.x,
                            position.y + _limitGap,
                            position.z),
                        rotation);
                floorInstanceTop.name = $"Floor_{id}_{floorType}";
                SetFloorProperties(floorInstanceTop, parentObject.transform, id,
                        floorType);

                SpawnApproachFloor(spawnSet, new Vector3(approachPos1.x,
                            approachPos1.y + _limitGap,
                            approachPos1.z),
                        rotation, id, floorType,
                        "Top", parentObject.transform);
                break;
        }
    }

    private void SpawnApproachFloor(SpawnSet spawnSet, Vector3 position,
            Quaternion rotation, int id, FloorType floorType, string suffix,
            Transform parentObject = null)
    {
        var approachFloorInstance = Instantiate(spawnSet.ApproachFloor, position,
                rotation);
        approachFloorInstance.name = $"ApproachFloor_{id}_{suffix}";
        SetFloorProperties(approachFloorInstance, parentObject, id, floorType);
    }

    private void SetFloorProperties(GameObject floorObject, Transform parentObject,
            int id, FloorType floorType)
    {
        var floorComponent = floorObject.GetComponent<Floor>();
        floorComponent.FloorId = id;
        floorComponent.FloorType = floorType;
        floorComponent.transform.parent = parentObject;
    }

    private void SpawnElevatorAtRandomPosition()
    {
        float minY = 0f; // Bottom floor
        float maxY = (_floorSpawnCount - 1) * _distanceBtwnFloors; // Top floor

        float randomY = UnityEngine.Random.Range(minY, maxY);

        Vector3 elevatorPosition = new Vector3(0f, randomY, 0f); // Assuming X and Z are 0
        var elevatorInstance = Instantiate(_spawnSetItems.Elevator, elevatorPosition,
                Quaternion.identity);
        elevatorInstance.name = "Elevator_Random";
        print($"Elevator spawned at random position: {elevatorPosition}");
    }

    /// <summary> Items to spawn <summary>
    [Serializable]
    public struct SpawnSet
    {
        public GameObject Floor;
        public GameObject ApproachFloor;
        public GameObject Elevator;
    };
}
