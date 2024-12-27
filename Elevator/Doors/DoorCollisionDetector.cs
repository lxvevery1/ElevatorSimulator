using UnityEngine;

public class DoorCollisionDetector : MonoBehaviour
{
    public ElevatorDoors ElevatorDoors;


    private void OnTriggerStay(Collider other)
    {
        // Notify the ElevatorDoors script if an obstacle is detected
        if (ElevatorDoors != null && !other.isTrigger)
        {
            // Only trigger the alarm if the doors are opening or closing
            if (ElevatorDoors.DoorState == ElevatorDoor.OPENING || ElevatorDoors.DoorState == ElevatorDoor.CLOSING)
            {
                ElevatorDoors.ObstacleAlarm = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear the obstacle alarm when the obstacle is no longer in the way
        if (ElevatorDoors != null && !other.isTrigger)
        {
            ElevatorDoors.ObstacleAlarm = false;
        }
    }
}
