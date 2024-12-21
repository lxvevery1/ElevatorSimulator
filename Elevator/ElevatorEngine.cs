using UnityEngine;

/// <summary>
/// Elevator Engine, that regulates acceleration of Elevator
/// It takes info about sensors triggers from elevator and changes acceleration.
/// <summary>
public class ElevatorEngine : MonoBehaviour
{
    public float Speed = 0f;


    [SerializeField]
    private float _maxSpeed = 200f;
    [SerializeField]
    private float _minSpeed = 80f;
    private float _defaultSpeed => (_maxSpeed + _minSpeed) / 2;
    private float _accelerationTime = 1.3f;

    private ElevatorDriveDynamic _driveDynamic = ElevatorDriveDynamic.STABLE;


    public void SetMode(ElevatorDriveDynamic driveMode)
    {
        _driveDynamic = driveMode;
    }


    private void Update()
    {
        switch (_driveDynamic)
        {
            case ElevatorDriveDynamic.STABLE:
                // Keep the speed at the default value
                Speed = _defaultSpeed;
                break;

            case ElevatorDriveDynamic.ACCELERATION:
                // Gradually increase the speed to max speed
                if (Speed < _maxSpeed)
                {
                    Speed += (_maxSpeed - _defaultSpeed) * Time.deltaTime / _accelerationTime;
                    Speed = Mathf.Min(Speed, _maxSpeed); // Clamp speed to max speed
                }
                break;

            case ElevatorDriveDynamic.SLOWDOWN:
                // Gradually decrease the speed to min speed
                if (Speed > _minSpeed)
                {
                    Speed -= (_defaultSpeed - _minSpeed) * Time.deltaTime / _accelerationTime;
                    Speed = Mathf.Max(Speed, _minSpeed); // Clamp speed to min speed
                }
                break;
        }
    }
}
