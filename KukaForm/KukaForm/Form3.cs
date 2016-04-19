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

namespace KukaForm
{
    public partial class Form3 : Form
    {
        #region Глобальные переменные
        QuadrocopterController copter;
        RequiredPosition myReqPos;
        SensorData mySensorData;



        enum WhichProcess { Nothing, Height, Pitch, Roll, Yaw , hover, FallDown}
        WhichProcess MyProcess = WhichProcess.Nothing;

        Form1 ControlForm;
        Form4 InformationForm;
        #endregion
        public Form3()
        {
            InitializeComponent();
            initial();
        }

        void initial()
        {
            MyProcess = WhichProcess.Height;
            copter = new QuadrocopterController(7777);
            myReqPos = new RequiredPosition();
            mySensorData = new SensorData();
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
                

                button1.Text = "Стоп";



                myReqPos = new RequiredPosition();
                myReqPos.Height = 1f;
                
                mySensorData = new SensorData();
                mySensorData = copter.GetSensorData();
                mySensorData = copter.GetSensorData();
                myReqPos.Yaw = mySensorData.Yaw;

                timer1.Enabled = true;

                ConditionGetHeight();

                InformationForm = new Form4(copter);
                ControlForm = new Form1(InformationForm, copter, myReqPos);

                InformationForm.Show();
                ControlForm.Show();

                button2.Visible = true;
            }
            else
            {
                button1.Text = "Старт";
                copter.Finish();
                timer1.Enabled = false;
                CloseAllWindows();
                button2.Visible = false;
                //закрывает все созданные окна
                //Добавить, чтобы закрывал окна
            }
        }

        void CloseAllWindows()
        {
            ;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetInformationFromCopter();
            switch (MyProcess)
            {
                case WhichProcess.Nothing:
                    ConditionHoverPosition();
                    break;
                case WhichProcess.Height:
                    ConditionGetHeight();
                    break;
                case WhichProcess.Pitch:
                    APFlyStraight();
                    break;
                //case WhichProcess.Roll:
                //    APFlySide();
                //    break;

                case WhichProcess.Yaw:
                    ConditionYawMoving();
                    break;

                case WhichProcess.hover:
                    HoverPosition();
                    break;

                case WhichProcess.FallDown:
                    GoingDown();
                    break;

            }
            //SetRequareMoving();
        }


        #region Обрабатываем данные с коптера и составляем уставки
        void ConditionGetHeight()
        {
            /* myReqPos.Pitch = 0;
             myReqPos.Roll = 0;
             myReqPos.Yaw = mySensorData.Yaw;
             myReqPos.Position.X = myReqPos.Position.Y = 0;
             myReqPos.Speed = new Points3(0, 0, 0);
             myReqPos.Height = 1;*/
            textBox1.Text = "Getting Height: " + myReqPos.Height.ToString();
           
           if ((Math.Abs((mySensorData.Height - myReqPos.Height)) < 0.1f) && (Math.Abs(mySensorData.Speed.Z )< 0.05f))
            {

                /*setMoveForward();
                MyProcess = WhichProcess.Pitch;*/
                HoldPosition();

            }
        }

       

        float dx = 0;
        float dy = 0;
        int i = 0;
        void APFlyStraight()
        {
            textBox1.Text = "Fly straight: " + myReqPos.Speed.X.ToString();
            dx += mySensorData.Speed.X;
            dx += mySensorData.Speed.Y;
            if (i == 1)
            {
                if (Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)) > 5f)
                {
                    HoldPosition();
                    dx = 0;
                    dy = 0;
                }
            }
            if (i == 2)
            {
                if (Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)) > 9f)
                {
                    HoldPosition();
                    dx = 0;
                    dy = 0;
                }
            }
            else if(i == 3)
            {
                if (Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)) > 20f)
                {
                    HoldPosition();
                    dx = 0;
                    dy = 0;
                }
            }
        }

        void GoingDown()
        {
            if (Math.Abs((mySensorData.Height - 0)) < 0.5f && Math.Abs((mySensorData.Height - 0)) < 0.1f)
            {
                FallDown();
                textBox1.Text = "Fall down";
            }
            else if (Math.Abs((mySensorData.Height - 0)) < 0.1f)
            {
                Offcopter();
                textBox1.Text = "Off";
            }
        }

        void HoverPosition()
        {
            textBox1.Text = "Hover Position";
            if ((Math.Abs(mySensorData.Speed.X) < 0.005f) && (Math.Abs(mySensorData.Speed.Y) < 0.005f) && (Math.Abs(mySensorData.Speed.Z) < 0.05f))
            {
                if (i == 0)
                {
                    //setMoveForward();
                    setYaw(0.4f);
                    i++;
                }
                else if(i == 1)
                {
                    //setReqHeight(0.8f);// setMoveForward();
                    setYaw(0.8f);
                    i++;
                }
                else if (i == 2)
                {
                    //setMoveForward();
                    setYaw(1f);
                    i++;
                }
                else if (i == 3)
                {
                    //setReqHeight(0.5f);
                    setYaw(1.2f);
                    i++;
                }
                else if(i == 4)
                {
                    setReqHeightDown();

                }
            }

        }
        void ConditionYawMoving()
        {
            textBox1.Text = "Getting Yaw: " + myReqPos.Yaw;
            if((Math.Abs(mySensorData.Yaw - myReqPos.Yaw) < 0.01f))
            {
                HoldPosition();
            }
        }

        void setReqHeight(float h)
        {
            MyProcess = WhichProcess.Height;
            myReqPos.Height = h;
            /*myReqPos.Roll = 0;
            myReqPos.Speed.X = 0f;
            myReqPos.Speed.Y = 0;
            myReqPos.Yaw = 0f;
            myReqPos.Position.X = myReqPos.Position.Y = 0;
            myReqPos.Speed = new Points3(0f, 0f, 0);*/
            SetRequareMoving(WhichControlToUse.PositionControl);
        }

        void setMoveForward()
        {
            MyProcess = WhichProcess.Pitch;
            myReqPos.Pitch = 0.008f;
            myReqPos.Roll = 0;
            //myReqPos.Yaw = 0f;
            myReqPos.Position.X = myReqPos.Position.Y = 0;
            myReqPos.Speed = new Points3(0.001f, 0f, 0);
            SetRequareMoving(WhichControlToUse.MoveForward);
        }

        void setReqHeightDown()
        {
            MyProcess = WhichProcess.FallDown;
            myReqPos.Height = 0;
            /*myReqPos.Roll = 0;
            myReqPos.Speed.X = 0f;
            myReqPos.Speed.Y = 0;
            myReqPos.Yaw = 0f;
            myReqPos.Position.X = myReqPos.Position.Y = 0;
            myReqPos.Speed = new Points3(0f, 0f, 0);*/
            SetRequareMoving(WhichControlToUse.PositionControl);
        }

        void setYaw(float y)
        {
            MyProcess = WhichProcess.Yaw;
            myReqPos.Pitch = 0;
            myReqPos.Roll = 0;
            myReqPos.Yaw = y;
            myReqPos.Position.X = myReqPos.Position.Y = 0;
            myReqPos.Speed = new Points3(0, 0, 0);
            SetRequareMoving(WhichControlToUse.YawControl);
        }

       
        void ConditionHoverPosition()
        {
            ;
        }

        

        void Offcopter()
        {
            myReqPos.Pitch = 0.00f;
            myReqPos.Roll = 0;
            //myReqPos.Yaw = 0f;
            myReqPos.Position.X = myReqPos.Position.Y = 0;
            myReqPos.Speed = new Points3(0.00f, 0f, 0);
            SetRequareMoving(WhichControlToUse.Off);
        }

        void FallDown()
        {
            //MyProcess = WhichProcess.Pitch;
            myReqPos.Pitch = 0.00f;
            myReqPos.Roll = 0;
            //myReqPos.Yaw = 0f;
            myReqPos.Position.X = myReqPos.Position.Y = 0;
            myReqPos.Speed = new Points3(0.00f, 0f, 0);
            SetRequareMoving(WhichControlToUse.Fall);
        }

        void HoldPosition()
        {
            SetRequareMoving(WhichControlToUse.Hover);
            MyProcess = WhichProcess.hover;
        }




        #endregion



        #region база условий смены состояния
            void ConditionHeight()
        {

        }

        void ConditionYaw()
        {

        }

        void ConditionHover()
        {

        }

        void ConditionStraightFly()
        {

        }
        #endregion


        #region вспомогательные функции
        Setpoint GetSetpoint(WhichProcess wp, float h, float y)
        {
            Setpoint sp = new Setpoint();
            /*sp.currentProcces = wp;
            sp.height = h;
            sp.yaw = y;*/
            return sp;
        }
        #endregion
        #region
        void GetInformationFromCopter()
        {
            mySensorData = InformationForm.GetDataFromCopter();
        }

        void SetRequareMoving()
        {
            ControlForm.GetRequareMoving(myReqPos);
        }
        void SetRequareMoving(WhichControlToUse wc)
        {
            ControlForm.GetRequareMoving(myReqPos, wc);
        }

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            myReqPos = new RequiredPosition();
            myReqPos.Height = 1f;

            mySensorData = new SensorData();
            mySensorData = copter.GetSensorData();
            mySensorData = copter.GetSensorData();
            myReqPos.Yaw = mySensorData.Yaw;
            ControlForm.StartModelToWork(WhichControlToUse.PositionControl, myReqPos);
        }
    }


    class Setpoint
    {
        public enum WhichProcess { Nothing, Height, Pitch, Roll, Yaw, hover, FallDown }
        public WhichProcess currentProcces;
        public float height;
        public float yaw;
    }
}
