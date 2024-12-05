using System.Collections.Generic;

// CycleRequest:
//   type: object
//   description: Набор управляющих воздействий
//   properties:
//     MoveUpFast: {type: boolean}
//     MoveUpSlow: {type: boolean}
//     MoveDownFast: {type: boolean}
//     MoveDownSlow: {type: boolean}
//     DoClose: {type: boolean}
//     DoOpen: {type: boolean}
//   required: [MoveUpFast, MoveUpSlow, MoveDownFast, MoveDownSlow, DoClose, DoOpen]
//   additionalProperties: false
//   example:
//     MoveUpFast: false
//     MoveUpSlow: false
//     MoveDownFast: false
//     MoveDownSlow: false
//     DoClose: false
//     DoOpen: false
public struct CycleRequest
{
    public bool MoveUpFast;
    public bool MoveUpSlow;
    public bool MoveDownFast;
    public bool MoveDownSlow;
    public bool DoClose;
    public bool DoOpen;
}

// CycleResponse:
//   type: object
//   properties:
//     sensors:
//       type: object
//       properties:
//         WeightSensor: {type: integer, nullable: false }
//         DoorOpened: {type: boolean, nullable: false}
//         DoorClosed: {type: boolean, nullable: false}
//         ObstacleSensor: {type: boolean, nullable: false}
//         FloorSensor:
//           type: array
//           items:
//             type: boolean
//         ApproachSensor:
//           type: array
//           items:
//             type: boolean
//         TopLimit: {type: boolean, nullable: false}
//         BottomLimit: {type: boolean, nullable: false}
//       required: [WeightSensor, DoorOpened, DoorClosed, ObstacleSensor, FloorSensor, ApproachSensor, TopLimit, BottomLimit]
public struct CycleResponse
{
    public int WeightSensor;
    public bool DoorOpened;
    public bool DoorClosed;
    public bool ObstacleSensor;

    public List<bool> FloorSensors;
    public List<bool> ApproachSensors;

    public bool TopLimit;
    public bool BottomLimit;
}

//   emulation:
//     type: object
//     properties:
//       Position:
//         type: number
//         description: Высота кабины в метрах
//         nullable: false
//       DriveDirection:
//         type: string
//         description: Направление движения привода лебёдки
//         enum:
//           - STOP
//           - UP
//           - DOWN
//         nullable: false
//       DriveDynamic:
//         type: string
//         description: Динамика привода лебёдки
//         enum:
//           - STABLE
//           - ACCELERATION
//           - SLOWDOWN
//         nullable: false
//       Door:
//         type: string
//         description: Состояние дверей
//         enum:
//           - CLOSED
//           - CLOSING
//           - OPENED
//           - OPENING
//         nullable: false
//       Alarm:
//         description: Сообщение об ошибке
//         type: string
//         nullable: true
//     required: [Position, DriveDirection, DriveDynamic, Door, Alarm]
// required: [sensors, emulation]
public struct Emulation
{
    public float PositionY;
    public ElevatorDriveDirection DriveDirection;
    public ElevatorDriveDynamic DriveDynamic;
    public ElevatorDoor Door;
}
