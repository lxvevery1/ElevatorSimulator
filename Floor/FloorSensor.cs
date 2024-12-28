using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class FloorSensor : MonoBehaviour
{
    public SensorData SensorDataFloor { get => _sensorDataFloor; }
    public SensorData SensorDataApproach { get => _sensorDataApproach; }

    private const string FLOOR_TAG = "Floor";
    private const string APPROACH_TAG = "ApproachFloor";
    private int _floor = -1;
    private int _approachFloor = -1;
    private MeshCollider _meshCollider;

    private bool _isActiveFloor = false;
    private bool _isActiveApproach = false;
    private bool _isLimitFloor = false;
    private bool _isLimitApproach = false;
    private SensorData _sensorDataFloor => new SensorData(_floor, _isActiveFloor, _isLimitFloor);
    private SensorData _sensorDataApproach => new SensorData(_approachFloor, _isActiveApproach, _isLimitApproach);


    private void Awake()
    {
        _meshCollider ??= GetComponent<MeshCollider>();

        _meshCollider.convex = true;
        _meshCollider.isTrigger = true;
    }

    // How it was going
    // private void OnTriggerEnter(Collider c)
    // {
    //     if (c.gameObject.CompareTag(FLOOR_TAG))
    //     {
    //         var floor = c.gameObject.GetComponent<Floor>();
    //         OnFloorDetectAction?.Invoke(floor);
    //         print($"{this.name} detect {c.gameObject.name}");
    //     }
    //     if (c.gameObject.CompareTag(APPROACH_TAG))
    //     {
    //         var floor = c.gameObject.GetComponent<Floor>();
    //         OnApproachFloorDetectAction?.Invoke(floor);
    //         print($"{this.name} detect {c.gameObject.name}");
    //     }
    // }

    // How it's going
    // If sensor starts collision with trigger
    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag(FLOOR_TAG))
        {
            var floor = c.gameObject.GetComponent<Floor>();
            _isActiveFloor = true;
            _floor = floor.FloorId;
            _isLimitFloor = floor.IsLimit;
            print($"{this.name} detect {c.gameObject.name}");
        }
        if (c.gameObject.CompareTag(APPROACH_TAG))
        {
            var floor = c.gameObject.GetComponent<Floor>();
            _isActiveApproach = true;
            _approachFloor = floor.FloorId;
            _isLimitApproach = floor.IsLimit;
            print($"{this.name} detect {c.gameObject.name}");
        }
    }

    // If sensor stays in collision with trigger
    private void OnTriggerStay(Collider c)
    {
        if (c.gameObject.CompareTag(FLOOR_TAG))
        {
            var floor = c.gameObject.GetComponent<Floor>();
            _isActiveFloor = true;
            _floor = floor.FloorId;
            _isLimitFloor = floor.IsLimit;
            print($"{this.name} detect {c.gameObject.name}");
        }
        if (c.gameObject.CompareTag(APPROACH_TAG))
        {
            var floor = c.gameObject.GetComponent<Floor>();
            _isActiveApproach = true;
            _approachFloor = floor.FloorId;
            _isLimitApproach = floor.IsLimit;
            print($"{this.name} detect {c.gameObject.name}");
        }
    }

    // If sensor exit collision with trigger
    private void OnTriggerExit(Collider c)
    {
        if (c.gameObject.CompareTag(FLOOR_TAG))
        {
            var floor = c.gameObject.GetComponent<Floor>();
            _isActiveFloor = false;
            _floor = floor.FloorId;
            _isLimitFloor = floor.IsLimit;
            print($"{this.name} exit {c.gameObject.name}");
        }
        if (c.gameObject.CompareTag(APPROACH_TAG))
        {
            var floor = c.gameObject.GetComponent<Floor>();
            _isActiveApproach = false;
            _approachFloor = floor.FloorId;
            _isLimitApproach = floor.IsLimit;
            print($"{this.name} exit {c.gameObject.name}");
        }
    }

    public struct SensorData
    {
        public int floor;
        public bool isActive;
        public bool isLimit;

        public SensorData(int _floor, bool _isActive, bool _isLimit)
        {
            floor = _floor;
            isActive = _isActive;
            isLimit = _isLimit;
        }
    }
}
