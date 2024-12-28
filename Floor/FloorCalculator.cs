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
    }

    // YEAH! Now I'm checking sensors state every frame!
    private void Update()
    {
        foreach (var sensor in _floorSensors)
        {
            if (sensor.SensorDataFloor.isActive)
            {
                OnFloorDetect(sensor.SensorDataFloor);
            }
            if (sensor.SensorDataApproach.isActive)
            {
                OnApproachFloorDetect(sensor.SensorDataApproach);
            }
        }
    }

    private void OnApproachFloorDetect(FloorSensor.SensorData sensorData)
    {
        OnApproachFloorDetectAction.Invoke(sensorData.floor);
        print($"{this.name} approaching floor -> {sensorData.floor}");
    }

    private void OnFloorDetect(FloorSensor.SensorData sensorData)
    {
        if (sensorData.floor > 0 && _lastFloorDetected > 0)
        {
            Tuple<float, float> topBottomFloors = new Tuple<float, float>
                (sensorData.floor, _lastFloorDetected);
            _calculatedFloorUP = DetectCurrentFloorUP(topBottomFloors);
            _calculatedFloorDOWN = DetectCurrentFloorDOWN(topBottomFloors);
        }

        _lastFloorDetected = sensorData.floor;

        OnFloorDetectAction?.Invoke(
                new Tuple<Tuple<float, float>, bool>
                (new Tuple<float, float>(_calculatedFloorDOWN, _calculatedFloorUP),
                 sensorData.isLimit));
        print($"{this.name} current floor -> {sensorData.floor}");
    }

    private float DetectCurrentFloorUP(Tuple<float, float> floorPair) =>
        (floorPair.Item1 + floorPair.Item2) * 0.5f;
    private float DetectCurrentFloorDOWN(Tuple<float, float> floorPair) =>
        (floorPair.Item1 + floorPair.Item2) * 0.5f - 0.5f;
}
