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
        PID pidDx;
        PID pidControlVX;
        PID pidDy;
        PID pidVYaw;

        FuzzyPID fpidPitch;
        FuzzyPID fpidRoll;

        FuzzyPID fpidVx;

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
        WhichControlToUse wctu = WhichControlToUse.Nothing;
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

            timer1.Enabled = true;
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

            pidAltitude = new PID(0.5f, 0.000001f, 10f);
            pidYaw = new PID(0.2f, 0.00f, 5f);
            pidRoll = new PID(0.5f, 0.000001f, 8f);
            pidPitch = new PID(0.5f, 0.000001f, 12f);


            pidX = new PID(0.005f, 0.0000f, 0.1f);
            pidY = new PID(0.005f, 0.000f, 0.1f);
            pidZ = new PID(0.03f, 0.000f, 10f);
            pidVX = new PID(0.075f, 0.001f, 0.1f);//(0.0005f, 0.0f, 0.01f);
            pidVY = new PID(0.075f, 0.001f, 0.1f);
            pidVYaw = new PID(0.004f, 0.0f, 0.08f);

            pidDx = new PID(0.001f, 0.000001f, 0.01f);
            pidControlVX = new PID(0.05f, 0.000f, 1f);
            pidDy = new PID(0.008f, 0, 0.05f);

            fpidPitch = new FuzzyPID(new float[]{ 1.55f, 0.8f, 0.5f}, new float[] { 0.00000f, 0.00000f, 0.00000f }, new float[] { 12f, 10f, 8f }, new float[] { 0.1f,  1 });
            fpidRoll = new FuzzyPID(new float[] { 1.55f, 0.8f, 0.5f }, new float[] { 0.00000f, 0.00000f, 0.00000f }, new float[] { 12f, 10f, 8f }, new float[] { 0.1f, 1 });
            fpidVx = new FuzzyPID(new float[] { 0.0001f, 0.0002f, 0.002f }, new float[] { 0.00000f, 0.00000f, 0.00000f }, new float[] { 0.001f, 0.001f, 0.001f }, new float[] { 0.05f, 0.5f });
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

          
            
            var dvelx = pidX.getEffect(-dvx );
            if (Math.Abs(dvelx) > 0.5236f)
            {
                dvelx = Math.Sign((dvelx)) * 0.5236f;
            }

            var dvely = pidY.getEffect(dvy - 0);
            if (Math.Abs(dvely) > 0.5236f)
            {
                dvely = Math.Sign((dvely)) * 0.5236f;
            }


            var dyaw = ryaw - _yaw;
            if (dyaw > Math.PI)
                dyaw = -2*(float)Math.PI + dyaw;
            else if (dyaw < -Math.PI)
                dyaw = 2 * (float)Math.PI + dyaw; 
            var yaw = pidYaw.getEffect(dyaw);
            var vel = pidAltitude.getEffect(rvel - _vel);
            var roll = pidRoll.getEffect( dvely  - _roll);
            var pitch = fpidPitch.GetEffect(dvelx - _pitch);//pidPitch.getEffect(/*_dx*/ dvelx - _pitch);
            moveDriver(commonVelocity + vel, yaw, pitch, roll);

            textBox1.Text = "Position control...";
            textBox1.Text += Environment.NewLine + _vel.ToString();
        }

        public void CoordinateControl()
        {
            
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

           
            

            var yaw = pidYaw.getEffect(dz);
            var vel = pidAltitude.getEffect(rvel - _vel);
            var roll = pidRoll.getEffect( rroll - _roll);
            var pitch = fpidPitch.GetEffect(rpitch - _pitch);
            moveDriver(commonVelocity + vel, yaw, pitch, roll);

            textBox1.Text = "Position control...";
            textBox1.Text += Environment.NewLine + _vel.ToString();
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
            var dangz = mySensorData.AngularSpeed.Z;

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

            var dvelx = pidVX.getEffect(-dvx);
            if (Math.Abs(dvelx) > 0.5236f)
            {
                dvelx = Math.Sign((dvelx)) * 0.5236f;
            }

            var dvely = pidVY.getEffect(dvy - 0);
            if (Math.Abs(dvely) > 0.5236f)
            {
                dvely = Math.Sign((dvely)) * 0.5236f;
            }
            var dyaw = mySensorData.AngularSpeed.Z;



            var yaw = pidYaw.getEffect(-dyaw);
            var vel = pidAltitude.getEffect(rvel - _vel);//pidAltitude.getEffect(rvel - _vel);
            /*var roll = pidRoll.getEffect(/*dvely*- _roll);
            var pitch = pidPitch.getEffect(/*dvelx  - _pitch);
            moveDriver(commonVelocity + vel, yaw, pitch - dvelx, roll - dvely);*/
            var roll = pidRoll.getEffect(/*_dy*/ dvely - _roll);
            var pitch = pidPitch.getEffect(/*_dx*/ dvelx - _pitch);


            textBox1.Text = "Position hover...";
            textBox1.Text += Environment.NewLine + _vel.ToString();
            moveDriver(commonVelocity + vel, yaw, pitch, roll);

            

        }


        void PitchRoll()
        {
            dvx = mySensorData.Speed.X;
            dvy = mySensorData.Speed.Y;
            dvz += mySensorData.Speed.Z;
            var dangz = mySensorData.AngularSpeed.Z;
            var dangpitch = mySensorData.AngularSpeed.Y;
            var dangroll = mySensorData.AngularSpeed.X;

            var _yaw = mySensorData.Yaw;
            var _vel = mySensorData.Height;
            var _roll = mySensorData.Roll;
            var _pitch = mySensorData.Pitch;

            

            var ryaw = myReqPos.Yaw;
            var rvel = myReqPos.Height;
            var rroll = myReqPos.Roll;
            var rpitch = myReqPos.Pitch;

            var dryaw = myReqPos.Speed.Z;

            var rx = myReqPos.Position.X;
            var ry = myReqPos.Position.Y;

            Console.WriteLine(rvel.ToString() + " " + rroll.ToString() + " " + rpitch.ToString());


            var dvelx = pidVX.getEffect(rpitch*100-dvx);
            if (Math.Abs(dvelx) > 0.5236f)
            {
                dvelx = Math.Sign((dvelx)) * 0.5236f;
            }

            var dvely = pidVY.getEffect(dvy + 10*rroll);
            if (Math.Abs(dvely) > 0.5236f)
            {
                dvely = Math.Sign((dvely)) * 0.5236f;
            }



            float velx = 0;
            if (Math.Abs(dvx) > 0.05f)
            {
                if (dvx > 0.05f)
                {
                    velx = pidControlVX.getEffect(0.05f - dvx);
                }
                else
                {
                    velx = pidControlVX.getEffect(-0.05f - dvx);
                }
            }

            var yaw = pidYaw.getEffect(-dangz - dryaw);
            //var yaw = pidYaw.getEffect(-dangz);
            var vel = pidAltitude.getEffect(rvel - _vel);//pidAltitude.getEffect(rvel - _vel);
            var roll = pidRoll.getEffect(dvely - _roll);//pidRoll.getEffect(/*-dvely +*/ rroll - _roll);
            float pitch = 0;
            //if (rpitch != 0)

                pitch = /*fpidVx.GetEffect(velx + 0 - dvx);*/ pidPitch.getEffect(dvelx - _pitch);
            //else
               // pitch = pidDx.getEffect(velx - dvx);//pidDx.getEffect(velx - dvx);


            Console.WriteLine(vel.ToString() + " " + roll.ToString() + " " + pitch.ToString());
           

            textBox1.Text = "PitchRoll...";
            textBox1.Text += Environment.NewLine + dvx.ToString() + " " + velx.ToString();
            moveDriver(commonVelocity + vel, yaw, pitch, roll);
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
            moveDriver(commonVelocity + vel, yaw/* - dyaw*/, pitch+dvelx, roll+dvely);
            //MyProcess = WhichProcess.Nothing;
        }

        void moveDriver(float vel, float yaw, float pitch, float roll)
        {
            copter.setVelocityToForceDriver(0, vel - yaw - pitch + roll);
            copter.setVelocityToForceDriver(1, vel + yaw + pitch + roll);
            copter.setVelocityToForceDriver(2, vel - yaw + pitch - roll);
            copter.setVelocityToForceDriver(3, vel + yaw - pitch - roll);

            //copter.setVelocityToForceDriver(0, commonVelocity * (vel -yaw + pitch + roll));
            //copter.setVelocityToForceDriver(1, commonVelocity * ( vel + yaw - pitch + roll));
            //copter.setVelocityToForceDriver(2, commonVelocity * (vel - yaw - pitch - roll));
            //copter.setVelocityToForceDriver(3, commonVelocity * ( vel+ yaw + pitch - roll));


            textBox1.Text += Environment.NewLine + (vel - yaw - pitch + roll).ToString() + " " + (vel + yaw + pitch + roll).ToString();
            textBox1.Text += Environment.NewLine + (vel - yaw + pitch - roll).ToString() + " " + (vel + yaw - pitch - roll).ToString();
        }
        #endregion

        WhichControlToUse prevcontrl = WhichControlToUse.Nothing;

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateSensorData();
            switch (wctu)
            {
                case WhichControlToUse.Nothing: break;
                case WhichControlToUse.PositionControl: PositionControl();  break;
                case WhichControlToUse.SpeedControl: break;
                case WhichControlToUse.YawControl: YawControl(); break;
                case WhichControlToUse.MoveForward: StraightFlyControl(); break;
                case WhichControlToUse.Hover: PositionHover(); break;
                case WhichControlToUse.Off: SetOffCopter(); break;
                case WhichControlToUse.Fall: fall(); break;
                case WhichControlToUse.PitchRoll: PitchRoll(); break;
                case WhichControlToUse.CoordinateControl: CoordinateControl(); break;

            }

            if (prevcontrl != wctu)
                ControlToChange();
            prevcontrl = wctu;
        }

        void ControlToChange()
        {
            dvx = dvy = dvz = 0;
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

}
