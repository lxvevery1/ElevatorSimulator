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
    [SerializeField]
    private int _targetFloor = 1;
    private Action<float> _onApproachFloorDetectAction;
    private bool _isApproaching = false;


    protected override bool Init()
    {
        _floorCalc.OnFloorDetectAction += OnFloorDetect;
        _floorCalc.OnApproachFloorDetectAction += OnApproachFloorDetect;
        InitSensors();
        return base.Init();
    }

    private void OnApproachFloorDetect(float approachFloor)
    {
        if (approachFloor < 1)
            return;

        if (approachFloor == _targetFloor)
        {
            _isApproaching = true;
            _elevatorEngine.Acceleration = ElevatorAcceleration.MIN;
        }
    }

    private void InitSensors()
    {
        if (!_sensorsInited)
        {
            print("Starting initialization for sensors");
            DriveDirection = _initDriveDirection;
        }
    }

    private void OnFloorDetect(Tuple<Tuple<float, float>, bool> floors)
    {
        print($"Elevator get floor id = <b>{floors.Item1.Item1}</b> and {floors.Item1.Item2} from sensor");
        switch (DriveDirection)
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

        if (_sensorsInited && DriveDirection != ElevatorDriveDirection.STOP)
        {
            DriveDirection = ElevatorDriveDirection.STOP;
            print("Elevator stopped after reaching the init floor.");
            print("Sensors initialized");

            MoveToFloor(_targetFloor);
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

        if (!_isApproaching)
        {
            _elevatorEngine.Acceleration = ElevatorAcceleration.MAX;
        }

        if (floorId == _currFloor)
        {
            targetDirection = ElevatorDriveDirection.STOP;
            _elevatorEngine.Acceleration = ElevatorAcceleration.ZERO;
            _isApproaching = false;
            _elevatorDoors.DoOpen();
        }

        DriveDirection = targetDirection;
    }
}
