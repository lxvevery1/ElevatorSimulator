using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ElevatorEngine))]
/// <summary>
/// Basic elevator, that can move vertically
/// Opens and closes its doors
/// Regulates speed with it's engine
/// <summary>
public class Elevator : MonoBehaviour
{
    public ElevatorDoors ElevatorDoors { get => _elevatorDoors; }
    public ElevatorDriveDirection DriveDirection = ElevatorDriveDirection.STOP;
    public ElevatorEngine ElevatorEngine { get => _elevatorEngine; }

    [SerializeField]
    private Rigidbody _rb;

    [SerializeField]
    protected ElevatorDoors _elevatorDoors;
    protected ElevatorEngine _elevatorEngine;

    [SerializeField]
    private float _smoothingTime = 0.5f; // Time to smoothly change direction

    private Vector3 _targetVelocity = Vector3.zero;
    private Vector3 _currentVelocity = Vector3.zero;

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

    protected virtual void FixedUpdate()
    {
        UpdateTargetVelocity();
        SmoothVelocity();
        MoveBody();
    }

    private void UpdateTargetVelocity()
    {
        _targetVelocity = _driveDirectionVector * _elevatorEngine.Speed;
    }

    private void SmoothVelocity()
    {
        // Smoothly interpolate between the current velocity and the target velocity
        _currentVelocity = Vector3.Lerp(_currentVelocity, _targetVelocity,
                Time.fixedDeltaTime / _smoothingTime);
    }

    protected void MoveBody()
    {
        if (!_rb)
            Init();

        _rb.linearVelocity = _currentVelocity;
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
}
