using System;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class FloorSensor : MonoBehaviour
{
    public SensorData SensorDataFloor { get => _sensorDataFloor; }
    public SensorData SensorDataApproach { get => _sensorDataApproach; }

    public Action<SensorData> OnFloorDetectAction;
    public Action<SensorData> OnApproachFloorDetectAction;


    private bool _isActiveFloor = false;
    private bool _isActiveApproach = false;
    private bool _isLimitFloor = false;
    private bool _isLimitApproach = false;

    private string FLOOR_TAG = "Floor";
    private string APPROACH_TAG = "ApproachFloor";
    private int _floor = -1;
    private int _approachFloor = -1;
    private MeshCollider _meshCollider;

    private SensorData _sensorDataFloor =>
        new SensorData(_floor, _isActiveFloor, _isLimitFloor);
    private SensorData _sensorDataApproach =>
        new SensorData(_approachFloor, _isActiveApproach, _isLimitApproach);

    private void Awake()
    {
        _meshCollider ??= GetComponent<MeshCollider>();

        _meshCollider.convex = true;
        _meshCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag(FLOOR_TAG))
        {
            var floor = c.gameObject.GetComponent<Floor>();
            OnFloorDetectAction?.Invoke(_sensorDataFloor);
            _isActiveFloor = true;
            _floor = floor.FloorId;
            _isLimitFloor = floor.IsLimit;
            print($"{this.name} detect {c.gameObject.name}");
        }
        if (c.gameObject.CompareTag(APPROACH_TAG))
        {
            var floor = c.gameObject.GetComponent<Floor>();
            OnApproachFloorDetectAction?.Invoke(_sensorDataApproach);
            _isActiveApproach = true;
            _approachFloor = floor.FloorId;
            _isLimitApproach = floor.IsLimit;
            print($"{this.name} detect {c.gameObject.name}");
        }
    }

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

    public struct SensorData : IEquatable<SensorData>
    {
        public int floorId { get; }
        public bool isActive { get; }
        public bool isLimit { get; }

        public SensorData(int _floor, bool _isActive, bool _isLimit)
        {
            floorId = _floor;
            isActive = _isActive;
            isLimit = _isLimit;
        }

        // Implement IEquatable<SensorData> for type-safe comparison
        public bool Equals(SensorData other)
        {
            return floorId == other.floorId &&
                   isActive == other.isActive &&
                   isLimit == other.isLimit;
        }

        // Override Equals method for general object comparison
        public override bool Equals(object obj)
        {
            if (obj is SensorData other)
            {
                return Equals(other);
            }
            return false;
        }

        // Override GetHashCode to ensure consistency with Equals
        public override int GetHashCode()
        {
            return HashCode.Combine(floorId, isActive, isLimit);
        }

        // Override == and != operators for convenience
        public static bool operator ==(SensorData left, SensorData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SensorData left, SensorData right)
        {
            return !(left == right);
        }
    }
}
