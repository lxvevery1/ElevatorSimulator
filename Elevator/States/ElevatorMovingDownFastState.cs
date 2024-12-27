public class ElevatorMoveDownFastState : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        elevator.DriveDirection = ElevatorDriveDirection.DOWN;
        elevator.ElevatorEngine.Acceleration = ElevatorAcceleration.MAX;
    }

    protected override void OnExitLogic()
    {
        base.OnExitLogic();
    }
}
