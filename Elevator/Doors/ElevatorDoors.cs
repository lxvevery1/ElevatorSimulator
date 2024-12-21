using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorDoors : MonoBehaviour, IElevatorDoors
{
    public ElevatorDoor DoorState;
    public bool IsOpened => _isOpened;

    private const int DOOR_COUNT = 2;
    [SerializeField]
    private List<GameObject> _doorsGO = new(DOOR_COUNT);
    private float _openedXOffset = 0.65f;
    private float _targetX;
    private float _animationDuration = 2.0f;
    private bool _isOpened = false;



    public void DoOpen()
    {
        DoorState = ElevatorDoor.OPENING;
        _targetX = _openedXOffset;
        ChangePosForEachDoor();
    }
    public void DoClose()
    {
        DoorState = ElevatorDoor.CLOSING;
        _targetX = 0;
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

        DoorState = targetX == _openedXOffset ? ElevatorDoor.OPENED : ElevatorDoor.CLOSED;
        print($"Coroutine for {door.name} ended, door {DoorState.ToString()}");
    }
}
