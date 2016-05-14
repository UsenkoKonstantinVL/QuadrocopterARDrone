using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukaForm
{

    public enum WhichControlToUse
    { PositionControl, SpeedControl, YawControl, HeightControl, MoveForward, Hover, Fall, Off, PitchRoll, Nothing}
    public enum WhichProcess { Nothing, Height, Pitch, Roll, Yaw, Hover, FallDown, PitchRoll }
    public enum FollowingLine { SearchLine, GetLineToCenter, Rotation, MovementOnLine}
    public enum DetectedZone { Center, Right, Left, Top, TopLeft, TopRight, Bottom, Zone1, Zone2, Zone3, Zone4}
    class Condition
    {
    }
}
