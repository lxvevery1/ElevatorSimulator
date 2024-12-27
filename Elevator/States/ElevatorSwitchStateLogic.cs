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


    private void Awake()
    {
        _elevator.OnApproachFloorDetectAction += OnApproachFloorDetect;
        _elevator.OnGetTargetFloor += OnGetTargetFloor;
        _elevator.OnFloorDetectAction += OnFloorDetect;
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
        if ((_currState == ElevatorStateType.SearchFloorDownSlow ||
                _currState == ElevatorStateType.SearchFloorUpSlow) &&
                _elevator.SensorsInited)
        {
            _currState = ElevatorStateType.Idle;
        }

        // if (_currState == ElevatorStateType.Idle && )
    }

    private void SearchFloor()
    {
        // Keep searching floor for initialization
        if (!_elevator.SensorsInited)
        {
            switch (_elevator.DriveDirection)
            {
                case ElevatorDriveDirection.UP:
                    _currState = ElevatorStateType.SearchFloorUpSlow;
                    break;
                case ElevatorDriveDirection.DOWN:
                    _currState = ElevatorStateType.SearchFloorDownSlow;
                    break;
                default:
                    _currState = ElevatorStateType.Initial;
                    break;
            }
        }
    }

    private void OnGetTargetFloor()
    {
        StartCoroutine(DoorOperationRoutine());
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
            if (_initDriveDirection == ElevatorDriveDirection.DOWN &&
                    floors.Item2)
            {
                _currState = ElevatorStateType.ChangeSearchingDirection;
            }
            else if (_initDriveDirection == ElevatorDriveDirection.UP &&
                    floors.Item2)
            {
                _currState = ElevatorStateType.ChangeSearchingDirection;
            }
        }

        if (_elevator.SensorsInited && _elevator.DriveDirection !=
                ElevatorDriveDirection.STOP)
        {
            // initialization completed
            _currState = ElevatorStateType.Idle;
        }
    }

    private IEnumerator DoorOperationRoutine()
    {
        _currState = ElevatorStateType.DoorOpening;
        yield return new WaitForSeconds(_elevator.ElevatorDoors.AnimationDuration);
        _currState = ElevatorStateType.WaitingForPeople;
        yield return new WaitForSeconds(_elevator.ElevatorDoors.AnimationDuration);
        _currState = ElevatorStateType.DoorClosing;
        yield return new WaitForSeconds(_elevator.ElevatorDoors.AnimationDuration);
        _currState = ElevatorStateType.Idle;
    }

    private void OnApproachFloorDetect(float approachFloor)
    {
        // Elevator inited?
        if (_elevator.SensorsInited)
        {
            switch (_elevator.DriveDirection)
            {
                case ElevatorDriveDirection.UP:
                    _currState = ElevatorStateType.MovingUpSlow;
                    break;
                case ElevatorDriveDirection.DOWN:
                    _currState = ElevatorStateType.MovingDownSlow;
                    break;
                default:
                    _currState = ElevatorStateType.Idle;
                    break;
            }
        }
        print($"OnApproachFloorDetect! {_elevator.DriveDirection} ->" +
                $" {_currState}");
    }
}
