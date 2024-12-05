using System;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class FloorSensor : MonoBehaviour
{
    public Action<int> OnFloorDetectAction;


    private string FLOOR_TAG = "Floor";
    private int _floor;
    private MeshCollider _meshCollider;

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
            OnFloorDetectAction?.Invoke(floor.FloorId);
            print($"{this.name} detect {c.gameObject.name}");
        }
    }
}
