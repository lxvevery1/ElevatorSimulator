public class ElevatorInitialState : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        elevator.InitSensors();
    }
}
