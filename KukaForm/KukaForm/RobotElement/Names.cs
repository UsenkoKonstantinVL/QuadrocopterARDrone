using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukaForm
{
    class Names
    {
        public static string robPosX = "PositionX";
        public static string robPosY = "PositionY";
        public static string robPosZ = "PositionZ";

        public static string targPosX = "TargetX";
        public static string targPosY = "TargetY";
        public static string targPosZ = "TargetZ";
        
        public static string leftBoard = "Left";
        public static string rightBoard = "Right";
        public static string forthBoard = "Centr";

        public static string laserPosX = "LasX";
        public static string laserPosY = "LasY";
        public static string laserPosZ = "LasZ";


        public static string rollJoint1 = "rollingJoint_fl";
        public static string rollJoint2 = "rollingJoint_rl";
        public static string rollJoint3 = "rollingJoint_rr";
        public static string rollJoint4 = "rollingJoint_fr";

        public static string hokuoObjName = "Hokuyo_URG_04LX_UG01";
    }

    class Point3D
    {
        int x, y, z;

        float fx, fy, fz;

        Point3D(int xx = 0, int yy = 0, int zz = 0)
        {
            x = xx;
            y = yy;
            z = zz;
        }


        Point3D(float xx = 0, float yy = 0, float zz = 0)
        {
            fx = xx;
            fy = yy;
            fz = zz;
        }

        public int X
        {
            get { return x; }
        }

        public float fX
        {
            get { return fx; }
        }

        public int Y
        {
            get { return y; }
        }

        public float fY
        {
            get { return fy; }
        }

        public int Z
        {
            get { return z; }
        }

        public float fZ
        {
            get { return fz; }
        }
    }
}
