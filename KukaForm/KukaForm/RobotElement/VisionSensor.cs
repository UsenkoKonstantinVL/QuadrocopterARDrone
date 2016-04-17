using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    class VisionSensor
    {
        VRepController vrep;
        int obj;
        string name;

        public VisionSensor(VRepController vr, string _name)
        {
            //name = _name;
            vrep = vr;
            obj = vr.ObjectHandle(_name);
            name = _name;
        }

        public Bitmap getImageFromVisionSensor()
        {
            return vrep.GetVisionImage(obj);
        }




    }
}
