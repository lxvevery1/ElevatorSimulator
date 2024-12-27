public class ElevatorInitialState : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        print("Init me!");
    }
}
