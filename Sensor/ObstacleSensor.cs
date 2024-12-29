using UnityEngine;

public class ObstacleSensor : MonoBehaviour
{
    public bool ObstacleAlarm
    {
        get => _obstacleAlarm;
    }


    private bool _obstacleAlarm;
    private const string _obstacleTag = "Obstacle";



    private void OnTriggerEnter(Collider other)
    {
        // Notify the ElevatorDoors script if an obstacle is detected
        if (!other.isTrigger && other.tag.Equals(_obstacleTag))
        {
            print("<color=#FF0000>Obstacle Alarm!<color>");
            _obstacleAlarm = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Notify the ElevatorDoors script if an obstacle is detected
        if (!other.isTrigger && other.tag.Equals(_obstacleTag))
        {
            _obstacleAlarm = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear the obstacle alarm when the obstacle is no longer in the way
        if (!other.isTrigger && other.tag.Equals(_obstacleTag))
        {
            _obstacleAlarm = false;
        }
    }
}
