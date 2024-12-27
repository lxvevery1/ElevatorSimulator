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
    private const float _peopleWaitDuration = 5.0f;


    private void Awake()
    {
        _elevator.OnApproachFloorDetectAction += OnApproachFloorDetect;
        _elevator.OnGetTargetFloor += OnGetTargetFloor;
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
            _currState = ElevatorStateType.SearchFloorDownSlow;
        }
        if ((_currState == ElevatorStateType.SearchFloorDownSlow ||
                _currState == ElevatorStateType.SearchFloorUpSlow) &&
                _elevator.SensorsInited)
        {
            _currState = ElevatorStateType.Idle;
        }
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
