using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ElevatorEngine))]
/// <summary>
/// Basic elevator, that can move vertically
/// Opens and closes its doors
/// <summary>
public class Elevator : MonoBehaviour
{
    public ElevatorDoors ElevatorDoors { get => _elevatorDoors; }
    public ElevatorDriveDirection DriveDirection = ElevatorDriveDirection.STOP;
    public ElevatorEngine ElevatorEngine { get => _elevatorEngine; }


    private float _position => gameObject.transform.position.y;
    [SerializeField]
    private Rigidbody _rb;

    [SerializeField]
    protected ElevatorDoors _elevatorDoors;
    protected ElevatorEngine _elevatorEngine;
    private Vector3 _driveDirectionVector => DirEnumToVector(DriveDirection);

    private static Vector3 DirEnumToVector(ElevatorDriveDirection dirEnum) =>
        dirEnum switch
        {
            ElevatorDriveDirection.UP => Vector3.up,
            ElevatorDriveDirection.DOWN => Vector3.down,
            ElevatorDriveDirection.STOP => Vector3.zero,
            _ => Vector3.zero
        };


    protected virtual bool Init()
    {
        bool inited = false;

        _rb ??= GetComponent<Rigidbody>();
        _elevatorEngine ??= GetComponent<ElevatorEngine>();


        inited = _elevatorEngine && _rb;
        return inited;
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     _driveDirection = ElevatorDriveDirection.UP;
        //     print(_driveDirection);
        // }
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     _driveDirection = ElevatorDriveDirection.DOWN;
        //     print(_driveDirection);
        // }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     _driveDirection = ElevatorDriveDirection.STOP;
        //     print(_driveDirection);
        //
        //     switch (_elevatorDoors.DoorState)
        //     {
        //         case ElevatorDoor.OPENED:
        //             _elevatorDoors.DoClose();
        //             break;
        //
        //         case ElevatorDoor.CLOSED:
        //             _elevatorDoors.DoOpen();
        //             break;
        //
        //         default:
        //             // do nothing
        //             break;
        //     }
        // }
    }

    protected virtual void FixedUpdate()
    {
        MoveBody(DriveDirection);
    }

    protected void MoveBody(ElevatorDriveDirection dir)
    {
        if (!_rb)
            Init();

        _rb.linearVelocity = DirEnumToVector(dir) * _elevatorEngine.Speed * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Changes elevator drive direction from up to down and from down to up
    /// </summary>
    public void ReverseElevatorDirection()
    {
        if (DriveDirection == ElevatorDriveDirection.STOP)
            return;

        DriveDirection = (DriveDirection == ElevatorDriveDirection.UP) ?
            ElevatorDriveDirection.DOWN :
            ElevatorDriveDirection.UP;
    }


    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    private void DrawDirectionVectorGizmo(Elevator elevator, GizmoType gizmoType)
    {
        Gizmos.DrawLine(elevator.transform.position, _driveDirectionVector);
    }
}
