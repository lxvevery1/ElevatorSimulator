using UnityEngine;

public class FloorSensor : MonoBehaviour
{
    private int _floor;
    private MeshCollider _meshCollider;


    private void Awake()
    {
        _meshCollider ??= GetComponent<MeshCollider>();

        _meshCollider.convex = true;
        _meshCollider.isTrigger = true;
    }
}
