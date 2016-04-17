using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class Drive
    {
        VRepController vrep;
        int obj;
        string name;
        string nameOfSignal;

        public Drive(VRepController vr, string _name, string signalName)
        {
            name = _name;
            vrep = vr;
            obj = vrep.ObjectHandle(name);
            nameOfSignal = signalName;
        }

        public Drive(VRepController vr, string _name)
        {
            name = _name;
            vrep = vr;
            obj = vrep.ObjectHandle(name);
            //nameOfSignal = signalName;
        }

        public void setVelocityToForceDrive(float value)
        {
            vrep.setFloatSignal(nameOfSignal, value);
        }

        public void setVelocityToDrive(float value)
        {
            vrep.SetVelocityToObject(obj, value);
        }

        public void setForceToDrive(string nSignal, int value)
        {
           // vrep.setFloatSignal(obj, value);
        }
    }
}
