using System;
using UnityEngine;

public class SensorableElevator : Elevator
{
    [SerializeField]
    private FloorCalculator _floorCalc;
    private float _currFloor;
    private bool _sensorsInited => _currFloor > 0;
    [SerializeField]
    private ElevatorDriveDirection _initDriveDirection;


    protected override bool Init()
    {
        _floorCalc.OnFloorDetectAction += OnFloorDetect;
        InitSensors();
        return base.Init();
    }


    private void InitSensors()
    {
        if (!_sensorsInited)
        {
            print("Starting initialization for sensors");
            _driveDirection = _initDriveDirection;
        }
    }

    private void OnFloorDetect(Tuple<float, float> floor)
    {
        print($"Elevator get floor id = <b>{floor.Item1}</b> from sensor");
        switch (_driveDirection)
        {
            case ElevatorDriveDirection.UP:
                _currFloor = floor.Item1;
                break;
            case ElevatorDriveDirection.DOWN:
                _currFloor = floor.Item2;
                break;
            default:
                _currFloor = floor.Item1;
                break;
        }

        if (_sensorsInited && _driveDirection != ElevatorDriveDirection.STOP)
        {
            _driveDirection = ElevatorDriveDirection.STOP;
            print("Elevator stopped after reaching the init floor.");
            print("Sensors initialized");

            MoveToFloor(3);
        }
    }

    /// <summary>
    /// Move elevator to target floor
    /// <param name="floorId"> id of floor: 1, 2, 3, 4, ...
    /// </summary>
    protected void MoveToFloor(int floorId)
    {
        if (floorId <= 0)
        {
            return;
        }

        var targetDirection = floorId > _currFloor ? ElevatorDriveDirection.UP :
            ElevatorDriveDirection.DOWN;

        if (floorId == _currFloor)
        {
            targetDirection = ElevatorDriveDirection.STOP;
        }

        _driveDirection = targetDirection;
    }
}
