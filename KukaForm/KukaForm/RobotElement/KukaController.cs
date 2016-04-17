using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using remoteApiNETWrapper;

namespace KukaForm
{
    class KukaController
    {
        int idClient;
        int[] wheelJoints;
        int laser;
        

        public KukaController()
        {
            idClient = -1;
        }


        public void Connect()
        {
            int idPort = 7777;
            wheelJoints = new int[4];
            idClient = VREPWrapper.simwStart("127.0.0.1", idPort);

            VREPWrapper.simwGetObjectHandle(idClient, "rollingJoint_fl", out wheelJoints[0]);
            VREPWrapper.simwGetObjectHandle(idClient, "rollingJoint_rl", out wheelJoints[1]);
            VREPWrapper.simwGetObjectHandle(idClient, "rollingJoint_rr", out wheelJoints[2]);
            VREPWrapper.simwGetObjectHandle(idClient, "rollingJoint_fr", out wheelJoints[3]);

        }

        public string getSignal(string s)
        {
            if (IsConnected())
                return VREPWrapper.simwGetStringSignal(idClient, s);
            else
                return null;
        }

        public float getFloat(string s)
        {
            if (IsConnected())
                return VREPWrapper.simwGetFloatSignal(idClient, s);
            else
                return -1f;
        }

        public void Finish()
        {
            VREPWrapper.simwFinish(idClient);
        }


        public bool IsConnected()
        {
            if (idClient == -1 && !VREPWrapper.isConnected(idClient))
                return false;
            else
                return true;
        }

        public void drive(float rb, float lb)
        {
            if (IsConnected())
            {
                VREPWrapper.simwSetJointTargetVelocity(idClient, wheelJoints[0], rb);
                VREPWrapper.simwSetJointTargetVelocity(idClient, wheelJoints[1], lb);
                VREPWrapper.simwSetJointTargetVelocity(idClient, wheelJoints[2], rb);
                VREPWrapper.simwSetJointTargetVelocity(idClient, wheelJoints[3], lb);
            }

        }

        public void driveEachWheakle(float fl, float fr, float rl, float rr)
        {
            if (IsConnected())
            {
                VREPWrapper.simwSetJointTargetVelocity(idClient, wheelJoints[0], fl);
                VREPWrapper.simwSetJointTargetVelocity(idClient, wheelJoints[1], rl);
                VREPWrapper.simwSetJointTargetVelocity(idClient, wheelJoints[2], rr);
                VREPWrapper.simwSetJointTargetVelocity(idClient, wheelJoints[3], fr);
            }

        }

        public int getInt(string s)
        {
            if (IsConnected())
                return VREPWrapper.simwGetIntegerSignal(idClient, s);
            else
                return -1;
        }

        public string getPosition()
        {
            return  getSignal("Position");
        }

        Point3d getLaserPosition()
        {
            Point3d mP3d = new Point3d();
            mP3d.X = getFloat(Names.laserPosX);
            mP3d.Y = getFloat(Names.laserPosY);
            mP3d.Z = getFloat(Names.laserPosZ);

            return mP3d;

        }

        Point3d getRobPos()
        {
            Point3d mP3d = new Point3d();
            var a = getFloat(Names.robPosX); ;
            mP3d.X = getFloat(Names.robPosX);
            mP3d.Y = getFloat(Names.robPosY);
            mP3d.Z = getFloat(Names.robPosZ);

            return mP3d;
        }

        Point3d getTargetPos()
        {
            Point3d mP3d = new Point3d();
            mP3d.X = getFloat(Names.targPosX);
            mP3d.Y = getFloat(Names.targPosY);
            mP3d.Z = getFloat(Names.targPosZ);

            return mP3d;
        }


        public float getDist(/*Point3d mPR, Point3d mpL*/)
        {
            float dist = 0 ;
            Point3d mPR = getRobPos();
            Point3d mpL = getTargetPos();
            dist = (float)Math.Sqrt((Math.Pow((mpL.X - mPR.X), 2) + Math.Pow((mpL.Y - mPR.Y), 2)));

            return dist;
        }

        Point3d shiftPoint(Point3d p1, Point3d p2)
        {
            Point3d mp = new Point3d(/*p1.x - p2.x, p1.y - p2.y*/);
            mp.X = p1.X - p2.X;
            mp.Y = p1.Y - p2.Y;
            mp.Z = 0;

            return mp;
        }

        public float computeAngle(/*Point3d posrob, Point3d poslas*/)
        {
            Point3d posrob = getRobPos();
            Point3d poslas = getLaserPosition();
            Point3d postarg = getTargetPos();



            float angle = -200;

            //float dist = getDist(posrob, poslas);
            poslas = shiftPoint(poslas, posrob);
            postarg = shiftPoint(postarg, posrob);

            float a1 = 0;
            float a2 = 0;

            a1 = 180 * (float)Math.Atan2(postarg.Y, postarg.X)/ (float)Math.PI;
            a2 = 180 * (float)Math.Atan2(poslas.Y, poslas.X) / (float)Math.PI;

            angle = a1  -  a2;

            if (angle > 180)
                angle = -360 + angle;

            return angle;
        }

    }

    class Point3d
    {
        float x, y, z;
        public float X { get { return x;}
                         set {x = value ;} }
        public float Y { get { return y;}
                         set {y = value ;} }
        public float Z { get { return z;} 
                         set {z = value ;} }

        public Point3d(float xx = 0, float yy = 0, float zz = 0)
        {
            this.x = xx;
            y = yy;
            z = zz;
        }
    }
}
