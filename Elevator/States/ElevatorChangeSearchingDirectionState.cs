public class ElevatorChangeSearchingDirectionState : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        elevator.ReverseElevatorDirection();
    }

    protected override void OnExitLogic()
    {
        base.OnExitLogic();

        elevator.DriveDirection = ElevatorDriveDirection.STOP;
    }
}
