using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorCalculator : MonoBehaviour
{
    public Action<float> OnFloorDetectAction;


    private float _calculatedFloor = -1;
    private int _lastFloorDetected = -1;
    [SerializeField]
    private FloorSensor _floorBottom;
    [SerializeField]
    private FloorSensor _floorTop;

    private List<FloorSensor> _floorSensors = new List<FloorSensor>();


    private void Awake() => Init();

    private void Init()
    {
        _floorSensors.Clear();
        _floorSensors.Add(_floorBottom);
        _floorSensors.Add(_floorTop);

        foreach (var sensor in _floorSensors)
        {
            sensor.OnFloorDetectAction += OnFloorDetect;
        }
    }

    private void OnFloorDetect(int floorId)
    {
        _calculatedFloor = DetectCurrentFloor(new Tuple<float, float>
                (floorId, _lastFloorDetected));
        _lastFloorDetected = floorId;

        OnFloorDetectAction?.Invoke(_calculatedFloor);
        print($"{this.name} current floor -> {floorId}");
    }

    private float DetectCurrentFloor(Tuple<float, float> floorPair) =>
        (floorPair.Item1 + floorPair.Item2) * 0.5f;
}
