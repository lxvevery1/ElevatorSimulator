using System.Collections.Generic;
using UnityEngine;

public class SensorableElevator : Elevator
{
    [SerializeField]
    private List<FloorSensor> _floorSensors;
}
