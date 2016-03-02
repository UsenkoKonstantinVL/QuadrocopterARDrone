using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Controller
{
    class QuadrocopterController
    {
        VRepController vrep;
        List<Drive> drivers;
        List<VisionSensor> sensors;
        int CopterDummy = 0;
        SensorData mySensorData;

        public QuadrocopterController(int port)
        {
            vrep = new VRepController(port);
            drivers = new List<Drive>();
            sensors = new List<VisionSensor>();
            
        }

        public void Start()
        {
            vrep.ConnectionStart();
            CopterDummy = vrep.ObjectHandle("Quadricopter_base");
        }

        public void Finish()
        {
            vrep.ConnectionStop();
            drivers.Clear();
            sensors.Clear();
        }


        public void AddDrive(string name, string nameOfSignal)
        {
            Drive dr = new Drive(vrep, name, nameOfSignal);
            drivers.Add(dr);
        }

        public void AddSensor(string name)
        {
            VisionSensor sens = new VisionSensor(vrep, (name));
            sensors.Add(sens);
        }


        public Bitmap getDataFromISensor(int i)
        {
            if (sensors.Count > i)
                return sensors[i].getImageFromVisionSensor();
            else
                return null;
        }

        public void setVelocityToIDriver(int i, float vel)
        {
            if (drivers.Count > i)
                drivers[i].setVelocityToDrive(vel);
            
        }

        public void setVelocityToForceDriver(int i,float vel)
        {
            if (drivers.Count > i)
                drivers[i].setVelocityToForceDrive(vel);
        }

        public VRepController getVrepController()
        {
            return vrep;
        }


        public SensorData GetSensorData()
        {
            SensorData sdata = new SensorData();

            var orient = vrep.getObjectOrientation(CopterDummy, -1);
            //Points3 or = new Points3(orient[0], orient[1], orient[2]);

            var vx = vrep.getFloatSignal("vx");
            var vy = vrep.getFloatSignal("vy");
            var vz = vrep.getFloatSignal("vz");

            
            Points3 speed = new Points3(vx, vy, vz);

            var pos = vrep.getObjectPosition(CopterDummy);
            Points3 poss = new Points3(pos[0], pos[1], pos[2]);

            sdata.Roll = orient[0];
            sdata.Pitch = orient[1];
            sdata.Yaw = orient[2];

            sdata.Speed = speed;
            sdata.Coordinates = poss;

            sdata.Height = pos[2];

            mySensorData = sdata;

            return sdata;
        }

    }
    class SensorData
    {
        float yaw, pitch, roll;
        float height;
        float x, y, z;
        float vx, vy, vz;
        float ax, ay, az;
        Points3 coordinates;
        Points3 speed;
        Points3 acceleration;


        public SensorData()
        {
            yaw = pitch = roll = 0;
            height = 0;
            x = y = z = vx = vy = vz = ax = ay = az = 0;
            coordinates = new Points3();
            speed = new Points3();
            acceleration = new Points3();

        }

        public float Pitch
        {
            set { pitch = value; }
            get { return pitch; }
        }

        public float Roll
        {
            set { roll = value; }
            get { return roll; }
        }

        public float Yaw
        {
            set { yaw = value; }
            get { return yaw; }
        }
        public float Height
        {
            set { height = value; }
            get { return height; }
        }


        public Points3 Coordinates
        {
            set { coordinates = value; }
            get { return coordinates; }
        }

        public Points3 Speed
        {
            set { speed = value; }
            get { return speed; }
        }

        public Points3 Acceleration
        {
            set { acceleration = value; }
            get { return acceleration; }
        }


    }

    class Points3
    {
        float x, y, z;

        public Points3()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public Points3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public float X
        {
            set { x = value; }
            get { return x; }
        }

        public float Y
        {
            set { y = value; }
            get { return y; }
        }

        public float Z
        {
            set { z = value; }
            get { return z; }
        }
    }

    class RequiredPosition
    {
        float roll, pitch, yaw;
        float height;
        Points3 speed;

        public RequiredPosition()
        {
            roll = pitch = yaw = height = 0;
            speed = new Points3();
        }

        public float Pitch
        {
            set { pitch = value; }
            get { return pitch; }
        }

        public float Roll
        {
            set { roll = value; }
            get { return roll; }
        }

        public float Yaw
        {
            set { yaw = value; }
            get { return yaw; }
        }

        public float Height
        {
            set { height = value; }
            get { return height; }
        }

        public Points3 Speed
        {
            set { speed = value; }
            get { return speed; }
        }
    }
}
