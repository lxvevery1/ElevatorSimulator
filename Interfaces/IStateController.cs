public interface IStateController<T>
{
    /// <summary>
    /// Exit from previous state -> Enter to new state ->
    /// Setup new state as current
    /// </summary>
    public void SwitchState(T state);
    public void DisableState(T state);
    public void DisableAllStates();
}
