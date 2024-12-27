public class ElevatorIdleState : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        elevator.DriveDirection = ElevatorDriveDirection.STOP;
    }
}
