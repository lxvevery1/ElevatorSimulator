using System;
using UnityEngine;

public class SensorableElevator : Elevator
{
    public Action<float> OnApproachFloorDetectAction;
    public Action<Tuple<Tuple<float, float>, bool>> OnFloorDetectAction;
    public Action OnGetTargetFloor;
    public Action OnTargetFloorGetAction;
    public bool SensorsInited => _sensorsInited;


    [SerializeField]
    private FloorCalculator _floorCalc;
    private float _currFloor;
    private bool _sensorsInited => _currFloor > 0;
    [SerializeField]
    private ElevatorDriveDirection _initDriveDirection;
    [SerializeField]
    private bool _isApproaching = false;
    private int _targetFloor = 1;


    protected override bool Init()
    {
        _floorCalc.OnFloorDetectAction += OnFloorDetect;
        _floorCalc.OnApproachFloorDetectAction += OnApproachFloorDetect;
        return base.Init();
    }

    private void OnApproachFloorDetect(float approachFloor)
    {
        OnApproachFloorDetectAction?.Invoke(approachFloor);
        if (approachFloor < 1)
            return;

        if (approachFloor == _targetFloor)
        {
            _isApproaching = true;
            _elevatorEngine.Acceleration = ElevatorAcceleration.MIN;
        }
    }

    private void OnFloorDetect(Tuple<Tuple<float, float>, bool> floors)
    {
        OnFloorDetectAction?.Invoke(floors);
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
        // Now it's in the SwitchStateLogic class
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
            OnGetTargetFloor?.Invoke();
            // _elevatorDoors.DoOpen();
        }

        DriveDirection = targetDirection;
    }
}
