using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorDoors : MonoBehaviour, IElevatorDoors
{
    private const int DOOR_COUNT = 2;
    [SerializeField]
    private List<GameObject> _doorsGO = new(DOOR_COUNT);
    private float _openedXOffset = 0.65f;
    private float _targetX;
    private float _animationDuration = 2.0f;


    public void DoOpen()
    {
        _targetX = _openedXOffset;
        StartCoroutine(HorizontalPositionChange(_doorsGO[0], -_targetX));
        StartCoroutine(HorizontalPositionChange(_doorsGO[1], _targetX));
    }
    public void DoClose()
    {
        _targetX = 0;
        StartCoroutine(HorizontalPositionChange(_doorsGO[0], -_targetX));
        StartCoroutine(HorizontalPositionChange(_doorsGO[1], _targetX));
    }


    private IEnumerator HorizontalPositionChange(GameObject door, float targetX)
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

        print($"Coroutine for {door.name} ended");
    }
}
