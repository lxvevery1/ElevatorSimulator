public class ElevatorDoorOpening : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        elevator.ElevatorDoors.DoOpen();
    }

    protected override void OnExitLogic()
    {
        base.OnExitLogic();
    }
}
