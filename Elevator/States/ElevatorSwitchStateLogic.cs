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
    private bool _handleObstacleAlarming = false;
    private bool _isApproaching => _targetFloor ==
        _floorSensor.SensorDataApproach.floorId;
    private bool _sensorsInited => _currFloor > 0;
    private bool _isMovingDownState => _currState == ElevatorStateType.MovingDownSlow ||
        _currState == ElevatorStateType.MovingDownFast;
    private bool _isMovingUpState => _currState == ElevatorStateType.MovingUpSlow ||
        _currState == ElevatorStateType.MovingUpFast;
    private bool _isMovingState => _isMovingUpState || _isMovingDownState;
    private bool _isSearchingState => _currState == ElevatorStateType.SearchFloorUp ||
        _currState == ElevatorStateType.SearchFloorDown ||
        _currState == ElevatorStateType.SearchFloorDownSlow ||
        _currState == ElevatorStateType.SearchFloorUpSlow;
    private bool _isDoorOpened => _doorSensors._sensorOpenedLeft.IsActive &&
        _doorSensors._sensorOpenedRight.IsActive;
    private bool _isDoorClosed => _doorSensors._sensorClosedLeft.IsActive &&
        _doorSensors._sensorClosedRight.IsActive;
    private bool _isDoorState => _currState == ElevatorStateType.DoorOpening ||
        _currState == ElevatorStateType.DoorClosing ||
        _currState == ElevatorStateType.WaitingForPeople;
    private bool _isObstacleDetected => _obstacleSensors._sensorLeftDoor.ObstacleAlarm
        || _obstacleSensors._sensorRightDoor.ObstacleAlarm;


    private void Start()
    {
        _currState = ElevatorStateType.Initial;
    }

    private void Update()
    {
        _currFloor = _floorSensor.SensorDataFloor.floorId;
        // Put this if you want strange initialization
        ChangeDirectionOnInitLogic();
        // You found an floor! Congrats!
        SearchingEndLogic();
        // we already at target floor
        TargetFloorLogic();
        // target floor is near
        ApproachingLogic();

        HandleObstacle();
    }

    private void LateUpdate()
    {
        // Init floor search
        StartSearch(() => Input.GetKeyDown(KeyCode.Space));

        // Check for number key presses (0-9) and
        // Move to target floor
        for (int i = 0; i <= 9; i++)
        {
            StartMoveToFloor(() => Input.GetKey(KeyCode.Alpha0 + i), i);
        }
    }

    private void StartSearch(Func<bool> someFunc)
    {
        if (_currState == ElevatorStateType.Initial &&
                someFunc())
        {
            if (_initDriveDirection == ElevatorDriveDirection.DOWN)
                _currState = ElevatorStateType.SearchFloorDownSlow;
            else if (_initDriveDirection == ElevatorDriveDirection.UP)
                _currState = ElevatorStateType.SearchFloorUpSlow;
        }
    }

    private void StartMoveToFloor(Func<bool> someFunc, int i)
    {
        if (someFunc())
        {
            if (!_isDoorState || _currState == ElevatorStateType.Idle)
            {
                _targetFloor = i + 1;
                MoveToFloor(_targetFloor);
            }
        }
    }

    /// <summary>
    /// Move elevator to target floor
    /// <param name="floorId"> id of floor: 1, 2, 3, 4, ...
    /// </summary>
    private void MoveToFloor(int floorId)
    {
        print($"<color=#FFF000>Move to floor {floorId}...</color>");
        if (floorId <= 0)
            return;


        ElevatorDriveDirection targetDirection;

        if (floorId > _currFloor)
            targetDirection = ElevatorDriveDirection.UP;
        else if (floorId < _currFloor)
            targetDirection = ElevatorDriveDirection.DOWN;
        else
            targetDirection = ElevatorDriveDirection.STOP;


        if (_floorSensor.SensorDataApproach.floorId !=
                _currFloor)
        {
            if (targetDirection == ElevatorDriveDirection.DOWN)
                _currState = ElevatorStateType.MovingDownFast;
            else if (targetDirection == ElevatorDriveDirection.UP)
                _currState = ElevatorStateType.MovingUpFast;
            else
                _currState = ElevatorStateType.Idle;
        }
        else
        {
            if (targetDirection == ElevatorDriveDirection.DOWN)
                _currState = ElevatorStateType.MovingDownSlow;
            else if (targetDirection == ElevatorDriveDirection.UP)
                _currState = ElevatorStateType.MovingUpSlow;
            else
                _currState = ElevatorStateType.Idle;
        }

        // we already here
        if (floorId == _currFloor)
        {
            targetDirection = ElevatorDriveDirection.STOP;
            StartCoroutine(DoorOperationRoutine());
        }

    }

    private void HandleObstacle()
    {
        if (_isObstacleDetected && _isDoorState && !_handleObstacleAlarming)
        {
            StartCoroutine(ObstacleHandleCoroutine());
        }
    }

    private void OnGetTargetFloor()
    {
        print("We got target floor!");
        if ((!_obstacleSensors._sensorLeftDoor.ObstacleAlarm
                    && !_obstacleSensors._sensorRightDoor.ObstacleAlarm))
            StartCoroutine(DoorOperationRoutine());
    }


    private void ChangeDirectionOnInitLogic()
    {
        if (_floorSensor.SensorDataFloor.floorId > 1 &&
                _floorSensor.SensorDataFloor.isLimit)
        {
            if (_isSearchingState)
            {
                _currState = ElevatorStateType.ChangeSearchingDirection;
            }
        }
    }

    private void SearchingEndLogic()
    {
        if (_isSearchingState && _sensorsInited)
        {
            _currState = ElevatorStateType.Idle;
        }
    }

    private void TargetFloorLogic()
    {
        if (_targetFloor == _currFloor && _isMovingState)
        {
            _currState = ElevatorStateType.Idle;
            StartCoroutine(DoorOperationRoutine());
        }
    }

    private void ApproachingLogic()
    {
        if (_isApproaching)
        {
            if (_isMovingDownState)
            {
                _currState = ElevatorStateType.MovingDownSlow;
            }
            else if (_isMovingUpState)
            {
                _currState = ElevatorStateType.MovingUpSlow;
            }
        }
        else
        {
            if (_isMovingDownState)
            {
                _currState = ElevatorStateType.MovingDownFast;
            }
            else if (_isMovingUpState)
            {
                _currState = ElevatorStateType.MovingUpFast;
            }
        }
    }

    private IEnumerator ObstacleHandleCoroutine()
    {
        _handleObstacleAlarming = true;

        _currState = ElevatorStateType.ObstacleSensorAlarm;
        yield return new WaitForSeconds(0.1f); // To apply prev state
        _currState = ElevatorStateType.DoorOpening;
        yield return new WaitUntil(() => _isDoorOpened);
        yield return new WaitUntil(() => !_isObstacleDetected);
        _handleObstacleAlarming = false;
        _currState = ElevatorStateType.DoorClosing;
        yield return new WaitUntil(() => _isDoorClosed);
        _currState = ElevatorStateType.Idle;

    }

    private IEnumerator DoorOperationRoutine()
    {
        print($"<color=#F00000>Door operation routine...</color>");
        yield return new WaitForSeconds(1.0f);
        _currState = ElevatorStateType.DoorOpening;
        yield return new WaitUntil(() => _isDoorOpened);
        _currState = ElevatorStateType.WaitingForPeople;
        yield return new WaitForSeconds(_peopleWaitDuration);
        _currState = ElevatorStateType.DoorClosing;
        yield return new WaitUntil(() => _isDoorClosed);
        _currState = ElevatorStateType.Idle;
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
