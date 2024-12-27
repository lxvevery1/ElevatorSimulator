public class ElevatorMovingUpFastState : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        elevator.DriveDirection = ElevatorDriveDirection.UP;
        elevator.ElevatorEngine.Acceleration = ElevatorAcceleration.MAX;
    }

    protected override void OnExitLogic()
    {
        base.OnExitLogic();
    }
}
