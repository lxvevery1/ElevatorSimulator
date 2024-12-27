public class ElevatorDoorClosing : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        elevator.ElevatorDoors.DoClose();
    }
}
