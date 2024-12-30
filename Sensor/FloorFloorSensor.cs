using UnityEngine;

/// <summary> Detects elevator presence on the floor </summary>
public class FloorFloorSensor : MonoBehaviour
{
    public bool IsActive { get => _isActive; }

    [SerializeField]
    private Collider _collider;
    private const bool _isTrigger = true;
    private const string _elevatorTag = "Elevator";
    private bool _isActive = false;


    private void Awake()
    {
        _collider ??= GetComponent<Collider>();
        _collider.isTrigger = _isTrigger;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(_elevatorTag))
        {
            _isActive = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals(_elevatorTag))
        {
            _isActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(_elevatorTag))
        {
            _isActive = false;
        }
    }

}
