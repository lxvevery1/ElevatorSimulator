using System;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour, IStateController<ElevatorState>
{
    private List<ElevatorState> _elevatorStatesList = new List<ElevatorState>();
    private ElevatorState _currElevatorState;
    [SerializeField]
    private ElevatorBehaviours _elevatorBehaviours;
    private ElevatorSwitchStateLogic _stateDetector;
    private Dictionary<ElevatorStateType, ElevatorState> _elevatorStatesDict;


    private void Awake()
    {
        _stateDetector = GetComponentInChildren<ElevatorSwitchStateLogic>() ??
            GetComponentInParent<ElevatorSwitchStateLogic>();
        InitStatesList();
        InitStatePairs();
        DisableAllStates();
        SwitchState(GetState(_stateDetector.State));
    }

    private void Update()
    {
        SwitchState(_currElevatorState);
    }

    public void SwitchState(ElevatorState state)
    {
        if (_currElevatorState == state)
            return;

        if (_currElevatorState != null)
            _currElevatorState.ExitState();

        _currElevatorState = state;

        Debug.Log($"SwitchState({typeof(ElevatorState).ToString()} state)");
        _currElevatorState.EnterState();
    }

    public void DisableAllStates()
    {
        foreach (ElevatorState elevatorState in _elevatorStatesList)
        {
            DisableState(elevatorState);
        }
    }

    public void DisableState(ElevatorState state)
    {
        if (state != null)
        {
            state.ExitState();
        }
    }

    private ElevatorState GetState(ElevatorStateType statesType)
    {
        _elevatorStatesDict.TryGetValue(statesType, out var state);
        return state;
    }


    private void InitStatesList()
    {
        _elevatorStatesList.AddRange(new[] {
            _elevatorBehaviours.initialState,
            _elevatorBehaviours.idleState
        });
    }

    private void InitStatePairs()
    {
        // Initialize the dictionary with state mappings
        _elevatorStatesDict = new Dictionary<ElevatorStateType, ElevatorState>
        {
            { ElevatorStateType.Initial, _elevatorBehaviours.initialState },
            { ElevatorStateType.Idle, _elevatorBehaviours.idleState },
            // ...
        };
    }

    [Serializable]
    private struct ElevatorBehaviours
    {
        public ElevatorState initialState;
        public ElevatorState idleState;
    }
}
