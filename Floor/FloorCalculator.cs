using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorCalculator : MonoBehaviour
{
    public Action<Tuple<Tuple<float, float>, bool>> OnFloorDetectAction;
    public Action<float> OnApproachFloorDetectAction;


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
            sensor.OnApproachFloorDetectAction += OnApproachFloorDetect;
        }
    }

    private void OnApproachFloorDetect(Floor floor)
    {
        OnApproachFloorDetectAction.Invoke(floor.FloorId);
        print($"{this.name} approaching floor -> {floor.FloorId}");
    }

    private void OnFloorDetect(Floor floor)
    {
        if (floor.FloorId > 0 && _lastFloorDetected > 0)
        {
            Tuple<float, float> topBottomFloors = new Tuple<float, float>
                (floor.FloorId, _lastFloorDetected);
            _calculatedFloorUP = DetectCurrentFloorUP(topBottomFloors);
            _calculatedFloorDOWN = DetectCurrentFloorDOWN(topBottomFloors);
        }
        _lastFloorDetected = floor.FloorId;

        OnFloorDetectAction?.Invoke(
                new Tuple<Tuple<float, float>, bool>
                (new Tuple<float, float>(_calculatedFloorDOWN, _calculatedFloorUP),
                 floor.IsLimit));
        print($"{this.name} current floor -> {floor.FloorId}");
    }

    private float DetectCurrentFloorUP(Tuple<float, float> floorPair) =>
        (floorPair.Item1 + floorPair.Item2) * 0.5f;
    private float DetectCurrentFloorDOWN(Tuple<float, float> floorPair) =>
        (floorPair.Item1 + floorPair.Item2) * 0.5f - 0.5f;
}
