using TMPro;
using UnityEngine;

public class FloorTextSync : MonoBehaviour
{
    [SerializeField]
    private FloorSensor _sensor;
    private TextMeshPro _textMeshPro;

    private bool _isInited = false;


    private void Awake()
    {
        _sensor ??= GetComponentInParent<FloorSensor>();
        _textMeshPro ??= GetComponent<TextMeshPro>();
    }


    private void Update()
    {
        if (_textMeshPro && _sensor)
            _textMeshPro.text = _sensor.SensorDataFloor.floorId.ToString();
        else if (_isInited)
        {
            Debug.LogError("Bad init");
        }

    }
}
