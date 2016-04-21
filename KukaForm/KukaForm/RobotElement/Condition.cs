using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukaForm
{

    public enum WhichControlToUse
    { PositionControl, SpeedControl, YawControl, HeightControl, MoveForward, Hover, Fall, Off }
    public enum WhichProcess { Nothing, Height, Pitch, Roll, Yaw, hover, FallDown }
    class Condition
    {
    }
}
