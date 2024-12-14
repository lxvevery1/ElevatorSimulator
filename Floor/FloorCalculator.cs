using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorCalculator : MonoBehaviour
{
    public Action<Tuple<float, float>> OnFloorDetectAction;


    private float _calculatedFloorUP = -1;
    private float _calculatedFloorDOWN = -1;
    private int _lastFloorDetected = -1;
    [SerializeField]
    private FloorSensor _floorBottom;
    [SerializeField]
    private FloorSensor _floorTop;

    private List<FloorSensor> _floorSensors = new List<FloorSensor>(2);


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
        if (floorId > 0 && _lastFloorDetected > 0)
        {
            _calculatedFloorUP = DetectCurrentFloorUP(new Tuple<float, float>
                    (floorId, _lastFloorDetected));
            _calculatedFloorDOWN = DetectCurrentFloorDOWN(new Tuple<float, float>
                    (floorId, _lastFloorDetected));
        }
        _lastFloorDetected = floorId;

        OnFloorDetectAction?.Invoke(
                new Tuple<float, float>(_calculatedFloorUP, _calculatedFloorDOWN));
        print($"{this.name} current floor -> {floorId}");
    }

    private float DetectCurrentFloorUP(Tuple<float, float> floorPair) =>
        (floorPair.Item1 + floorPair.Item2) * 0.5f;
    private float DetectCurrentFloorDOWN(Tuple<float, float> floorPair) =>
        (floorPair.Item1 + floorPair.Item2) * 0.5f - 0.5f;
}
