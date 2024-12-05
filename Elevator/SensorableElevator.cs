using UnityEngine;

public class SensorableElevator : Elevator
{
    [SerializeField]
    private FloorCalculator _floorCalc;
    private float _currFloor;
    private bool _sensorsInited => _currFloor > 0;


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
            _driveDirection = ElevatorDriveDirection.UP;
        }
    }

    private void OnFloorDetect(float floorId)
    {
        print($"Elevator get floor id = <b>{floorId}</b> from sensor");
        _currFloor = floorId;

        if (_sensorsInited && _driveDirection != ElevatorDriveDirection.STOP)
        {
            _driveDirection = ElevatorDriveDirection.STOP;
            print("Elevator stopped after reaching the init floor.");
            print("Sensors initialized");

            MoveToFloor(3);
        }
    }

    protected void MoveToFloor(int floorId)
    {
        var targetDirection = floorId > _currFloor ? ElevatorDriveDirection.UP :
            ElevatorDriveDirection.DOWN;

        if (floorId == _currFloor)
        {
            targetDirection = ElevatorDriveDirection.STOP;
        }

        _driveDirection = targetDirection;
    }
}
