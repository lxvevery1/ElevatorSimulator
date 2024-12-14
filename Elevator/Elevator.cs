using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
/// <summary> Basic elevator, that can move vertically <summary>
public class Elevator : MonoBehaviour
{
    private float _position => gameObject.transform.position.y;
    [SerializeField]
    private float _speed = 40f;
    private Rigidbody _rb;

    protected static ElevatorDriveDirection _driveDirection = ElevatorDriveDirection.STOP;
    private static Vector3 _driveDirectionVector => DirEnumToVector(_driveDirection);

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
        _rb ??= GetComponent<Rigidbody>() ?? GetComponentInChildren<Rigidbody>();

        return _rb;
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _driveDirection = ElevatorDriveDirection.UP;
            print(_driveDirection);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _driveDirection = ElevatorDriveDirection.DOWN;
            print(_driveDirection);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _driveDirection = ElevatorDriveDirection.STOP;
            print(_driveDirection);
        }
    }

    protected virtual void FixedUpdate()
    {
        MoveBody(_driveDirection);
    }

    protected void MoveBody(ElevatorDriveDirection dir)
    {
        if (!_rb)
            Init();

        _rb.linearVelocity = DirEnumToVector(dir) * _speed * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Changes elevator drive direction from up to down and from down to up
    /// </summary>
    protected void ReverseElevatorDirection()
    {
        if (_driveDirection == ElevatorDriveDirection.STOP)
            return;

        _driveDirection = (_driveDirection == ElevatorDriveDirection.UP) ?
            ElevatorDriveDirection.DOWN :
            ElevatorDriveDirection.UP;
    }


    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    private static void DrawDirectionVectorGizmo(Elevator elevator, GizmoType gizmoType)
    {
        Gizmos.DrawLine(elevator.transform.position, _driveDirectionVector);
    }

}
