public enum ElevatorState
{
    // To initialize elevator
    // Set the current cabin floor, read it from sensor
    // if sensor don't get it
    // -> SEARCHFLOORUP
    Initial,

    // Enters when elevator don't detect it's own floor
    // it can enter the next state: MOVING DOWN SLOW
    // TOP LIMIT IS TRUE APPROACHING SENSOR IS FALSE
    SearchFloorDown,

    // Enters when elevator need to get approaching sensor value,
    // it can enter the next state: MOVING UP SLOW ->
    // TOP LIMIT IS FALSE APPROACHING SENSOR IS FALSE
    SearchFloorUp,

    // Enters when elevator don't detect it's own floor
    // it can enter the next state: MOVING DOWN SLOW -> STOP SEARCHING
    SearchFloorDownSlow,

    // Enters when elevator get approaching sensor,
    // it can enter the next state: MOVING UP SLOW -> STOP SEARCHING
    // APPROACHING SENSOR IS TRUE - elevator know where he is located
    SearchFloorUpSlow,

    // When sensor says you move to the limit (-1 floor or > max) ->
    ChangeSearchingDirection,

    // When elevator reads it floor from sensor
    // -> IDLE
    StopSearching,

    // Do nothing, wait for some action
    Idle,

    // On button pressed or idle->timeout
    DoorClosing,

    // Enter DOOR OPENING state
    ObstacleSensorAlarm,

    // On button pressed or Obstacle sensor trigger
    // -> DOOR CLOSE
    DoorOpening,

    // Wait for people to enter elevator i think
    WaitingForPeople,

    // Close doors before making move
    ClosingBeforeMove,

    // Before making some action, open or close door
    // reset signal of moving with some delay
    StopWaiting,

    // Elevator won't stop at next floor
    MovingDownFast,

    // Elevator will stop at next floor
    MovingDownSlow,

    // Elevator moving UP, won't stop at next floor
    MovingUpFast,

    // Elevator moving UP, will stop at next floor
    MovingUpSlow,
}

public enum ElevatorDriveDirection
{
    STOP,
    UP,
    DOWN
}

public enum ElevatorDriveDynamic
{
    STABLE,
    ACCELERATION,
    SLOWDOWN
}

public enum ElevatorAcceleration
{
    ZERO,
    MAX,
    MIN
}

public enum ElevatorDoor
{
    CLOSED,
    CLOSING,
    OPENED,
    OPENING
}

public enum ErrorResponse
{
    BAD_SESSION,
    ENTRY_NOT_POSSIBLE,
    EXIT_NOT_POSSIBLE,
    NO_PASSENGERS
}
