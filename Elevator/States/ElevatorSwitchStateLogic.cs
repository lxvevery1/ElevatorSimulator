using System;
using System.Collections;
using UnityEngine;

/// <summary> Returns Elevator's state at current moment </summary>
public class ElevatorSwitchStateLogic : MonoBehaviour
{
    public ElevatorStateType State { get => _currState; }


    [SerializeField]
    private FloorSensor _floorSensor;
    [SerializeField]
    private ObstacleSensorPack _obstacleSensors;
    [SerializeField]
    private DoorPositionSensorPack _doorSensors;
    private ElevatorStateType _currState = ElevatorStateType.Initial;
    [SerializeField]
    private ElevatorDriveDirection _initDriveDirection;
    private const float _peopleWaitDuration = 5.0f;
    private const float _obstacleAlarmDuration = 5.0f;
    private int _targetFloor = 0;
    private int _currFloor = 0;
    private bool _obstacleAlarmed = false;
    private bool _isApproaching = false;
    private bool _sensorsInited => _currFloor > 1;
    private bool _isMovingDown => _currState == ElevatorStateType.MovingDownSlow ||
        _currState == ElevatorStateType.MovingDownFast;
    private bool _isMovingUp => _currState == ElevatorStateType.MovingUpSlow ||
        _currState == ElevatorStateType.MovingUpFast;
    private bool _isMoving => _isMovingUp || _isMovingDown;
    private bool _isSearching => _currState == ElevatorStateType.SearchFloorUp ||
        _currState == ElevatorStateType.SearchFloorDown ||
        _currState == ElevatorStateType.SearchFloorDownSlow ||
        _currState == ElevatorStateType.SearchFloorUpSlow;


    private void Awake()
    {
        // _floorSensor.OnApproachFloorDetectAction += OnApproachFloorDetect;
        // _floorSensor.OnFloorDetectAction += OnFloorDetect;
        // _floorSensor.OnGetTargetFloor += OnGetTargetFloor;
        // _elevator.ElevatorDoors.OnGetObstacleAlarm += OnGetObstacleAlarm;
    }

    private void Start()
    {
        _currState = ElevatorStateType.Initial;
    }

    private void Update()
    {
        // Put this if you want strange initialization
        if (_floorSensor.SensorDataFloor.floorId > 1 &&
                _floorSensor.SensorDataFloor.isLimit)
        {
            if (_isSearching)
            {
                _currState = ElevatorStateType.ChangeSearchingDirection;
            }
        }
        // You found an floor! Congrats!
        if (_isSearching && _sensorsInited)
        {
            _currState = ElevatorStateType.Idle;
        }

        _currFloor = _floorSensor.SensorDataFloor.floorId;

        // we already at target floor
        if (_targetFloor == _currFloor && _isMoving)
        {
            _currState = ElevatorStateType.Idle;
        }
    }

    private void LateUpdate()
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


        var targetDirection = floorId > _currFloor ?
            ElevatorDriveDirection.UP :
            ElevatorDriveDirection.DOWN;


        if (_floorSensor.SensorDataApproach.floorId !=
                _currFloor)
        {
            print("<color=#00FF00>Approach == Floor sensor");
            _currState = (targetDirection == ElevatorDriveDirection.DOWN) ?
                ElevatorStateType.MovingDownFast :
                ElevatorStateType.MovingUpFast;
        }
        else
        {
            print("<color=#F00000>Approach != Floor sensor");
            _currState = (targetDirection == ElevatorDriveDirection.DOWN) ?
                ElevatorStateType.MovingDownSlow :
                ElevatorStateType.MovingUpSlow;
        }
        // we already here
        if (floorId == _currFloor)
        {
            targetDirection = ElevatorDriveDirection.STOP;
            _currState = ElevatorStateType.Idle;
        }

    }


    private void OnGetObstacleAlarm()
    {
        print("We got obstacle alarm!");
        if ((_obstacleSensors._sensorLeftDoor.ObstacleAlarm ||
                    _obstacleSensors._sensorRightDoor.ObstacleAlarm) &&
                !_obstacleAlarmed)
        {
            _obstacleAlarmed = true;
            StartCoroutine(ObstacleHandleCoroutine());
        }
        else
            print("Nvm, it's fine already!");
    }

    private void OnGetTargetFloor()
    {
        print("We got target floor!");
        if ((!_obstacleSensors._sensorLeftDoor.ObstacleAlarm
                    && !_obstacleSensors._sensorRightDoor.ObstacleAlarm))
            StartCoroutine(DoorOperationRoutine());
    }

    private IEnumerator ObstacleHandleCoroutine()
    {
        _currState = ElevatorStateType.ObstacleSensorAlarm;
        yield return new WaitForSeconds(0.1f); // DEBUG
        _currState = ElevatorStateType.DoorOpening;
        yield return new WaitUntil(() => _doorSensors._sensorOpenedLeft.IsActive &&
                _doorSensors._sensorOpenedLeft.IsActive);
        yield return new WaitForSeconds(_obstacleAlarmDuration);
        _currState = ElevatorStateType.DoorClosing;
        yield return new WaitUntil(() => _doorSensors._sensorClosedLeft.IsActive &&
                _doorSensors._sensorClosedRight.IsActive);
        _currState = ElevatorStateType.Idle;

        _obstacleAlarmed = false;
    }

    private void OnFloorDetect(FloorSensor.SensorData floorSensorData)
    {
        // Put this if you want strange initialization
        if (_floorSensor.SensorDataFloor.floorId > 1 && floorSensorData.isLimit)
        {
            if (_isSearching)
            {
                _currState = ElevatorStateType.ChangeSearchingDirection;
            }
        }

        if (_floorSensor.SensorDataFloor.floorId > 1 &&
                _currState != ElevatorStateType.Idle)
        {
            print("some floor reached");
            _currState = ElevatorStateType.Idle;
            _isApproaching = false;
            MoveToFloor(_targetFloor);
        }
    }

    private IEnumerator DoorOperationRoutine()
    {
        print($"<color=#F00000>Door operation routine...</color>");
        _currState = ElevatorStateType.DoorOpening;
        yield return new WaitForSeconds(2);
        _currState = ElevatorStateType.WaitingForPeople;
        yield return new WaitForSeconds(_peopleWaitDuration);
        _currState = ElevatorStateType.DoorClosing;
        yield return new WaitForSeconds(2);
        _currState = ElevatorStateType.Idle;
    }

    private void OnApproachFloorDetect(FloorSensor.SensorData sensorData)
    {
        // Elevator inited?
        if (sensorData.floorId == _targetFloor)
        {
            _isApproaching = true;
            switch (_currState)
            {
                case ElevatorStateType.MovingDownFast:
                    print("Moving down slow");
                    _currState = ElevatorStateType.MovingDownSlow;
                    break;
                case ElevatorStateType.MovingDownSlow:
                    print("Moving down slow");
                    _currState = ElevatorStateType.MovingDownSlow;
                    break;
                case ElevatorStateType.MovingUpFast:
                    print("Moving up slow");
                    _currState = ElevatorStateType.MovingUpSlow;
                    break;
                case ElevatorStateType.MovingUpSlow:
                    print("Moving down slow");
                    _currState = ElevatorStateType.MovingUpSlow;
                    break;
                default:
                    print("Unknown direction");
                    _currState = ElevatorStateType.Idle;
                    break;
            }
        }
        print($"OnApproachFloorDetect! -> {_currState.ToString()}");
    }

    [Serializable]
    private struct DoorPositionSensorPack
    {
        public DoorPositionSensor _sensorClosedLeft;
        public DoorPositionSensor _sensorOpenedLeft;
        public DoorPositionSensor _sensorClosedRight;
        public DoorPositionSensor _sensorOpenedRight;
    }

    [Serializable]
    private struct ObstacleSensorPack
    {
        public ObstacleSensor _sensorLeftDoor;
        public ObstacleSensor _sensorRightDoor;
    }
}
