using TMPro;
using UnityEngine;

public class FloorTextSync : MonoBehaviour
{
    private SensorableElevator _elevator;
    private TextMeshPro _textMeshPro;


    private void Awake()
    {
        _elevator ??= GetComponentInParent<SensorableElevator>();
        _textMeshPro ??= GetComponent<TextMeshPro>();
    }


    private void Update()
    {
        _textMeshPro.text = _elevator.Floor.ToString();
    }
}
