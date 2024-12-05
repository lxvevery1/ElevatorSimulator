using UnityEditor;
using UnityEngine;

/// <summary> Basic elevator, that can move vertically <summary>
public class Elevator : MonoBehaviour
{
    private float _position => gameObject.transform.position.y;
    private float _speed = 40f;
    private Rigidbody _rb;

    private static ElevatorDriveDirection _driveDirection = ElevatorDriveDirection.DOWN;
    private static Vector3 _driveDirectionVector => DirEnumToVector(_driveDirection);

    private static Vector3 DirEnumToVector(ElevatorDriveDirection dirEnum) =>
        dirEnum switch
        {
            ElevatorDriveDirection.UP => Vector3.up,
            ElevatorDriveDirection.DOWN => Vector3.down,
            ElevatorDriveDirection.STOP => Vector3.zero,
            _ => Vector3.zero
        };


    private bool Init()
    {
        if (!_rb)
            _rb = GetComponent<Rigidbody>() ?? GetComponentInChildren<Rigidbody>();

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
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _driveDirection = ElevatorDriveDirection.DOWN;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _driveDirection = ElevatorDriveDirection.STOP;
        }
    }

    private void FixedUpdate()
    {
        MoveBody(_driveDirection);
    }

    private void MoveBody(ElevatorDriveDirection dir)
    {
        if (!_rb)
            Init();

        _rb.linearVelocity = DirEnumToVector(dir) * _speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider c)
    {
        print($"Elevator {this.name} on the {c.gameObject.name}");
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    private static void DrawDirectionVectorGizmo(Elevator elevator, GizmoType gizmoType)
    {
        Gizmos.DrawLine(elevator.transform.position, _driveDirectionVector);
    }
}
