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
    private bool _doorsOpenedAfterGetTarget = false;
    private bool _obstacleAlarmed = false;
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

        if (_currState == ElevatorStateType.Idle &&
                _targetFloor == _currFloor &&
                !_doorsOpenedAfterGetTarget)
        {
            StartCoroutine(DoorOperationRoutine());
        }
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
        if ((!_isDoorState || _currState == ElevatorStateType.Idle) &&
                someFunc())
        {
            _targetFloor = i + 1;
            MoveToFloor(_targetFloor);
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


        var targetDirection = floorId > _currFloor ?
            ElevatorDriveDirection.UP :
            ElevatorDriveDirection.DOWN;


        if (_floorSensor.SensorDataApproach.floorId !=
                _currFloor)
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

    private IEnumerator DoorOperationRoutine()
    {
        _doorsOpenedAfterGetTarget = true;

        print($"<color=#F00000>Door operation routine...</color>");
        yield return new WaitForSeconds(1.0f);
        _currState = ElevatorStateType.DoorOpening;
        yield return new WaitForSeconds(2);
        _currState = ElevatorStateType.WaitingForPeople;
        yield return new WaitForSeconds(_peopleWaitDuration);
        _currState = ElevatorStateType.DoorClosing;
        yield return new WaitForSeconds(2);
        _currState = ElevatorStateType.Idle;

        _doorsOpenedAfterGetTarget = false;
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
