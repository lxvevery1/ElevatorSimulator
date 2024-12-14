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

    private void OnFloorDetect(Tuple<Tuple<float, float>, bool> floors)
    {
        print($"Elevator get floor id = <b>{floors.Item1.Item1}</b> and {floors.Item1.Item2} from sensor");
        switch (_driveDirection)
        {
            case ElevatorDriveDirection.UP:
                _currFloor = floors.Item1.Item2;
                break;
            case ElevatorDriveDirection.DOWN:
                _currFloor = floors.Item1.Item1;
                break;
            default:
                _currFloor = floors.Item1.Item2;
                break;
        }

        // Put this if you want strange initialization
        if (!_sensorsInited)
        {
            if (_initDriveDirection == ElevatorDriveDirection.DOWN &&
                    floors.Item2)
            {
                ReverseElevatorDirection();
            }
            else if (_initDriveDirection == ElevatorDriveDirection.UP &&
                    floors.Item2)
            {
                ReverseElevatorDirection();
            }
        }

        if (_sensorsInited && _driveDirection != ElevatorDriveDirection.STOP)
        {
            _driveDirection = ElevatorDriveDirection.STOP;
            print("Elevator stopped after reaching the init floor.");
            print("Sensors initialized");

            MoveToFloor(1);
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

        var targetDirection = floorId > _currFloor ?
            ElevatorDriveDirection.UP :
            ElevatorDriveDirection.DOWN;

        if (floorId == _currFloor)
        {
            targetDirection = ElevatorDriveDirection.STOP;
        }

        _driveDirection = targetDirection;
    }
}
