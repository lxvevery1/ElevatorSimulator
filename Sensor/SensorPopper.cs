using UnityEngine;

public class SensorPopper : MonoBehaviour
{
    [SerializeField]
    private Transform _objToTrack;


    private void Awake()
    {
        this.transform.SetParent(null);
    }

    private void Update()
    {
        this.transform.SetPositionAndRotation(_objToTrack.position,
                _objToTrack.rotation);
    }
}
