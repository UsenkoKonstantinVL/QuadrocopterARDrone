using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller;
using System.Threading;
using System.Diagnostics;

namespace KukaForm
{
    public partial class Form2 : Form
    {
        #region Используемые глобальные переменные
        QuadrocopterController copter;

        Thread workerThread = null;
        Thread workerThread2 = null;

        PID pidAltitude;
        PID pidYaw;
        PID pidRoll;
        PID pidPitch;
        PID pidX;
        PID pidY;
        PID pidVX;
        PID pidVY;

        

        CoordinatsOfObject cq;

        Bitmap picfromCopter = null;

        float commonVelocity = 5.335f;

        bool isWorking = false;
        RequiredPosition myReqPos;
        SensorData mySensorData;
        InformationSystem infoSystem;

        enum WhichProcess { Nothing, Height, Pitch, Roll, Yaw }
        WhichProcess MyProcess = WhichProcess.Nothing;


        #endregion
        public Form2()
        {
            InitializeComponent();
            copter = new QuadrocopterController(7777);

            initializePID();

            cq = new CoordinatsOfObject(copter.getVrepController(), copter.getVrepController().ObjectHandle("Quadricopter"));//_base"));
            //PointXYZ point = new PointXYZ();
            label5.Text = "";
        }

        public void initializePID()
        {
            pidAltitude = new PID(0.4f, 0.0f, 12f);
            pidYaw = new PID(0.2f, 0.00f, 5f);
            pidRoll = new PID(0.1f, 0f, 10f);
            pidPitch = new PID(1f, 0f, 16.0f);
            pidX = new PID(0.0001f, 0.0f, 0.008f);
            pidY = new PID(0.0001f, 0.0f, 0.08f);
            pidVX = new PID(0.01f, 0.0000f, 0.1f);//(0.0005f, 0.0f, 0.01f);
            pidVY = new PID(0.01f, 0.0000f, 0.1f);

            workerThread2 = new Thread(flyUp);
            workerThread = new Thread(PidRegulation);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Старт")
            {
                copter.Start();
                copter.AddDrive("Quadricopter_propeller1", "dVelocity1");
                copter.AddDrive("Quadricopter_propeller2", "dVelocity2");
                copter.AddDrive("Quadricopter_propeller3", "dVelocity3");
                copter.AddDrive("Quadricopter_propeller4", "dVelocity4");

                
                copter.AddSensor("Vision_sensor1");
                copter.AddSensor("Visionfloor_sensor");
                //copter.AddSensor("Visionfront_sensor");
                //copter.AddSensor("Visionfloor_sensor");

                /*workerThread = new Thread(setPicture);
                workerThread.Start();*/

                button1.Text = "Стоп";


                isWorking = true;
                timer1.Enabled = true;
                myReqPos = new RequiredPosition();
                myReqPos.Height = 1f;
                mySensorData = new SensorData();
                mySensorData = copter.GetSensorData();
                myReqPos.Yaw = mySensorData.Yaw;

                infoSystem = new InformationSystem(copter);
                infoSystem.MyReqPos = myReqPos;
                infoSystem.MySensorData = mySensorData;

                MyProcess = WhichProcess.Height;

                initializePID();

            }
            else
            {
                button1.Text = "Старт";
                copter.Finish();
                isWorking = false;
                timer1.Enabled = false;
                //workerThread.Abort();
            }

        }

        public void setPicture()
        {
            //copter.getVrepController().isClientConnected())
            {
                // Bitmap bmp = copter..getImageFromVisionSensor();
                /*picfromCopter = copter.getDataFromISensor(0);
                pictureBox1.Image = picfromCopter;*/
                pictureBox1.Image = infoSystem.GetPictureFromCamera;
                //pictureBox2.Image = infoSystem.InformationFromCamera.bmp;
                //Thread.Sleep(25);
                //Thread.Sleep(10);
                //pictureBox2.Image = copter.getDataFromISensor(1);
                //Thread.Sleep(50);
            }
        }

        public void GetPictureToPicBox(Bitmap bmp)
        {
            pictureBox2.Image = bmp;
        }

        public delegate void DelegateForGettingPicture(InformationFromPicture bmp);

        public void GetPictureToPicBox(InformationFromPicture info)
        {
            
            if (label5.InvokeRequired)
            {
                DelegateForGettingPicture d = new DelegateForGettingPicture(GetPictureToPicBox);
                Invoke(d, new object[] { info });
            }
            else
            {
                label5.Text = info.cCyrclX.ToString() + " | " + info.cCyrclY.ToString();
            }
        }

        string GetProcessName()
        {
            string st = "";

            switch (MyProcess)
            {
                case WhichProcess.Height:
                    st = "Height";
                    break;
                case WhichProcess.Nothing:
                    st = "Nothing";
                    break;
                case WhichProcess.Pitch:
                    st = "Pitch";
                    break;
                case WhichProcess.Roll:
                    st = "Roll";
                    break;
                case WhichProcess.Yaw:
                    st = "Yaw";
                    break;
            }

            return st;
        }

        #region Непонятно что

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Взлет")
            {
                if (copter.getVrepController().isClientConnected())
                {
                    workerThread = new Thread(flyUp);
                    workerThread.Start();
                   /* workerThread2 = new Thread(setPicture);
                    workerThread2.Start();*/
                    button2.Text = "Стоп";
                }
            }
            else
            {
                if (workerThread.IsAlive)
                    workerThread.Abort();
               /* if (workerThread2.IsAlive)
                    workerThread2.Abort();*/
                button2.Text = "Взлет";
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(button3.Text == "Ролл")
            {
                if (copter.getVrepController().isClientConnected())
                {
                    workerThread = new Thread(PidRegulation);
                    workerThread.Start();
                    button3.Text = "Стоп";
                }
            }
            else
            {
                if (workerThread.IsAlive)
                    workerThread.Abort();
                button3.Text = "Ролл";
            }
        }


        

        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            //flyUp();
            /*
            if (copter.getVrepController().isClientConnected())
            {
                
                workerThread2 = new Thread(setPicture);
                workerThread2.Start();
               
            }*/
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text == "Автономное перемещение")
            {
                if (copter.getVrepController().isClientConnected())
                {
                    workerThread = new Thread(autonomusfly);
                    workerThread.Start();
                    
                    button4.Text = "Стоп";
                }
            }
            else
            {
                if (workerThread.IsAlive)
                    workerThread.Abort();
                
                button4.Text = "Автономное перемещение";
            }
        }

        public void autonomusfly()
        {
            int com = 0;


            while(copter.getVrepController().isClientConnected())
            {
                switch (com)
              {

                    case 0:
                       com = autonomusFlyUp();
                        break;
                    case 1:
                        com = AutonomusPidRegulation();
                        break;

                }
            }
        }

         int autonomusFlyUp()
        {
            var vrep = copter.getVrepController();
            float high = 1;
            var vel = 0f;
            var yaw = 0f;
            var pitch = 0f;
            var roll = 0f;
            var nline = Environment.NewLine;
            var vx = 0f;
            var vy = 0f;
            var c = vrep.getFloatSignal("z");
            vx = vrep.getFloatSignal("vx");
            vy = vrep.getFloatSignal("vy"); ;
            // c = copter.getVrepController().getFloatSignal("z");
            var xp = 0.5f;
            var yp = 0.5f;

            var quadrbase = vrep.ObjectHandle("Quadricopter_base");

            var e = high - c;
            float[] err = new float[3];
            for (int i = 0; i < 3; i++)
            {
                err[i] = e;
            }

            //Stopwatch sWatch = new Stopwatch();
            float xxx = 0f, yyy = 0f;
            int j = 0;
            float sum = e;
            //float f = 0;
            while ((vrep.isClientConnected() && (sum > 0.01f)) || ((xxx>0.01f||xxx<-0.1f)&&(yyy>0.01f||yyy<-0.1f)))
            {
                // 
                c = vrep.getFloatSignal("z");
                var v = vrep.getObjectOrientation(quadrbase, -1);
                vx = vrep.getFloatSignal("vx");
                vy = vrep.getFloatSignal("vy");

                xxx += vx;
                yyy += vy;

                var xx = pidVX.getEffect(0 - xxx);
                if (xx > 0.5236f)
                    xx = 0.5236f;

                var yy = pidVY.getEffect(yyy - 0);
                if (yy > 0.5236f)
                    yy = 0.5236f;


                yaw = pidYaw.getEffect(0 - v[2]);
                vel = pidAltitude.getEffect(high - c);
                roll = pidRoll.getEffect((yy - v[0]));
                pitch = pidPitch.getEffect(xx - v[1]);
                moveDriver(commonVelocity + vel, yaw, pitch, roll);
                var st = "Velx: " + xxx.ToString() + nline + "Vely: " + yyy.ToString() + nline + nline;//"Xtarg : " + xp.ToString() + nline + "Ytarg: " + yp.ToString() + nline + "Xc:" + x.ToString() + nline + "Yc:" + y.ToString() + nline;
                var str = st + "XX vel:" + xx.ToString() + nline + "YY vel:" + yy.ToString() + nline + "Yaw:" + yaw.ToString() + nline + "Roll: " + roll + nline + "Pitch" + pitch;//c.ToString() + Environment.NewLine + (vel + 5.35f).ToString() + Environment.NewLine + (high - c).ToString();
                var str2 = nline + nline + "Atitude:" + c.ToString() + nline + "Velocity: " + vel.ToString() + nline + "Real yaw :" + v[2].ToString() + nline + "Real pitch:" + v[1].ToString() + nline + "Real roll: " + v[0].ToString();
                SetDistanceToLable(textBox1, str + str2);


                err[j] += high - c;
                j++;
                if (j > 2)
                    j = 0;


                for (int i = 0; i < err.Length; i++)
                    sum += err[i];
                sum = sum / err.Length;
                
            }
            SetDistanceToLable(textBox1, "Взлет закончен...");
            return 1;
        }

        int AutonomusPidRegulation()
        {
            float high = 1;
            var vel = 0f;
            var yaw = 0f;
            var pitch = 0f;
            var roll = 0f;
            var nline = Environment.NewLine;
            var x = 0f;
            var y = 0f;
            var c = copter.getVrepController().getFloatSignal("z");
            x = copter.getVrepController().getFloatSignal("x");
            y = copter.getVrepController().getFloatSignal("y"); ;
            // c = copter.getVrepController().getFloatSignal("z");
            var xp = 1.5f;
            var yp = 1.5f;



            //float f = 0;
            while (copter.getVrepController().isClientConnected())
            {
                /*PointXYZ p = cq.getCoordinatOfObj();
                f = p[2];
                var ff = p[1];
                //vel = pid.getEffect(high - f);
                //moveDriver(vel);*/
                var v = copter.getVrepController().getObjectOrientation(copter.getVrepController().ObjectHandle("Quadricopter_base"), -1);

                x = copter.getVrepController().getFloatSignal("x");
                y = copter.getVrepController().getFloatSignal("y"); ;
                c = copter.getVrepController().getFloatSignal("z");
                //vel = pid.getEffect(high - v[2]);
                //pid.getEffect(c);

                var xx = pidX.getEffect(xp - x);
                if (xx > 0.5236f)
                    xx = 0.5236f;

                var yy = pidY.getEffect(y - yp);
                if (yy > 0.5236f)
                    yy = 0.5236f;


                yaw = pidYaw.getEffect(0 - v[2]);
                vel = pidAltitude.getEffect(high - c);
                roll = pidRoll.getEffect((yy - v[0]));
                pitch = pidPitch.getEffect(xx - v[1]);
                moveDriver(commonVelocity + vel, yaw, pitch, roll);
                var st = "Xtarg : " + xp.ToString() + nline + "Ytarg: " + yp.ToString() + nline + "Xc:" + x.ToString() + nline + "Yc:" + y.ToString() + nline;
                var str = st + "XX vel:" + xx.ToString() + nline + "YY vel:" + yy.ToString() + nline + "Yaw:" + yaw.ToString() + nline + "Roll: " + roll + nline + "Pitch" + pitch;//c.ToString() + Environment.NewLine + (vel + 5.35f).ToString() + Environment.NewLine + (high - c).ToString();
                var str2 = nline + nline + "Atitude:" + c.ToString() + nline + "Velocity: " + vel.ToString() + nline + "Real yaw :" + v[2].ToString() + nline + "Real pitch:" + v[1].ToString() + nline + "Real roll: " + v[0].ToString();
                SetDistanceToLable(textBox1, str + str2);
                //setPicture();

            }
            return 1;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (copter.getVrepController().isClientConnected())
            {
                copter.setVelocityToForceDriver(0, commonVelocity );
                copter.setVelocityToForceDriver(1, commonVelocity );
                copter.setVelocityToForceDriver(2, commonVelocity );
                copter.setVelocityToForceDriver(3, commonVelocity );
            }
        }

#endregion

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (workerThread.IsAlive)
                workerThread.Abort();
        }

        #region Основные алгоритмы

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(isWorking)
            {
                infoSystem.UpdateSensorDara();
                //mySensorData = copter.GetSensorData();
                textBox1.Text = infoSystem.MySensorData.Speed.X.ToString() + "    " + infoSystem.MySensorData.Speed.Y.ToString() + Environment.NewLine
                    + "X: " + infoSystem.MySensorData.Coordinates.X.ToString()
                    + "Y: " + infoSystem.MySensorData.Coordinates.Y.ToString()
                    + "Z: " + infoSystem.MySensorData.Coordinates.Z.ToString()
                    + Environment.NewLine + infoSystem.MySensorData.PrintResult()
                    + Environment.NewLine + GetProcessName();
                if(MyProcess == WhichProcess.Pitch)
                    textBox1.Text += Environment.NewLine + myC.dx.ToString();
                /* switch (MyProcess)
                 {
                     case WhichProcess.Nothing:
                         APNothingToDo();
                         break;
                     case WhichProcess.Height:
                         APReachHeight();
                         break;
                     case WhichProcess.Pitch:
                         APFlyStraight();
                         break;
                     case WhichProcess.Roll:
                         APFlySide();
                         break;
                     case WhichProcess.Yaw:
                         break;
                 }*/
                setPicture();
                GetPictureToPicBox(infoSystem.InformationFromCamera.bmp);
            }

           
            //infoSystem.DetectObjectFromData(GetPictureToPicBox);
            

        }

        void APNothingToDo()
        {
            //ЛА должен сохранять принятое положение в воздухе
            dvx = infoSystem.MySensorData.Speed.X;
            dvy = infoSystem.MySensorData.Speed.Y;

            //Превращение скоростей в требуемые углы поворота ЛА
            var dvelx = pidVX.getEffect(0 - dvx);
            if (dvelx > 0.5236f)
                dvelx = 0.5236f;

            var dvely = pidVY.getEffect(dvy - 0);
            if (dvely > 0.5236f)
                dvely = 0.5236f;

            // При нажатии на кнопку надо обнулять dvх и dvy
            var yaw = pidYaw.getEffect(infoSystem.MyReqPos.Yaw - infoSystem.MySensorData.Yaw);
            var vel = pidAltitude.getEffect(infoSystem.MyReqPos.Height - infoSystem.MySensorData.Height);
            var roll = pidRoll.getEffect(dvely + infoSystem.MyReqPos.Roll - infoSystem.MySensorData.Roll);
            var pitch = pidPitch.getEffect(dvelx + infoSystem.MyReqPos.Pitch - infoSystem.MySensorData.Pitch);
            moveDriver(commonVelocity + vel, yaw, pitch, roll);
            //MyProcess = WhichProcess.Nothing;
        }

        float dvx = 0;
        float dvy = 0;

        void APReachHeight()
        {
            //ЛА изменять высоту
            

            dvx += infoSystem.MySensorData.Speed.X;
            dvy += infoSystem.MySensorData.Speed.Y;
            
            //Превращение скоростей в требуемые углы поворота ЛА
            var dvelx = pidX.getEffect(0 - dvx);
            if (dvelx > 0.5236f)
                dvelx = 0.5236f;

            var dvely = pidY.getEffect(dvy - 0);// infoSystem.MyReqPos.Speed.Y);
            if (dvely > 0.5236f)
                dvely = 0.5236f;
            //dvelx = 0f;
            //dvely = 0f;
            // При нажатии на кнопку надо обнулять dvх и dvy
            var yaw = pidYaw.getEffect(infoSystem.MyReqPos.Yaw - infoSystem.MySensorData.Yaw);
            var vel = pidAltitude.getEffect(infoSystem.MyReqPos.Height - infoSystem.MySensorData.Height);
            var roll = pidRoll.getEffect(dvely + infoSystem.MyReqPos.Roll - infoSystem.MySensorData.Roll);
            var pitch = pidPitch.getEffect(dvelx + infoSystem.MyReqPos.Pitch - infoSystem.MySensorData.Pitch);
            moveDriver(commonVelocity + vel, yaw, pitch, roll);
            //MyProcess = WhichProcess.Nothing;
            if((Math.Abs((infoSystem.MySensorData.Height - infoSystem.MyReqPos.Height)) < 0.1f)&&(infoSystem.MySensorData.Speed.Z < 0.05f))
            {
                MyProcess = WhichProcess.Pitch;
                infoSystem.MyReqPos.Roll = 0;
                infoSystem.MyReqPos.Pitch = 0;
                infoSystem.MyReqPos.Yaw = 0;
                infoSystem.MyReqPos.Speed.X = 0;
                infoSystem.MyReqPos.Speed.Y = 0;
                infoSystem.MyReqPos.Speed.Z = 0;

                dvx = 0;
                dvy = 0;

                myC.dx = 0;

                infoSystem.MyReqPos.Speed.X = 0.5f;
            }
         
        }

        CoordinatesForFlyingStraight myC = new CoordinatesForFlyingStraight();

        void APFlyStraight()
        {
            //ЛА изменять высоту


            dvx = infoSystem.MySensorData.Speed.X;
            dvy = infoSystem.MySensorData.Speed.Y;

            myC.dx += dvx * 0.01f;

            //Превращение скоростей в требуемые углы поворота ЛА
            var dvelx = pidVX.getEffect(infoSystem.MyReqPos.Speed.X - dvx);
            if (dvelx > 0.5236f)
                dvelx = 0.5236f;

            var dvely = pidVY.getEffect(dvy - 0);
            if (dvely > 0.5236f)
                dvely = 0.5236f;
            //dvelx = 0f;
            //dvely = 0f;
            // При нажатии на кнопку надо обнулять dvх и dvy
            var yaw = pidYaw.getEffect(infoSystem.MyReqPos.Yaw - infoSystem.MySensorData.Yaw);
            var vel = pidAltitude.getEffect(infoSystem.MyReqPos.Height - infoSystem.MySensorData.Height);
            var roll = pidRoll.getEffect(dvely + infoSystem.MyReqPos.Roll - infoSystem.MySensorData.Roll);
            var pitch = pidPitch.getEffect(dvelx + infoSystem.MyReqPos.Pitch - infoSystem.MySensorData.Pitch);
            moveDriver(commonVelocity + vel + dvelx, yaw, pitch, roll);
            //MyProcess = WhichProcess.Nothing;

            if (myC.dx > 1f)
            {
                MyProcess = WhichProcess.Nothing;
            }

        }

        void APFlySide()
        {
            //ЛА изменять высоту


            dvx = infoSystem.MySensorData.Speed.X;
            dvy = infoSystem.MySensorData.Speed.Y;

            //Превращение скоростей в требуемые углы поворота ЛА
            var dvelx = pidVX.getEffect(0 - dvx);
            if (dvelx > 0.5236f)
                dvelx = 0.5236f;

            var dvely = pidVY.getEffect(dvy - infoSystem.MyReqPos.Speed.Y);
            if (dvely > 0.5236f)
                dvely = 0.5236f;
            //dvelx = 0f;
            //dvely = 0f;
            // При нажатии на кнопку надо обнулять dvх и dvy
            var yaw = pidYaw.getEffect(infoSystem.MyReqPos.Yaw - infoSystem.MySensorData.Yaw);
            var vel = pidAltitude.getEffect(infoSystem.MyReqPos.Height - infoSystem.MySensorData.Height);
            var roll = pidRoll.getEffect(infoSystem.MyReqPos.Roll - infoSystem.MySensorData.Roll);
            var pitch = pidPitch.getEffect(dvelx + infoSystem.MyReqPos.Pitch - infoSystem.MySensorData.Pitch);
            moveDriver(commonVelocity + vel + dvely, yaw, pitch, roll);
            //MyProcess = WhichProcess.Nothing;

        }

        #endregion

       


        #region Управление кнопками(События)
        private void button7_Click(object sender, EventArgs e)
        {
            MyProcess = WhichProcess.Height;
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = 0;
            infoSystem.MyReqPos.Speed.X = 0;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
            infoSystem.MyReqPos.Height += 0.5f;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MyProcess = WhichProcess.Height;
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = 0;
            infoSystem.MyReqPos.Speed.X = 0;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
            infoSystem.MyReqPos.Height -= 0.5f;
        }

        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            MyProcess = WhichProcess.Nothing;
            dvx = dvy = 0;
        }

        private void button8_MouseUp(object sender, MouseEventArgs e)
        {
            MyProcess = WhichProcess.Nothing;
            dvx = dvy = 0;
        }

        private void button9_MouseDown(object sender, MouseEventArgs e)
        {
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = -0.05f;
            infoSystem.MyReqPos.Speed.X = 0.10f;//-0.05f;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
            MyProcess = WhichProcess.Pitch;
            //myReqPos.Height -= 0.5f;
        }

        private void button9_MouseUp(object sender, MouseEventArgs e)
        {
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = 0f;
            infoSystem.MyReqPos.Speed.X = 0f;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
            dvx = 0;
            dvy = 0;
            MyProcess = WhichProcess.Nothing;
        }

        private void button13_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void button13_Click(object sender, EventArgs e)
        {
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = 0f;
            infoSystem.MyReqPos.Yaw += 0.05f;
            infoSystem.MyReqPos.Speed.X = 0f;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = 0f;
            infoSystem.MyReqPos.Yaw -= 0.05f;
            infoSystem.MyReqPos.Speed.X = 0f;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
        }

        private void button10_MouseDown(object sender, MouseEventArgs e)
        {
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = 0.05f;
            infoSystem.MyReqPos.Speed.X = -0.10f;//-0.05f;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
            MyProcess = WhichProcess.Pitch;

        }

        private void button10_MouseUp(object sender, MouseEventArgs e)
        {
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = 0f;
            infoSystem.MyReqPos.Speed.X = 0f;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
            dvx = 0;
            dvy = 0;
            MyProcess = WhichProcess.Nothing;
        }

        private void button11_MouseDown(object sender, MouseEventArgs e)
        {

            infoSystem.MyReqPos.Roll = -0.1f;
            infoSystem.MyReqPos.Pitch = 0;
            infoSystem.MyReqPos.Speed.X = 0;
            infoSystem.MyReqPos.Speed.Y = 0.05f;
            infoSystem.MyReqPos.Speed.Z = 0;
            MyProcess = WhichProcess.Roll;
            label5.Text = "Нажата кнопка";
        }

        private void button11_MouseUp(object sender, MouseEventArgs e)
        {
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = 0;
            infoSystem.MyReqPos.Speed.X = 0;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
            dvx = 0;
            dvy = 0;
            MyProcess = WhichProcess.Nothing;
            label5.Text = "Отжата кнопка";
        }

        private void button12_MouseDown(object sender, MouseEventArgs e)
        {
            infoSystem.MyReqPos.Roll = 0.1f;
            infoSystem.MyReqPos.Pitch = 0;
            infoSystem.MyReqPos.Speed.X = 0;
            infoSystem.MyReqPos.Speed.Y = -0.05f;
            infoSystem.MyReqPos.Speed.Z = 0;
            MyProcess = WhichProcess.Roll;
            label5.Text = "Нажата кнопка";
        }

        private void button12_MouseUp(object sender, MouseEventArgs e)
        {
            infoSystem.MyReqPos.Roll = 0;
            infoSystem.MyReqPos.Pitch = 0;
            infoSystem.MyReqPos.Speed.X = 0;
            infoSystem.MyReqPos.Speed.Y = 0;
            infoSystem.MyReqPos.Speed.Z = 0;
            dvx = 0;
            dvy = 0;
            MyProcess = WhichProcess.Nothing;
            label5.Text = "Отжата кнопка";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            /*if (MyProcess == WhichProcess.Pitch)
            {
                MyProcess = WhichProcess.Nothing;
                myReqPos.Roll = 0;
                myReqPos.Pitch = 0f;
                myReqPos.Speed.X = 0f;//-0.05f;
                myReqPos.Speed.Y = 0;
                myReqPos.Speed.Z = 0;
                dvx =  0;
                dvy = 0;
            }
            else
            {
                MyProcess = WhichProcess.Pitch;
                myReqPos.Roll = 0;
                myReqPos.Pitch = 0f;
                myReqPos.Speed.X = -0.05f;//-0.05f;
                myReqPos.Speed.Y = 0;
                myReqPos.Speed.Z = 0;

            }*/
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if(MyProcess == WhichProcess.Pitch)
            {
                MyProcess = WhichProcess.Nothing;
                infoSystem.MyReqPos.Roll = 0;
                infoSystem.MyReqPos.Pitch = 0f;
                infoSystem.MyReqPos.Speed.X = 0f;//-0.05f;
                infoSystem.MyReqPos.Speed.Y = 0;
                infoSystem.MyReqPos.Speed.Z = 0;
                dvx = 0;
                dvy = 0;
            }
            else
            {
                MyProcess = WhichProcess.Pitch;
                infoSystem.MyReqPos.Roll = 0;
                infoSystem.MyReqPos.Pitch = 0f;
                infoSystem.MyReqPos.Speed.X = 0.05f;//-0.05f;
                infoSystem.MyReqPos.Speed.Y = 0;
                infoSystem.MyReqPos.Speed.Z = 0;

            }
            
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                label5.Text = "Нажата W";
        }

        private void Form2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                label5.Text = "Отжата W";

        }
        #endregion
        #region Неиспользуемые функции

        void flyUpNewVersion()
        {
            var vrep = copter.getVrepController();
            float high = 1;
            var vel = 0f;
            var yaw = 0f;
            var pitch = 0f;
            var roll = 0f;
            var nline = Environment.NewLine;
            var vx = 0f;
            var vy = 0f;
            var c = vrep.getFloatSignal("z");
            vx = vrep.getFloatSignal("vx");
            vy = vrep.getFloatSignal("vy"); ;
            // c = copter.getVrepController().getFloatSignal("z");
            var xp = 0.5f;
            var yp = 0.5f;

            var quadrbase = vrep.ObjectHandle("Quadricopter_base");

            var e = high - c;
            float[] err = new float[3];
            for (int i = 0; i < 3; i++)
            {
                err[i] = e;
            }

            //Stopwatch sWatch = new Stopwatch();
            float xxx = 0f, yyy = 0f;
            int j = 0;
            float sum = e;
            //float f = 0;
            if (vrep.isClientConnected() && (sum > 0.1f))
            {
                // 
                c = vrep.getFloatSignal("z");
                var v = vrep.getObjectOrientation(quadrbase, -1);
                vx = vrep.getFloatSignal("vx");
                vy = vrep.getFloatSignal("vy");

                xxx += vx;
                yyy += vy;

                var xx = pidVX.getEffect(0 - xxx);
                if (xx > 0.5236f)
                    xx = 0.5236f;

                var yy = pidVY.getEffect(yyy - 0);
                if (yy > 0.5236f)
                    yy = 0.5236f;


                yaw = pidYaw.getEffect(0 - v[2]);
                vel = pidAltitude.getEffect(high - c);
                roll = pidRoll.getEffect((yy - v[0]));
                pitch = pidPitch.getEffect(xx - v[1]);
                moveDriver(commonVelocity + vel, yaw, pitch, roll);
                var st = "Velx: " + xxx.ToString() + nline + "Vely: " + yyy.ToString() + nline + nline;//"Xtarg : " + xp.ToString() + nline + "Ytarg: " + yp.ToString() + nline + "Xc:" + x.ToString() + nline + "Yc:" + y.ToString() + nline;
                var str = st + "XX vel:" + xx.ToString() + nline + "YY vel:" + yy.ToString() + nline + "Yaw:" + yaw.ToString() + nline + "Roll: " + roll + nline + "Pitch" + pitch;//c.ToString() + Environment.NewLine + (vel + 5.35f).ToString() + Environment.NewLine + (high - c).ToString();
                var str2 = nline + nline + "Atitude:" + c.ToString() + nline + "Velocity: " + vel.ToString() + nline + "Real yaw :" + v[2].ToString() + nline + "Real pitch:" + v[1].ToString() + nline + "Real roll: " + v[0].ToString();
                SetDistanceToLable(textBox1, str + str2);


                err[j] += high - c;
                j++;
                if (j > 2)
                    j = 0;


                for (int i = 0; i < err.Length; i++)
                    sum += err[i];
                sum = sum / err.Length;
                //setPicture();
            }
            SetDistanceToLable(textBox1, "Взлет закончен...");
        }

        void flyUp()
        {
            var vrep = copter.getVrepController();
            float high = 1;
            var vel = 0f;
            var yaw = 0f;
            var pitch = 0f;
            var roll = 0f;
            var nline = Environment.NewLine;
            var vx = 0f;
            var vy = 0f;
            var c = vrep.getFloatSignal("z");
            vx = vrep.getFloatSignal("vx");
            vy = vrep.getFloatSignal("vy"); ;
            // c = copter.getVrepController().getFloatSignal("z");
            var xp = 0.5f;
            var yp = 0.5f;

            var quadrbase = vrep.ObjectHandle("Quadricopter_base");

            var e = high - c;
            float[] err = new float[3];
            for (int i = 0; i < 3; i++)
            {
                err[i] = e;
            }

            //Stopwatch sWatch = new Stopwatch();
            float xxx = 0f, yyy = 0f;
            int j = 0;
            float sum = e;
            //float f = 0;
            while (vrep.isClientConnected() && (sum > 0.1f))
            {
                // 
                c = vrep.getFloatSignal("z");
                var v = vrep.getObjectOrientation(quadrbase, -1);
                vx = vrep.getFloatSignal("vx");
                vy = vrep.getFloatSignal("vy");

                xxx += vx;
                yyy += vy;

                var xx = pidVX.getEffect(0 - xxx);
                if (xx > 0.5236f)
                    xx = 0.5236f;

                var yy = pidVY.getEffect(yyy - 0);
                if (yy > 0.5236f)
                    yy = 0.5236f;


                yaw = pidYaw.getEffect(0 - v[2]);
                vel = pidAltitude.getEffect(high - c);
                roll = pidRoll.getEffect((yy - v[0]));
                pitch = pidPitch.getEffect(xx - v[1]);
                moveDriver(commonVelocity + vel, yaw, pitch, roll);
                var st = "Velx: " + xxx.ToString() + nline + "Vely: " + yyy.ToString() + nline + nline;//"Xtarg : " + xp.ToString() + nline + "Ytarg: " + yp.ToString() + nline + "Xc:" + x.ToString() + nline + "Yc:" + y.ToString() + nline;
                var str = st + "XX vel:" + xx.ToString() + nline + "YY vel:" + yy.ToString() + nline + "Yaw:" + yaw.ToString() + nline + "Roll: " + roll + nline + "Pitch" + pitch;//c.ToString() + Environment.NewLine + (vel + 5.35f).ToString() + Environment.NewLine + (high - c).ToString();
                var str2 = nline + nline + "Atitude:" + c.ToString() + nline + "Velocity: " + vel.ToString() + nline + "Real yaw :" + v[2].ToString() + nline + "Real pitch:" + v[1].ToString() + nline + "Real roll: " + v[0].ToString();
                SetDistanceToLable(textBox1, str + str2);


                err[j] += high - c;
                j++;
                if (j > 2)
                    j = 0;


                for (int i = 0; i < err.Length; i++)
                    sum += err[i];
                sum = sum / err.Length;
                //setPicture();
            }
            SetDistanceToLable(textBox1, "Взлет закончен...");
        }


        void PidRegulation()
        {
            float high = 1;
            var vel = 0f;
            var yaw = 0f;
            var pitch = 0f;
            var roll = 0f;
            var nline = Environment.NewLine;
            var x = 0f;
            var y = 0f;
            var c = copter.getVrepController().getFloatSignal("z");
            x = copter.getVrepController().getFloatSignal("x");
            y = copter.getVrepController().getFloatSignal("y"); ;
            // c = copter.getVrepController().getFloatSignal("z");
            var xp = 0.5f;
            var yp = 0.5f;



            //float f = 0;
            while (copter.getVrepController().isClientConnected())
            {
                /*PointXYZ p = cq.getCoordinatOfObj();
                f = p[2];
                var ff = p[1];
                //vel = pid.getEffect(high - f);
                //moveDriver(vel);*/
                var v = copter.getVrepController().getObjectOrientation(copter.getVrepController().ObjectHandle("Quadricopter_base"), -1);

                x = copter.getVrepController().getFloatSignal("x");
                y = copter.getVrepController().getFloatSignal("y"); ;
                c = copter.getVrepController().getFloatSignal("z");
                //vel = pid.getEffect(high - v[2]);
                //pid.getEffect(c);

                var xx = pidX.getEffect(xp - x);
                if (xx > 0.5236f)
                    xx = 0.5236f;

                var yy = pidY.getEffect(y - yp);
                if (yy > 0.5236f)
                    yy = 0.5236f;


                yaw = pidYaw.getEffect(0 - v[2]);
                vel = pidAltitude.getEffect(high - c);
                roll = pidRoll.getEffect((yy - v[0]));
                pitch = pidPitch.getEffect(xx - v[1]);
                moveDriver(commonVelocity + vel, yaw, pitch, roll);
                var st = "Xtarg : " + xp.ToString() + nline + "Ytarg: " + yp.ToString() + nline + "Xc:" + x.ToString() + nline + "Yc:" + y.ToString() + nline;
                var str = st + "XX vel:" + xx.ToString() + nline + "YY vel:" + yy.ToString() + nline + "Yaw:" + yaw.ToString() + nline + "Roll: " + roll + nline + "Pitch" + pitch;//c.ToString() + Environment.NewLine + (vel + 5.35f).ToString() + Environment.NewLine + (high - c).ToString();
                var str2 = nline + nline + "Atitude:" + c.ToString() + nline + "Velocity: " + vel.ToString() + nline + "Real yaw :" + v[2].ToString() + nline + "Real pitch:" + v[1].ToString() + nline + "Real roll: " + v[0].ToString();
                SetDistanceToLable(textBox1, str + str2);

            }
        }

        delegate void SetTellCallBack(System.Windows.Forms.Control control, string lab);

        void SetDistanceToLable(System.Windows.Forms.Control control, string lab)
        {
            if (control.InvokeRequired)
            {
                SetTellCallBack d = new SetTellCallBack(SetDistanceToLable);
                Invoke(d, new object[] { control, lab });
            }
            else
            {
                control.Text = lab;
            }
        }

        void moveDriver(float vel)
        {
            copter.setVelocityToForceDriver(0, vel);
            copter.setVelocityToForceDriver(1, vel);
            copter.setVelocityToForceDriver(2, vel);
            copter.setVelocityToForceDriver(3, vel);
        }
        void moveDriver(float vel, float yaw, float pitch, float roll)
        {
            copter.setVelocityToForceDriver(0, vel - yaw - pitch + roll);
            copter.setVelocityToForceDriver(1, vel + yaw + pitch + roll);
            copter.setVelocityToForceDriver(2, vel - yaw + pitch - roll);
            copter.setVelocityToForceDriver(3, vel + yaw - pitch - roll);
        }

        #endregion
    }


    public class CoordinatesForFlyingStraight
    {
        public float dx = 0;
    }
}