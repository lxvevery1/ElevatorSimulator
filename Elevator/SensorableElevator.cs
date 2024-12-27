using System;
using UnityEngine;

public class SensorableElevator : Elevator
{
    public Action<float> OnApproachFloorDetectAction;
    public Action<Tuple<Tuple<float, float>, bool>> OnFloorDetectAction;
    public Action OnGetTargetFloor;
    public bool SensorsInited => _sensorsInited;
    public bool IsApproaching
    {
        get => _isApproaching;
        set => _isApproaching = value;
    }
    public float Floor
    {
        get => _currFloor;
        set => _currFloor = value;
    }


    [SerializeField]
    private FloorCalculator _floorCalc;
    private float _currFloor;
    private bool _sensorsInited => _currFloor > 0;
    [SerializeField]
    private bool _isApproaching = false;


    protected override bool Init()
    {
        _floorCalc.OnFloorDetectAction += OnFloorDetect;
        _floorCalc.OnApproachFloorDetectAction += OnApproachFloorDetect;
        return base.Init();
    }

    private void OnApproachFloorDetect(float approachFloor)
    {
        if (approachFloor < 1)
            return;

        OnApproachFloorDetectAction?.Invoke(approachFloor);
    }

    private void OnFloorDetect(Tuple<Tuple<float, float>, bool> floors)
    {
        OnFloorDetectAction?.Invoke(floors);
        print($"Elevator get floor id = <b>{floors.Item1.Item1}</b> and {floors.Item1.Item2} from sensor");
        // Now it's in the SwitchStateLogic class
    }

}
