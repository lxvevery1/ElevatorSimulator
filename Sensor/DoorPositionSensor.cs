using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorPositionSensor : MonoBehaviour
{
    [SerializeField]
    public bool IsActive;
    private const string _doorTag = "Door";
    private Collider _collider;
    private const bool _isTrigger = true;
    private bool _isActive = true;
    private bool ActivationCondition(Collider other) => !other.isTrigger &&
        other.tag.Equals(_doorTag);

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = _isTrigger;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (ActivationCondition(other))
        {
            _isActive = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (ActivationCondition(other))
        {
            _isActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ActivationCondition(other))
        {
            _isActive = false;
        }
    }
}
