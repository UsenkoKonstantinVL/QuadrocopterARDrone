using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    class CoordinatsOfObject
    {
        int obj;
        VRepController vrep;
        //float x, y, z;

        public CoordinatsOfObject(VRepController vr ,int _obj)
        {
            vrep = vr;
            obj = _obj;
        }

        public CoordinatsOfObject(VRepController vr, string _obj)
        {
            vrep = vr;
            obj = vr.ObjectHandle(_obj);
        }

        public PointXYZ getCoordinatOfObj()
        {
            float[] f = vrep.getObjectPosition(obj);
            return new PointXYZ(f[0], f[1], f[2]);
        }

        public PointXYZ getCoordinatRelativeObj(int _obj)
        {
            float[] f1 = vrep.getObjectPosition(obj);
            float[] f2 = vrep.getObjectPosition(_obj);

            return new PointXYZ((f2[0] - f1[0]), (f2[1] - f1[1]), (f2[2] - f1[2]));
        }

        public float[] getOrientatino(int relative)
        {
            return vrep.getObjectOrientation(obj, relative);
        }
    }

    class PointXYZ
    {
        float x, y, z;
        //float[] ar;


        public PointXYZ(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public PointXYZ(PointXYZ p)
        {
            x = p[0];
            y = p[1];
            z = p[2];
        }

        public float this[int i]
        {
            get
            {
                if (i == 0)
                    return x;
                else if (i == 1)
                    return y;
                else if (i == 2)
                    return z;
                else
                    return 0;
            }

            set
            {
                if (i == 0)
                     x = value;
                else if (i == 1)
                     y = value;
                else if (i == 2)
                     z = value;
                else
                    ;
            }
        }

    }
}
