using UnityEngine;

public class ElevatorState : MonoBehaviour, IStateComponent
{
    [SerializeField]
    protected Elevator elevator;


    private void Awake()
    {
        InitElevatorComponent();
    }

    private void OnApplicationQuit()
    {
        ExitState();
    }


    public void EnterState()
    {
        this.gameObject.SetActive(true);
        OnEnterLogic();
    }

    public void ExitState()
    {
        OnExitLogic();
        this.gameObject.SetActive(false);
    }


    protected virtual void OnEnterLogic() { }
    protected virtual void OnExitLogic() { }


    private bool InitElevatorComponent()
    {
        elevator ??= GetComponentInParent<Elevator>() ??
            GetComponentInChildren<Elevator>();

        return elevator;
    }
}
