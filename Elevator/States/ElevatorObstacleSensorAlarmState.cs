public class ElevatorObstacleSensorAlarmState : ElevatorState
{
    protected override void OnEnterLogic()
    {
        base.OnEnterLogic();

        print("<color=#FF0000>Obstacle Alarm!!!</color>");
        elevator.ElevatorDoors.StopAllCoroutines();
        print("Alarm! Door Obstacle State!!!");
    }

    protected override void OnExitLogic()
    {
        base.OnExitLogic();
    }
}
