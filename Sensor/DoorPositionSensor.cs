using UnityEngine;

public class DoorPositionSensor : MonoBehaviour
{
    public bool IsActive => _isDoorDetected;


    [SerializeField]
    private Vector3 _rayDirection = Vector3.forward;
    [SerializeField]
    private float _rayLength = 5f;
    private string _doorTag = "Door"; // Tag to check for

    private bool _isDoorDetected = false;

    private void Update()
    {
        _isDoorDetected = CheckForDoor();
    }

    private bool CheckForDoor()
    {
        // Create a ray from the object's position in the specified direction
        Ray ray = new Ray(transform.position,
                transform.TransformDirection(_rayDirection));

        // Perform the raycast
        if (Physics.Raycast(ray, out RaycastHit hit, _rayLength))
        {
            if (hit.collider.CompareTag(_doorTag))
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        // Draw the ray in the Scene view for debugging
        Gizmos.color = _isDoorDetected ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position,
                transform.TransformDirection(_rayDirection) * _rayLength);
    }
}
