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
    private bool _isApproaching => _targetFloor ==
        _floorSensor.SensorDataApproach.floorId;
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


    private void Start()
    {
        _currState = ElevatorStateType.Initial;
    }

    private void Update()
    {
        _currFloor = _floorSensor.SensorDataFloor.floorId;
        // Put this if you want strange initialization
        StrangeInitLogic();
        // You found an floor! Congrats!
        SearchingEndLogic();
        // we already at target floor
        TargetFloorLogic();
        // target floor is near
        ApproachingLogic();
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
        if (_currState == ElevatorStateType.Idle &&
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

    private void StrangeInitLogic()
    {
        if (_floorSensor.SensorDataFloor.floorId > 1 &&
                _floorSensor.SensorDataFloor.isLimit)
        {
            if (_isSearching)
            {
                _currState = ElevatorStateType.ChangeSearchingDirection;
            }
        }
    }
    private void SearchingEndLogic()
    {
        if (_isSearching && _sensorsInited)
        {
            _currState = ElevatorStateType.Idle;
        }
    }

    private void TargetFloorLogic()
    {
        if (_targetFloor == _currFloor && _isMoving)
        {
            _currState = ElevatorStateType.Idle;
        }
    }

    private void ApproachingLogic()
    {
        if (_isApproaching)
        {
            if (_isMovingDown)
            {
                _currState = ElevatorStateType.MovingDownSlow;
            }
            else if (_isMovingUp)
            {
                _currState = ElevatorStateType.MovingUpSlow;
            }
        }
        else
        {
            if (_isMovingDown)
            {
                _currState = ElevatorStateType.MovingDownFast;
            }
            else if (_isMovingUp)
            {
                _currState = ElevatorStateType.MovingUpFast;
            }
        }
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
