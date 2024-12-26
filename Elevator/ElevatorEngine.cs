using UnityEngine;

/// <summary>
/// Elevator Engine, that regulates acceleration of Elevator
/// It takes info about sensors triggers from elevator and changes acceleration.
/// <summary>
public class ElevatorEngine : MonoBehaviour
{
    public float Speed = 0f;
    public ElevatorAcceleration Acceleration
    {
        get => _acceleration;
        set => _acceleration = value;
    }

    [SerializeField]
    private float _acceleratedSpeed = 200f;
    [SerializeField]
    private float _approachingSpeed = 20f;
    private float _maxAccelerationTime = 2.5f;
    private float _minAccelerationTime = 2f;

    private ElevatorDriveDynamic _driveDynamic = ElevatorDriveDynamic.STABLE;
    private ElevatorAcceleration _acceleration = ElevatorAcceleration.ZERO;


    private void Update()
    {
        SpeedRegulation();
    }

    private void SpeedRegulation()
    {
        switch (_acceleration)
        {
            case ElevatorAcceleration.ZERO:
                _driveDynamic = ElevatorDriveDynamic.STABLE;
                Speed = _approachingSpeed;
                break;

            case ElevatorAcceleration.MAX:
                if (Speed < _acceleratedSpeed)
                {
                    _driveDynamic = ElevatorDriveDynamic.ACCELERATION;
                    Speed += (_acceleratedSpeed - _approachingSpeed) *
                        Time.deltaTime / _maxAccelerationTime;
                }
                else
                {
                    _driveDynamic = ElevatorDriveDynamic.STABLE;
                    Speed = Mathf.Min(Speed, _acceleratedSpeed); // Clamp speed to max speed
                }
                break;

            case ElevatorAcceleration.MIN:
                if (Speed > _approachingSpeed)
                {
                    _driveDynamic = ElevatorDriveDynamic.SLOWDOWN;
                    Speed -= (_acceleratedSpeed - _approachingSpeed) *
                        Time.deltaTime / (_minAccelerationTime);
                }
                else
                {
                    _driveDynamic = ElevatorDriveDynamic.STABLE;
                    Speed = Mathf.Max(Speed, _approachingSpeed); // Clamp speed to min speed
                }
                break;
        }
    }
}
