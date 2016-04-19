using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Windows.Media;
using System.Windows.Forms;

using remoteApiNETWrapper;
using Controller;

namespace KukaForm
{
    public partial class Form1 : Form
    {
        #region
        QuadrocopterController copter;

        PID pidAltitude;
        PID pidYaw;
        PID pidRoll;
        PID pidPitch;
        PID pidX;
        PID pidY;
        PID pidZ;
        PID pidVX;
        PID pidVY;
        PID pidVYaw;

        float dvx = 0;
        float dvy = 0;
        float dvz = 0;



        CoordinatsOfObject cq;

        Bitmap picfromCopter = null;

        float commonVelocity = 5.335f;

        bool isWorking = false;
        RequiredPosition myReqPos;
        SensorData mySensorData;
        InformationSystem infoSystem;
        WhichControlToUse wctu = WhichControlToUse.PositionControl;
        RequiredPosition mRP;
        SensorData sd;
        Form4 FormDataSens;
        #endregion

        public Form1(Form4 frm4, QuadrocopterController q, RequiredPosition rp)
        {
            InitializeComponent();
            initializePID();

            copter = q;
            myReqPos = rp;
            FormDataSens = frm4;

            timer1.Enabled = false;
        }
        public Form1()
        {
            InitializeComponent();
            initializePID();

        }

        public void initializePID()
        {
            //pidAltitude = new PID(0.4f, 0.0f, 12f);
            //pidYaw = new PID(0.2f, 0.00f, 5f);
            //pidRoll = new PID(0.1f, 0f, 10f);
            //pidPitch = new PID(1f, 0f, 10.0f);
            //pidX = new PID(0.01f, 0.0f, 0.8f);
            //pidY = new PID(0.01f, 0.0f, 0.8f);
            //pidVX = new PID(0.01f, 0.0000f, 0.1f);//(0.0005f, 0.0f, 0.01f);
            //pidVY = new PID(0.01f, 0.0000f, 0.1f);

            pidAltitude = new PID(1f, 0.0f, 12f);
            pidYaw = new PID(0.2f, 0.00f, 5f);
            pidRoll = new PID(0.2f, 0.005f, 5f);
            pidPitch = new PID(0.2f, 0.005f, 5f);
            pidX = new PID(0.0055f, 0.000f, 0.15f);
            pidY = new PID(0.0055f, 0.000f, 0.15f);
            pidZ = new PID(0.05f, 0.000f, 10f);
            pidVX = new PID(0.008f, 0.00000f, 0.15f);//(0.0005f, 0.0f, 0.01f);
            pidVY = new PID(0.008f, 0.00000f, 0.15f);
            pidVYaw = new PID(0.004f, 0.0f, 0.08f);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            
        }
        #region
        public void SetData(InformationSystem setInfo)
        {
            infoSystem = setInfo;
        }

        public void SetData(WhichControlToUse _w, InformationSystem setInfo)
        {
            infoSystem = setInfo;
            wctu = _w;
        }

        public void StartModelToWork(WhichControlToUse _w, RequiredPosition _r)
        {
            isWorking = true;
            timer1.Enabled = true;
            wctu = _w;
            mRP = _r;
        }

        float znak(float f)
        {
            if (f > 0)
                return 1;
            else if (f < 0)
                return -1;
            else
                return 0;
        }

        

        public void StopModelToWork()
        {
            isWorking = false;
            timer1.Enabled = false;
        }
        #endregion

        #region
        public void PositionControl()
        {
            dvx += mySensorData.Speed.X;
            dvy += mySensorData.Speed.Y;
            var dz = mySensorData.Speed.Z;

            var _yaw = mySensorData.Yaw;
            var _vel = mySensorData.Height;
            var _roll = mySensorData.Roll;
            var _pitch = mySensorData.Pitch;

            var ryaw = myReqPos.Yaw;
            var rvel = myReqPos.Height;
            var rroll = myReqPos.Roll;
            var rpitch = myReqPos.Pitch;

            var rx = myReqPos.Position.X;
            var ry = myReqPos.Position.Y;

            //Превращение скоростей в требуемые углы поворота ЛА
          /*  var _dx = pidX.getEffect(0 - dvx);
            if (_dx > 0.5236f)
                _dx = 0.5236f;

            var _dy = pidY.getEffect(dvy - 0);// infoSystem.MyReqPos.Speed.Y);
            if (_dy > 0.5236f)
                _dy = 0.5236f;*/
            
            var dvelx = pidX.getEffect(-dvx );
            if (Math.Abs(dvelx) > 0.5236f)
            {
                dvelx = znak(dvelx) * 0.5236f;
            }

            var dvely = pidY.getEffect(dvy - 0);
            if (Math.Abs(dvely) > 0.5236f)
            {
                dvely = znak(dvely) * 0.5236f;
            }
            //dvelx = 0f;
            //dvely = 0f;
            // При нажатии на кнопку надо обнулять dvх и dvy

            if((Math.Abs(dz) > 0.01f) && (dz != null))
            {
                
                if (dz > 0)
                {
                    dz = pidZ.getEffect(0.01f - dz);
                }
                else if (dz < 0)
                {
                    dz = pidZ.getEffect(-0.01f + dz);
                }
            }
            else
                dz = 0;

            var dyaw = ryaw - _yaw;
            if (dyaw > Math.PI)
                dyaw = -2*(float)Math.PI + dyaw;
            else if (dyaw < -Math.PI)
                dyaw = 2 * (float)Math.PI + dyaw; 
            var yaw = pidYaw.getEffect(dyaw);
            var vel = pidAltitude.getEffect(rvel - _vel);
            var roll = pidRoll.getEffect(/*_dy*/ /*dvely*/  - _roll);
            var pitch = pidPitch.getEffect(/*_dx*/ /*dvelx*/ - _pitch);
            moveDriver(commonVelocity + vel/*-dz*/, 0*yaw, pitch, roll);

            textBox1.Text = _vel.ToString();
        }

        void SpeedControl()
        {
            //ЛА должен сохранять принятое положение в воздухе
            dvx = mySensorData.Speed.X;
            dvy = mySensorData.Speed.Y;

            var _yaw = mySensorData.Yaw;
            var _vel = mySensorData.Height;
            var _roll = mySensorData.Roll;
            var _pitch = mySensorData.Pitch;
            var dyaw = mySensorData.AngularSpeed.Z;
          

            var ryaw = myReqPos.Yaw;
            var rvel = myReqPos.Height;
            var rroll = myReqPos.Roll;
            var rpitch = myReqPos.Pitch;

            var rx = myReqPos.Position.X;
            var ry = myReqPos.Position.Y;

            //Превращение скоростей в требуемые углы поворота ЛА
            var dvelx = pidVX.getEffect(0 - dvx);
            if (Math.Abs(dvelx) > 0.5236f)
            {
                dvelx = znak(dvelx) * 0.5236f;
            }

            var dvely = pidVY.getEffect(dvy - 0);
            if (Math.Abs(dvely) > 0.5236f)
            {
                dvely = znak(dvely) * 0.5236f;
            }

            // При нажатии на кнопку надо обнулять dvх и dvy
            var dvelyaw = pidVYaw.getEffect(0 - dyaw);
            //var yaw = pidYaw.getEffect(ryaw - _yaw);
            var vel = pidAltitude.getEffect(rvel - _vel);
            var roll = pidRoll.getEffect(/*_dy*/ dvely + rroll - _roll);
            var pitch = pidPitch.getEffect(/*_dx*/ dvelx + rpitch - _pitch);
            moveDriver(commonVelocity + vel, dvelyaw, pitch, roll);
            //MyProcess = WhichProcess.Nothing;
        }

        void StraightFlyControl()
        {
            //ЛА должен сохранять принятое положение в воздухе
            dvx += mySensorData.Speed.X;
            dvy += mySensorData.Speed.Y;

            var _yaw = mySensorData.Yaw;
            var _vel = mySensorData.Height;
            var _roll = mySensorData.Roll;
            var _pitch = mySensorData.Pitch;
            var dyaw = mySensorData.AngularSpeed.Z;


            var ryaw = myReqPos.Yaw;
            var rvel = myReqPos.Height;
            var rroll = myReqPos.Roll;
            var rpitch = myReqPos.Pitch;

            //var rx = myReqPos.;
           // var ry = myReqPos.Position.Y;

            //Превращение скоростей в требуемые углы поворота ЛА
            var dvelx = pidVX.getEffect(myReqPos.Speed.X - dvx* (float)Math.Cos(_pitch));
            /*if (dvelx > 0.5236f)
                dvelx = 0.5236f;*/

            var dvely = pidVY.getEffect(dvy - 0);
            if (Math.Abs(dvely) > 0.5236f)
            {
                dvely = znak(dvely) * 0.5236f;
            }

            // При нажатии на кнопку надо обнулять dvх и dvy
            //var dvelyaw = pidVYaw.getEffect(0 - dyaw);
            var yaw = pidYaw.getEffect(ryaw - _yaw);
            var vel = pidAltitude.getEffect(/*-dvelx*/  + rvel - _vel);
            var roll = pidRoll.getEffect(/*_dy* dvely*/ + 0 - _roll);
            var pitch = pidPitch.getEffect(/*_dx* dvelx*/ + rpitch - _pitch);
            moveDriver(commonVelocity + vel /*+ dvelx*/, yaw, pitch, roll);
            //MyProcess = WhichProcess.Nothing;
        }

        void PositionHover()
        {
            dvx = mySensorData.Speed.X;
            dvy = mySensorData.Speed.Y;
            dvz += mySensorData.Speed.Z;

            var _yaw = mySensorData.Yaw;
            var _vel = mySensorData.Height;
            var _roll = mySensorData.Roll;
            var _pitch = mySensorData.Pitch;

            var ryaw = myReqPos.Yaw;
            var rvel = myReqPos.Height;
            var rroll = myReqPos.Roll;
            var rpitch = myReqPos.Pitch;

            var rx = myReqPos.Position.X;
            var ry = myReqPos.Position.Y;

   

            var dvelx = pidVX.getEffect(0 - dvx);
            if (Math.Abs(dvelx) > 1f)
            {
                dvelx = znak(dvelx) * 1f;
            }

            var dvely = pidVY.getEffect(dvy - 0);
            if (Math.Abs(dvely) > 1f)
            {
                dvely = znak(dvely) * 1f;
            }

            var dyaw = ryaw - _yaw;
            if (dyaw > Math.PI)
                dyaw = -2 * (float)Math.PI + dyaw;
            else if (dyaw < -Math.PI)
                dyaw = 2 * (float)Math.PI + dyaw;
            var yaw = pidYaw.getEffect(dyaw);
            //var yaw = pidYaw.getEffect(ryaw - _yaw);
            var vel = pidAltitude.getEffect(rvel - _vel);//pidAltitude.getEffect(rvel - _vel);
            var roll = pidRoll.getEffect(/*dvely*/ - _roll);
            var pitch = pidPitch.getEffect(/*dvelx*/  - _pitch);
            moveDriver(commonVelocity + vel, yaw, pitch + dvelx, roll + dvely);

            textBox1.Text = _vel.ToString();
        }

        void fall()
        {
            dvx = mySensorData.Speed.X;
            dvy = mySensorData.Speed.Y;
            dvz += mySensorData.Speed.Z;

            var _yaw = mySensorData.Yaw;
            var _vel = mySensorData.Height;
            var _roll = mySensorData.Roll;
            var _pitch = mySensorData.Pitch;

            var ryaw = myReqPos.Yaw;
            var rvel = myReqPos.Height;
            var rroll = myReqPos.Roll;
            var rpitch = myReqPos.Pitch;

            var rx = myReqPos.Position.X;
            var ry = myReqPos.Position.Y;



            var dvelx = pidVX.getEffect(0 - dvx);
            if (Math.Abs(dvelx) > 0.5236f)
            {
                dvelx = znak(dvelx) * 0.5236f;
            }

            var dvely = pidVY.getEffect(dvy - 0);
            if (Math.Abs(dvely) > 0.5236f)
            {
                dvely = znak(dvely) * 0.5236f;
            }

            var yaw = pidYaw.getEffect(ryaw - _yaw);
            //var vel = pidAltitude.getEffect(rvel - _vel);//pidAltitude.getEffect(rvel - _vel);
            var roll = pidRoll.getEffect(0 - _roll);
            var pitch = pidPitch.getEffect(0 - _pitch);
            moveDriver(0, yaw, pitch, roll);
        }

        void SetOffCopter()
        {
            moveDriver(0, 0, 0, 0);
        }




        void YawControl()
        {
            //ЛА должен сохранять принятое положение в воздухе
            dvx = mySensorData.Speed.X;
            dvy = mySensorData.Speed.Y;

            var _yaw = mySensorData.Yaw;
            var _vel = mySensorData.Height;
            var _roll = mySensorData.Roll;
            var _pitch = mySensorData.Pitch;
            //var dyaw = mySensorData.AngularSpeed.Z;


            var ryaw = myReqPos.Yaw;
            var rvel = myReqPos.Height;
            var rroll = myReqPos.Roll;
            var rpitch = myReqPos.Pitch;
            //var dyaw = mySensorData.AngularSpeed.Z;//


            //Превращение скоростей в требуемые углы поворота ЛА
            var dvelx = pidVX.getEffect(0 - dvx);
            /*if (Math.Abs(dvelx) > 0.5236f)
            {
                dvelx = znak(dvelx) * 0.5236f;
            }*/

           // var dvelyaw = pidVYaw.getEffect(0 - dyaw);
            

            var dvely = pidVY.getEffect(dvy - 0);
            /*if (Math.Abs(dvely) > 0.5236f)
            {
                dvely = znak(dvely) * 0.5236f;
            }*/
            /*float dvyaw = 0;
            if (Math.Abs(dyaw) > 0.2)
            {
                
                if(dyaw > 0)
                {
                    dvyaw = pidVYaw.getEffect(0.2f - dyaw);
                }
                else if(dyaw < 0)
                {
                    dvyaw = pidVYaw.getEffect(-0.2f - dyaw);
                }
            }*/

            // При нажатии на кнопку надо обнулять dvх и dvy
            var dyaw = ryaw - _yaw;
            if (dyaw > Math.PI)
                dyaw = -2 * (float)Math.PI + dyaw;
            else if (dyaw < -Math.PI)
                dyaw = 2 * (float)Math.PI + dyaw;
            var yaw = pidYaw.getEffect(dyaw);
            //var yaw = pidYaw.getEffect(ryaw - _yaw);
            var vel = pidAltitude.getEffect(rvel - _vel);
            var roll = pidRoll.getEffect(/*dvely*/   - _roll);
            var pitch = pidPitch.getEffect(/*dvelx*/- _pitch);
            moveDriver(commonVelocity + vel, yaw /*- dyaw*/, pitch/*+dvelx*/, roll/*+dvely*/);
            //MyProcess = WhichProcess.Nothing;
        }

        void moveDriver(float vel, float yaw, float pitch, float roll)
        {
            copter.setVelocityToForceDriver(0, vel - yaw - pitch + roll);
            copter.setVelocityToForceDriver(1, vel + yaw + pitch + roll);
            copter.setVelocityToForceDriver(2, vel - yaw + pitch - roll);
            copter.setVelocityToForceDriver(3, vel + yaw - pitch - roll);
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateSensorData();
            switch (wctu)
            {
                case WhichControlToUse.PositionControl: PositionControl();  break;
                case WhichControlToUse.SpeedControl: break;
                case WhichControlToUse.YawControl: YawControl(); break;
                case WhichControlToUse.MoveForward: StraightFlyControl(); break;
                case WhichControlToUse.Hover:
                    PositionHover(); break;
                case WhichControlToUse.Off: SetOffCopter(); break;
                case WhichControlToUse.Fall: fall(); break;

            }
           

        }

        void UpdateSensorData()
        {
            mySensorData = FormDataSens.GetDataFromCopter();
        }

        public void GetRequareMoving(RequiredPosition mR)
        {
            //myReqPos = mR;
            myReqPos.Pitch = mR.Pitch;
            myReqPos.Roll = mR.Roll;
            myReqPos.Yaw = mySensorData.Yaw;
            myReqPos.Position.X = myReqPos.Position.Y = 0;
            myReqPos.Speed = new Points3(mR.Speed.X, mR.Speed.Y, mR.Speed.Z);
            myReqPos.Height = mR.Height;
        }
        public void GetRequareMoving(RequiredPosition mR, WhichControlToUse wc)
        {
            myReqPos.Pitch = mR.Pitch;
            myReqPos.Roll = mR.Roll;
            myReqPos.Yaw = mR.Yaw;
            myReqPos.Position.X = myReqPos.Position.Y = 0;
            myReqPos.Speed = new Points3(mR.Speed.X, mR.Speed.Y, mR.Speed.Z);
            myReqPos.Height = mR.Height;
            if (wctu != wc)
                initializePID();
            wctu = wc;
        }

    }

    public enum WhichControlToUse
    { PositionControl, SpeedControl, YawControl, HeightControl, MoveForward, Hover, Fall, Off}
}
