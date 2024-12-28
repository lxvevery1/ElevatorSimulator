using System;
using System.Collections;
using UnityEngine;

/// <summary> Returns Elevator's state at current moment </summary>
public class ElevatorSwitchStateLogic : MonoBehaviour
{
    public ElevatorStateType State { get => _currState; }


    [SerializeField]
    private SensorableElevator _elevator;
    private ElevatorStateType _currState = ElevatorStateType.Initial;
    private ElevatorDoors _elevatorDoors => _elevator.ElevatorDoors;
    [SerializeField]
    private ElevatorDriveDirection _initDriveDirection;
    private const float _peopleWaitDuration = 5.0f;
    private const float _obstacleAlarmDuration = 5.0f;
    private int _targetFloor = 0;
    private bool _obstacleAlarmed = false;


    private void Awake()
    {
        _elevator.OnApproachFloorDetectAction += OnApproachFloorDetect;
        _elevator.OnGetTargetFloor += OnGetTargetFloor;
        _elevator.OnFloorDetectAction += OnFloorDetect;
        _elevator.ElevatorDoors.OnGetObstacleAlarm += OnGetObstacleAlarm;
    }

    private void Start()
    {
        _currState = ElevatorStateType.Initial;
    }

    private void Update()
    {
        if (_currState == ElevatorStateType.Initial &&
                Input.GetKeyDown(KeyCode.Space))
        {
            if (_initDriveDirection == ElevatorDriveDirection.DOWN)
                _currState = ElevatorStateType.SearchFloorDownSlow;
            else if (_initDriveDirection == ElevatorDriveDirection.UP)
                _currState = ElevatorStateType.SearchFloorUpSlow;
        }

        // Check for number key presses (0-9)
        for (int i = 0; i <= 9; i++)
        {
            if (_currState == ElevatorStateType.Idle &&
                    Input.GetKey(KeyCode.Alpha0 + i))
            {
                _targetFloor = i + 1;
                MoveToFloor(_targetFloor);
                break; // Exit the loop after handling the key press
            }
        }
    }

    /// <summary>
    /// Move elevator to target floor
    /// <param name="floorId"> id of floor: 1, 2, 3, 4, ...
    /// </summary>
    protected void MoveToFloor(int floorId)
    {
        print($"<color=#FFF000>Move to floor {floorId}...</color>");
        if (floorId <= 0)
            return;


        var targetDirection = floorId > _elevator.Floor ?
            ElevatorDriveDirection.UP :
            ElevatorDriveDirection.DOWN;

        if (!_elevator.IsApproaching)
        {
            _currState = (targetDirection == ElevatorDriveDirection.DOWN) ?
                ElevatorStateType.MovingDownFast :
                ElevatorStateType.MovingUpFast;
        }
        else
        {
            _currState = (targetDirection == ElevatorDriveDirection.DOWN) ?
                ElevatorStateType.MovingDownSlow :
                ElevatorStateType.MovingUpSlow;
        }

        // we already here
        if (floorId == _elevator.Floor)
        {
            targetDirection = ElevatorDriveDirection.STOP;
            _currState = ElevatorStateType.Idle;
            _elevator.OnGetTargetFloor?.Invoke();
        }

        _elevator.DriveDirection = targetDirection;
    }


    private void OnGetObstacleAlarm()
    {
        print("We got obstacle alarm!");
        if (_elevator.ElevatorDoors.ObstacleAlarm)
        {
            _obstacleAlarmed = true;
            StartCoroutine(ObstacleHandleCoroutine());
        }
        else
            print("Nah, it's fine already!");
    }

    private void OnGetTargetFloor()
    {
        print("We got target floor!");
        if (!_elevator.ElevatorDoors.ObstacleAlarm)
            StartCoroutine(DoorOperationRoutine());
    }

    private IEnumerator ObstacleHandleCoroutine()
    {
        if (_elevator.ElevatorDoors.ObstacleAlarm && _obstacleAlarmed)
        {
            _currState = ElevatorStateType.ObstacleSensorAlarm;
            yield return new WaitForSeconds(_obstacleAlarmDuration);
            _currState = ElevatorStateType.DoorOpening;
            yield return new WaitForSeconds(_elevator.ElevatorDoors.AnimationDuration);
            _currState = ElevatorStateType.DoorClosing;
            yield return new WaitForSeconds(_elevator.ElevatorDoors.AnimationDuration);
            _currState = ElevatorStateType.Idle;

            _obstacleAlarmed = false;
        }
    }

    private void OnFloorDetect(Tuple<Tuple<float, float>, bool> floors)
    {
        switch (_elevator.DriveDirection)
        {
            case ElevatorDriveDirection.UP:
                _elevator.Floor = floors.Item1.Item2;
                break;
            case ElevatorDriveDirection.DOWN:
                _elevator.Floor = floors.Item1.Item1;
                break;
            default:
                _elevator.Floor = floors.Item1.Item2;
                break;
        }
        // Put this if you want strange initialization
        if (!_elevator.SensorsInited)
        {
            if ((_currState == ElevatorStateType.SearchFloorDownSlow ||
                    _currState == ElevatorStateType.SearchFloorDown) &&
                    floors.Item2)
            {
                _currState = ElevatorStateType.ChangeSearchingDirection;
            }
            else if ((_currState == ElevatorStateType.SearchFloorUpSlow ||
                    _currState == ElevatorStateType.SearchFloorUp) &&
                    floors.Item2)
            {
                _currState = ElevatorStateType.ChangeSearchingDirection;
            }
        }

        if (_elevator.SensorsInited && _elevator.DriveDirection !=
                ElevatorDriveDirection.STOP)
        {
            print("some floor reached");
            _currState = ElevatorStateType.Idle;
            MoveToFloor(_targetFloor);
        }
    }

    private IEnumerator DoorOperationRoutine()
    {
        print($"<color=#F00000>Door operation routine...</color>");
        _currState = ElevatorStateType.DoorOpening;
        yield return new WaitForSeconds(_elevator.ElevatorDoors.AnimationDuration);
        _currState = ElevatorStateType.WaitingForPeople;
        yield return new WaitForSeconds(_peopleWaitDuration);
        _currState = ElevatorStateType.DoorClosing;
        yield return new WaitForSeconds(_elevator.ElevatorDoors.AnimationDuration);
        _currState = ElevatorStateType.Idle;
    }

    private void OnApproachFloorDetect(float approachFloor)
    {
        // Elevator inited?
        if (approachFloor == _targetFloor)
        {
            _elevator.IsApproaching = true;
            _elevator.ElevatorEngine.Acceleration = ElevatorAcceleration.MIN;
            switch (_elevator.DriveDirection)
            {
                case ElevatorDriveDirection.UP:
                    print("Moving up slow");
                    _currState = ElevatorStateType.MovingUpSlow;
                    break;
                case ElevatorDriveDirection.DOWN:
                    print("Moving down slow");
                    _currState = ElevatorStateType.MovingDownSlow;
                    break;
                default:
                    print("Unknown direction");
                    _currState = ElevatorStateType.Idle;
                    break;
            }
        }
        print($"OnApproachFloorDetect! {_elevator.DriveDirection} ->" +
                $" {_currState}");
    }
}
