using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ElevatorDoors : MonoBehaviour, IElevatorDoors
{
    public float AnimationDuration { get => _animationDuration; }
    public Action OnGetObstacleAlarm;
    private Tuple<DoorPositionSensor, DoorPositionSensor> _doorPosSensors;

    private const int DOOR_COUNT = 2;
    [SerializeField]
    private List<GameObject> _doorsGO = new(DOOR_COUNT); // List of door GameObjects
    private float _openedXOffset = 0.65f;
    private float _targetX;
    public float _animationDuration = 2.0f;
    private bool _isOpened = false;

    private List<ObstacleSensor> _doorCollisionDetectors = new(DOOR_COUNT);
    private bool _doorObstacleAlarm =>
        _doorCollisionDetectors.Any(sensor => sensor.ObstacleAlarm);


    public void DoOpen()
    {
        _targetX = _openedXOffset;
        _isOpened = true;
        ChangePosForEachDoor();
    }

    public void DoClose()
    {
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
            elapsedTime += Time.deltaTime;
            float blend = Mathf.Clamp01(elapsedTime / _animationDuration);

            door.transform.localPosition = new Vector3(
                    Mathf.Lerp(oldPos.x, targetX, blend),
                    oldPos.y,
                    oldPos.z);

            yield return null;
        }
    }
}
