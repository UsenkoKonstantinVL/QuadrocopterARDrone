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

        enum WhichProcess { Nothing, Height, Pitch, Roll, Yaw }
        WhichProcess MyProcess = WhichProcess.Nothing;
        public Form2()
        {
            InitializeComponent();
            copter = new QuadrocopterController(7777);

            initializePID();

            cq = new CoordinatsOfObject(copter.getVrepController(), copter.getVrepController().ObjectHandle("Quadricopter"));//_base"));
            //PointXYZ point = new PointXYZ();
        }

        public void initializePID()
        {
            pidAltitude = new PID(0.08f, 0f, 5f);
            pidYaw = new PID(0.25f, 0f, 5f);
            pidRoll = new PID(0.25f, 0f, 5.000f);
            pidPitch = new PID(0.25f, 0f, 5.000f);
            pidX = new PID(0.1f, 0.0f, 1f);
            pidY = new PID(0.1f, 0.0f, 1f);
            pidVX = new PID(0.0005f, 0.0f, 0.08f);
            pidVY = new PID(0.0005f, 0.0f, 0.08f);

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
                mySensorData = new SensorData();

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
                pictureBox1.Image = copter.getDataFromISensor(0);
                //Thread.Sleep(25);
                //Thread.Sleep(10);
                //pictureBox2.Image = copter.getDataFromISensor(1);
                //Thread.Sleep(50);
            }
        }

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
            for(int i = 0; i<3; i++)
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

                var xx = pidVX.getEffect( 0-xxx);
                if (xx > 0.5236f)
                    xx = 0.5236f;

                var yy = pidVY.getEffect(yyy -0);
                if (yy > 0.5236f)
                    yy = 0.5236f;


                yaw = pidYaw.getEffect(0 - v[2]);
                vel = pidAltitude.getEffect(high - c);
                roll = pidRoll.getEffect((yy - v[0]));
                pitch = pidPitch.getEffect(xx - v[1]);
                moveDriver(commonVelocity + vel, yaw, pitch, roll);
                var st = "Velx: "+ xxx.ToString() + nline + "Vely: " + yyy.ToString() + nline + nline;//"Xtarg : " + xp.ToString() + nline + "Ytarg: " + yp.ToString() + nline + "Xc:" + x.ToString() + nline + "Yc:" + y.ToString() + nline;
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


                yaw =  pidYaw.getEffect(0 - v[2]);
                vel = pidAltitude.getEffect(high - c);
                roll =  pidRoll.getEffect((yy - v[0]));
                pitch =  pidPitch.getEffect(xx - v[1]);
                moveDriver(commonVelocity + vel, yaw, pitch, roll);
                var st = "Xtarg : " + xp.ToString() + nline + "Ytarg: " + yp.ToString()+nline + "Xc:" + x.ToString() + nline + "Yc:" + y.ToString() + nline;
                var str =  st + "XX vel:" + xx.ToString() + nline + "YY vel:" +yy.ToString() +nline+"Yaw:" + yaw.ToString() + nline + "Roll: " + roll + nline + "Pitch" + pitch;//c.ToString() + Environment.NewLine + (vel + 5.35f).ToString() + Environment.NewLine + (high - c).ToString();
                var str2 = nline + nline + "Atitude:" + c.ToString() + nline + "Velocity: " + vel.ToString() + nline + "Real yaw :"+ v[2].ToString() + nline + "Real pitch:" + v[1].ToString() + nline + "Real roll: " + v[0].ToString();
                SetDistanceToLable(textBox1, str+str2);

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
            copter.setVelocityToForceDriver(1, vel  + yaw + pitch +  roll);
            copter.setVelocityToForceDriver(2, vel - yaw + pitch - roll);
            copter.setVelocityToForceDriver(3, vel + yaw - pitch -  roll);
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

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (workerThread.IsAlive)
                workerThread.Abort();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(isWorking)
            {
                mySensorData = copter.GetSensorData();
                switch(MyProcess)
                {
                    case WhichProcess.Nothing:
                        APReachHeight();
                        break;
                    case WhichProcess.Height:
                        APReachHeight();
                        break;
                    case WhichProcess.Pitch:
                        break;
                    case WhichProcess.Roll:
                        break;
                    case WhichProcess.Yaw:
                        break;
                }
            }
            
        }

        void APNothingToDo()
        {
            //ЛА должен сохранять принятое положение в воздухе
            dvx += mySensorData.Speed.X;
            dvy += mySensorData.Speed.Y;

            //Превращение скоростей в требуемые углы поворота ЛА
            var dvelx = pidVX.getEffect(0 - dvx);
            if (dvelx > 0.5236f)
                dvelx = 0.5236f;

            var dvely = pidVY.getEffect(dvy - 0);
            if (dvely > 0.5236f)
                dvely = 0.5236f;

            // При нажатии на кнопку надо обнулять dvх и dvy
            var yaw = pidYaw.getEffect(myReqPos.Yaw - mySensorData.Yaw);
            var vel = pidAltitude.getEffect(myReqPos.Height - mySensorData.Height);
            var roll = pidRoll.getEffect(dvely - 0 - mySensorData.Roll);
            var pitch = pidPitch.getEffect(dvelx - 0 - mySensorData.Pitch);
            moveDriver(commonVelocity + vel, yaw, pitch, roll);
            MyProcess = WhichProcess.Nothing;
        }

        float dvx = 0;
        float dvy = 0;

        void APReachHeight()
        {
            //ЛА изменять высоту
            //var vrep = copter.getVrepController();
            //float high = 1;
            //var vel = 0f;
            //var yaw = 0f;
            //var pitch = 0f;
            //var roll = 0f;
            //var nline = Environment.NewLine;
            //var vx = 0f;
            //var vy = 0f;
            //var c = vrep.getFloatSignal("z");
            //vx = vrep.getFloatSignal("vx");
            //vy = vrep.getFloatSignal("vy"); ;
            //// c = copter.getVrepController().getFloatSignal("z");
            //var xp = 0.5f;
            //var yp = 0.5f;

            //var quadrbase = vrep.ObjectHandle("Quadricopter_base");

            //var e = high - c;
            //float[] err = new float[3];
            //for (int i = 0; i < 3; i++)
            //{
            //    err[i] = e;
            //}

            ////Stopwatch sWatch = new Stopwatch();
            //float xxx = 0f, yyy = 0f;
            //int j = 0;
            //float sum = e;
            ////float f = 0;
            
            //// 
            //c = vrep.getFloatSignal("z");
            //var v = vrep.getObjectOrientation(quadrbase, -1);
            //vx = vrep.getFloatSignal("vx");
            //vy = vrep.getFloatSignal("vy");

            dvx += mySensorData.Speed.X;
            dvy += mySensorData.Speed.Y;
            
            //Превращение скоростей в требуемые углы поворота ЛА
            var dvelx = pidVX.getEffect(0 - dvx);
            if (dvelx > 0.5236f)
                dvelx = 0.5236f;

            var dvely = pidVY.getEffect(dvy - 0);
            if (dvely > 0.5236f)
                dvely = 0.5236f;

            // При нажатии на кнопку надо обнулять dvх и dvy
            var yaw = pidYaw.getEffect(myReqPos.Yaw - mySensorData.Yaw);
            var vel = pidAltitude.getEffect(myReqPos.Height - mySensorData.Height);
            var roll = pidRoll.getEffect(dvely - myReqPos.Roll - mySensorData.Roll);
            var pitch = pidPitch.getEffect(dvelx - myReqPos.Pitch - mySensorData.Pitch);
            moveDriver(commonVelocity + vel, yaw, pitch, roll);
            MyProcess = WhichProcess.Nothing;
         
        }

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

        private void button7_Click(object sender, EventArgs e)
        {
            MyProcess = WhichProcess.Height;
            myReqPos.Roll = 0;
            myReqPos.Pitch = 0;
            myReqPos.Speed.X = 0;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
            myReqPos.Height += 0.5f;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MyProcess = WhichProcess.Height;
            myReqPos.Roll = 0;
            myReqPos.Pitch = 0;
            myReqPos.Speed.X = 0;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
            myReqPos.Height -= 0.5f;
        }

        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            /*MyProcess = WhichProcess.Nothing;
            dvx = dvy = 0;*/
        }

        private void button8_MouseUp(object sender, MouseEventArgs e)
        {
            /*MyProcess = WhichProcess.Nothing;
            dvx = dvy = 0;*/
        }

        private void button9_MouseDown(object sender, MouseEventArgs e)
        {
            myReqPos.Roll = 0;
            myReqPos.Pitch = 0.5f;
            myReqPos.Speed.X = 0;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
            //myReqPos.Height -= 0.5f;
        }

        private void button9_MouseUp(object sender, MouseEventArgs e)
        {
            myReqPos.Roll = 0;
            myReqPos.Pitch = 0f;
            myReqPos.Speed.X = 0f;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
        }

        private void button13_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void button13_Click(object sender, EventArgs e)
        {
            myReqPos.Roll = 0;
            myReqPos.Pitch = 0f;
            myReqPos.Yaw += 0.05f;
            myReqPos.Speed.X = 0f;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            myReqPos.Roll = 0;
            myReqPos.Pitch = 0f;
            myReqPos.Yaw -= 0.05f;
            myReqPos.Speed.X = -0.50f;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
        }

        private void button10_MouseDown(object sender, MouseEventArgs e)
        {
            myReqPos.Roll = 0;
            myReqPos.Pitch = -0.5f;
            myReqPos.Speed.X = 0.5f;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
        }

        private void button10_MouseUp(object sender, MouseEventArgs e)
        {
            myReqPos.Roll = 0;
            myReqPos.Pitch = 0;
            myReqPos.Speed.X = 0;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
        }

        private void button11_MouseDown(object sender, MouseEventArgs e)
        {
            myReqPos.Roll = 0.1f;
            myReqPos.Pitch = 0;
            myReqPos.Speed.X = 0;
            myReqPos.Speed.Y = -0.05f;
            myReqPos.Speed.Z = 0;
        }

        private void button11_MouseUp(object sender, MouseEventArgs e)
        {
            myReqPos.Roll = 0;
            myReqPos.Pitch = 0;
            myReqPos.Speed.X = 0;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
        }

        private void button12_MouseDown(object sender, MouseEventArgs e)
        {
            myReqPos.Roll = -0.1f;
            myReqPos.Pitch = 0;
            myReqPos.Speed.X = 0;
            myReqPos.Speed.Y = 0.05f;
            myReqPos.Speed.Z = 0;
        }

        private void button12_MouseUp(object sender, MouseEventArgs e)
        {
            myReqPos.Roll = 0;
            myReqPos.Pitch = 0;
            myReqPos.Speed.X = 0;
            myReqPos.Speed.Y = 0;
            myReqPos.Speed.Z = 0;
        }
    }
}