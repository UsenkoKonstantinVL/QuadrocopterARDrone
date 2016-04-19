using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Controller
{
    public class QuadrocopterController
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
            mySensorData = new SensorData();
            
        }

        public void Start()
        {
            vrep.ConnectionStart();
            CopterDummy = vrep.ObjectHandle("Quadricopter");// _base");
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

            var vm = (float)Math.Sqrt(Math.Pow(vx, 2) + Math.Pow(vy,2));

            var alpha = Math.Acos(vx/vm);

            if(vy < 0)
            {
                alpha = -alpha;
            }

            Points3 speed = new Points3(vm*(float)Math.Cos(alpha - orient[2]), vm * (float)Math.Sin(alpha - orient[2]), vz);

            var pos = vrep.getObjectPosition(CopterDummy);
            Points3 poss = new Points3(pos[0], pos[1], pos[2]);

            sdata.Roll = (float)(Math.Cos(-orient[2])* orient[0] - Math.Sin(-orient[2]) * orient[1]);
            sdata.Pitch = (float)(Math.Sin(-orient[2]) * orient[0] + Math.Cos(-orient[2]) * orient[1]);
            sdata.Yaw = orient[2];

            sdata.Speed = speed;
            sdata.Coordinates = poss;

            var dt = vrep.GetVelocity(CopterDummy);

            sdata.AngularSpeed = new Points3(dt.angularVelocity[0], dt.angularVelocity[1], dt.angularVelocity[2]);

            sdata.Height = pos[2];

            //sdata.Coordinates = new Points3(mySensorData.Coordinates.X  + (mySensorData.Speed.X * 0.01f), mySensorData.Coordinates.Y + (mySensorData.Speed.Y * 0.01f), mySensorData.Coordinates.Z + (mySensorData.Speed.Z * 0.01f));
            sdata.Coordinates = new Points3(poss.X,poss.Y, poss.Z);
            mySensorData = sdata;

            return sdata;
        }

    }
    public class SensorData
    {
        float yaw, pitch, roll;
        float height;
        float x, y, z;
        float vx, vy, vz;
        float ax, ay, az;
        Points3 coordinates;
        Points3 speed;
        Points3 acceleration;


        public string PrintResult()
        {
            string nl = Environment.NewLine;
            string ypr = "Yaw " + yaw.ToString() + " Pitch " + pitch.ToString() + " Roll "+ roll.ToString();
            string h = "Height " + height.ToString();
            string cr = coordinates.PrintResult();
            string spd = speed.PrintResult();
            string acc = AngularSpeed.PrintResult();


            return ypr +
                nl + h +
                nl + "Coordinates " + cr +
                nl + "Speed " + spd +
                nl + "Acceleration " + acc;
        }


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

        public Points3 AngularSpeed
        {
            set { acceleration = value; }
            get { return acceleration; }
        }


    }

    public class Points3
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

        public string PrintResult()
        {
            return "X " + X.ToString() + " Y: " + Y.ToString() + " Z: " + Z.ToString();
        }
    }

    public class RequiredPosition
    {
        float roll, pitch, yaw;
        float height;
        Points3 speed;
        Points3 position;


        public RequiredPosition()
        {
            roll = pitch = yaw = height = 0;
            speed = new Points3();
            position = new Points3();
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
        public Points3 Position
        {
            set { position = value; }
            get { return position; }
        }
    }
}
