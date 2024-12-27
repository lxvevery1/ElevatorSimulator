public class ElevatorSearchFloorDownState : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        elevator.DriveDirection = ElevatorDriveDirection.DOWN;
        elevator.ElevatorEngine.Acceleration = ElevatorAcceleration.MIN;
    }

    protected override void OnExitLogic()
    {
        base.OnExitLogic();

        elevator.DriveDirection = ElevatorDriveDirection.STOP;
        elevator.ElevatorEngine.Acceleration = ElevatorAcceleration.ZERO;
    }
}
