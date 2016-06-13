using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukaForm
{

    public enum WhichControlToUse
    { PositionControl, SpeedControl, YawControl, HeightControl, MoveForward, Hover, Fall, Off, PitchRoll, CoordinateControl, Nothing}
    public enum WhichProcess { Nothing, Height, Pitch, Roll, Yaw, Hover, FallDown, PitchRoll, Coordinates }
    public enum FollowingLine { SearchLine, GetLineToCenter, Rotation, MovementOnLine}
    public enum DetectedZone { Center, Right, Left, Top, TopLeft, TopRight, Bottom, BottomLeft, BottomRight, Zone1, Zone2, Zone3, Zone4, All}
    class Condition
    {
    }
}
