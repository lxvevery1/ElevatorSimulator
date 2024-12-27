using System;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour, IStateController<ElevatorState>
{
    private List<ElevatorState> _elevatorStatesList = new List<ElevatorState>();
    private ElevatorState _currElevatorState;
    private ElevatorState _elevatorState => GetState(_stateDetector.State);
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
        SwitchState(_elevatorState);
    }

    private void Update()
    {
        SwitchState(_elevatorState);
    }

    public void SwitchState(ElevatorState state)
    {
        if (_currElevatorState == state)
            return;

        if (_currElevatorState != null)
            _currElevatorState.ExitState();

        _currElevatorState = state;

        print($"<color=#00FF00>SwitchState(<b>{state.gameObject.name.ToString()}</b> state</color>");
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
            _elevatorBehaviours.idleState,
            _elevatorBehaviours.SearchFloorUp,
            _elevatorBehaviours.SearchFloorDown,
            _elevatorBehaviours.ChangeSearchingDirection,
            /// ------ ///
            _elevatorBehaviours.MovingDownFast,
            _elevatorBehaviours.MovingDownSlow,
            _elevatorBehaviours.MovingUpFast,
            _elevatorBehaviours.MovingUpSlow,
            /// ------ ///
            _elevatorBehaviours.DoorClosing,
            _elevatorBehaviours.DoorOpening,
            _elevatorBehaviours.WaitingForPeople,
            _elevatorBehaviours.ObstacleSensorAlarm,
        });
    }

    private void InitStatePairs()
    {
        // Initialize the dictionary with state mappings
        _elevatorStatesDict = new Dictionary<ElevatorStateType, ElevatorState>
        {
            { ElevatorStateType.Initial, _elevatorBehaviours.initialState },
            { ElevatorStateType.Idle, _elevatorBehaviours.idleState },
            { ElevatorStateType.SearchFloorDownSlow,
                _elevatorBehaviours.SearchFloorDown },
            { ElevatorStateType.SearchFloorUpSlow,
                _elevatorBehaviours.SearchFloorUp },
            { ElevatorStateType.ChangeSearchingDirection,
                _elevatorBehaviours.ChangeSearchingDirection },
            /// ------ ///
            { ElevatorStateType.MovingDownFast,
                _elevatorBehaviours.MovingDownFast },
            { ElevatorStateType.MovingDownSlow,
                _elevatorBehaviours.MovingDownSlow },
            { ElevatorStateType.MovingUpFast,
                _elevatorBehaviours.MovingUpFast },
            { ElevatorStateType.MovingUpSlow,
                _elevatorBehaviours.MovingUpSlow },
            /// ------ ///
            { ElevatorStateType.DoorClosing,
                _elevatorBehaviours.DoorClosing },
            { ElevatorStateType.DoorOpening,
                _elevatorBehaviours.DoorOpening },
            { ElevatorStateType.WaitingForPeople,
                _elevatorBehaviours.WaitingForPeople },
            { ElevatorStateType.ObstacleSensorAlarm,
                _elevatorBehaviours.ObstacleSensorAlarm },
            // ...
        };
    }

    [Serializable]
    private struct ElevatorBehaviours
    {
        public ElevatorState initialState;
        public ElevatorState idleState;
        public ElevatorState SearchFloorDown;
        public ElevatorState SearchFloorUp;
        public ElevatorState ChangeSearchingDirection;
        /// ----- ///
        public ElevatorState MovingDownFast;
        public ElevatorState MovingDownSlow;
        public ElevatorState MovingUpFast;
        public ElevatorState MovingUpSlow;
        /// ----- ///
        public ElevatorState DoorClosing;
        public ElevatorState DoorOpening;
        public ElevatorState WaitingForPeople;
        public ElevatorState ObstacleSensorAlarm;
    }
}
