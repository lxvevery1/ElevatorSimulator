using UnityEngine;

/// <summary> Returns Elevator's state at current moment </summary>
public class ElevatorSwitchStateLogic : MonoBehaviour
{
    public ElevatorStateType State
    {
        get => _currState;
    }


    [SerializeField]
    private SensorableElevator _elevator;
    private ElevatorStateType _currState = ElevatorStateType.Initial;


    private void Awake()
    {
        _elevator.OnApproachFloorDetectAction += OnApproachFloorDetect;
    }

    private void Start()
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


    public ElevatorStateType DetectStateLogic()
    {
        return ElevatorStateType.Idle;
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
        // Keep searching floor for initialization
        else
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
        print($"OnApproachFloorDetect! {_elevator.DriveDirection} ->" +
                $" {_currState}");
    }
}
