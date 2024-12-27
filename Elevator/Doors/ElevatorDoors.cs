using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ElevatorDoors : MonoBehaviour, IElevatorDoors
{
    public float AnimationDuration { get => _animationDuration; }
    public Action OnGetObstacleAlarm;
    public ElevatorDoor DoorState;
    public bool IsOpened => _isOpened;
    public bool ObstacleAlarm
    {
        get => _obstacleAlarm;
        set
        {
            if (value != _obstacleAlarm)
            {
                _obstacleAlarm = value;
                if (_obstacleAlarm)
                {
                    OnGetObstacleAlarm?.Invoke();
                }
            }
        }
    }

    private const int DOOR_COUNT = 2;
    [SerializeField]
    private List<GameObject> _doorsGO = new(DOOR_COUNT); // List of door GameObjects
    private float _openedXOffset = 0.65f;
    private float _targetX;
    public float _animationDuration = 2.0f;
    private bool _isOpened = false;
    private bool _obstacleAlarm = false;

    private List<DoorCollisionDetector> _doorCollisionDetectors = new(DOOR_COUNT);


    private void Start()
    {
        // Initialize the DoorCollisionDetector scripts
        foreach (var door in _doorsGO)
        {
            if (door != null)
            {
                var detector = door.GetComponentInChildren<DoorCollisionDetector>();
                if (detector == null)
                {
                    detector = door.GetComponentInChildren<DoorCollisionDetector>();
                }
                detector.ElevatorDoors = this;
                _doorCollisionDetectors.Add(detector);
            }
        }
    }

    public void DoOpen()
    {
        if (DoorState == ElevatorDoor.OPENED || _obstacleAlarm)
            return;

        DoorState = ElevatorDoor.OPENING;
        _targetX = _openedXOffset;
        _isOpened = true;
        ChangePosForEachDoor();
    }

    public void DoClose()
    {
        if (DoorState == ElevatorDoor.CLOSED || _obstacleAlarm)
            return;

        DoorState = ElevatorDoor.CLOSING;
        _targetX = 0;
        _isOpened = false;
        ChangePosForEachDoor();
    }

    private void ChangePosForEachDoor()
    {
        StartCoroutine(ChangeHorizontalPosition(_doorsGO[0], -_targetX));
        StartCoroutine(ChangeHorizontalPosition(_doorsGO[1], _targetX));
    }

    private IEnumerator ChangeHorizontalPosition(GameObject door, float targetX)
    {
        float elapsedTime = 0;
        var oldPos = door.transform.localPosition;

        while (elapsedTime < _animationDuration)
        {
            if (_obstacleAlarm)
                yield break; // Exit coroutine if an obstacle is detected

            elapsedTime += Time.deltaTime;
            float blend = Mathf.Clamp01(elapsedTime / _animationDuration);

            door.transform.localPosition = new Vector3(
                    Mathf.Lerp(oldPos.x, targetX, blend),
                    oldPos.y,
                    oldPos.z);

            yield return null;
        }

        DoorState = targetX == _openedXOffset ? ElevatorDoor.OPENED : ElevatorDoor.CLOSED;
        print($"Coroutine for {door.name} ended, door {DoorState.ToString()}");
    }
}
