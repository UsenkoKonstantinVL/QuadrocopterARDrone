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

        WhichProcess MyProcess = WhichProcess.Nothing;
        FollowingLine FollowingLineCondition;

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



                /*myReqPos = new RequiredPosition();
                myReqPos.Height = 0.3f;*/
                
                mySensorData = new SensorData();
                mySensorData = copter.GetSensorData();
                mySensorData = copter.GetSensorData();
                myReqPos.Yaw = mySensorData.Yaw;

                

                InformationForm = new Form4(copter);
                ControlForm = new Form1(InformationForm, copter, myReqPos);

                InformationForm.Show();
                ControlForm.Show();


                //timer1.Enabled = true;

                //InitializeSets();


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

        List<Setpoint> mySets = new List<Setpoint>();
        int CurrentSets = 0;

        void InitializeSets()
        {
            Setpoint sp = new Setpoint();
            sp.currentProcces = WhichProcess.Height;
            sp.height = 0.35f;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.Hover;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.PitchRoll;
            mySets.Add(sp);

            /*sp = new Setpoint();
            sp.currentProcces = WhichProcess.Yaw;
            sp.yaw = 0.5f;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.Height;
            sp.height = 1;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.Yaw;
            sp.yaw = 0.8f;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.Hover;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.Height;
            sp.height = 0.5f;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.Hover;
            mySets.Add(sp);*/
            CurrentSets = 0;

        }

        void ChangeSets()
        {
            if(CurrentSets < (mySets.Count-1))
            {
                CurrentSets++;
                PrepareToMoving();
            }
               
        }

        void PrepareToMoving()
        {
            switch (mySets[CurrentSets].currentProcces)
            {
                case WhichProcess.PitchRoll: PrepareToPitchRoll(); break;
            }
        }

        #region Prepare region

        void PrepareToPitchRoll()
        {
            FollowingLineCondition = FollowingLine.SearchLine;
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetInformationFromCopter();
            switch (mySets[CurrentSets].currentProcces)
            {
                case WhichProcess.Nothing:
                    AlgorythmHoverPosition();
                    ConditionHover();
                    break;
                case WhichProcess.Height:
                    AlgorythmGetHeight();
                    ConditionHeight();
                    break;
                case WhichProcess.Pitch:
                    AlgorithmGetPitch();
                    break;
                case WhichProcess.PitchRoll:
                    AlgorythmPitchRoll();
                    ConditionPitchRoll();
                    break;

                case WhichProcess.Yaw:
                    AlgorythmGetYaw();
                    ConditionYaw();
                    break;

                case WhichProcess.Hover:
                    AlgorythmHoverPosition();
                    ConditionHover();
                    break;

                case WhichProcess.FallDown:
                    AlgorythmGoingDown();
                    break;

            }
            
        }


        #region База алгоритмов

        void ConditionHoverPosition()
        {
            ;
        }

        void AlgorythmGetHeight()
        {
            myReqPos.Height = mySets[CurrentSets].height;
            textBox1.Text = "Getting Height: " + myReqPos.Height.ToString();

            SetRequareMoving(WhichControlToUse.PositionControl);


        }


        #region Varaibles for Alg get pitch
        float dx = 0;
        float dy = 0;
        int i = 0;
        #endregion
        void AlgorithmGetPitch()
        {
            
        }

        #region Varaibles for alg pitch and roll
        int icycles = -1;
        bool workCompleted = false;
        double millis = 0;
        #endregion
        void AlgorythmPitchRoll()
        {
            #region notUsen
            ////double millis = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            //double timeToFly = 10000;


            //if (icycles == -1)
            //{
            //    millis = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            //    icycles++;

            //}

            //else if (icycles == 0)
            //{
            //    myReqPos.Pitch = 0.05f;
            //    myReqPos.Roll = 0.05f;

            //    SetRequareMoving(WhichControlToUse.PitchRoll);

            //    if(((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds - millis) > timeToFly)
            //    {
            //        icycles++;
            //        millis = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            //    }
            //}
            //else if(icycles == 1)
            //{
            //    myReqPos.Pitch = 0.05f;
            //    myReqPos.Roll = -0.05f;

            //    SetRequareMoving(WhichControlToUse.PitchRoll);
            //    if (((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds - millis) > timeToFly)
            //    {
            //        icycles = -1;
            //        workCompleted = true;
            //    }
            //}
            #endregion
            textBox1.Text = "Line following...";
            SearchLine();/*
            switch (FollowingLineCondition)
            {
                case FollowingLine.SearchLine:
                    SearchLine();
                    break;
                case FollowingLine.GetLineToCenter:
                    GetLineToCenter();
                    break;
                case FollowingLine.Rotation:
                    Rotation();
                    break;
                case FollowingLine.MovementOnLine:
                    MovementOnLine();
                    break;
            }*/
        }

        #region Helpful func for PitchRoll

        void SearchLine()
        {
            

            double zone1 = 0, zone2 = 0, zone3 = 0, zone4 = 0;
            double zonecenter = 0;


            zonecenter = InformationForm.GetZonePercentage(DetectedZone.Center);

            // определение значений каждой зоны

            //
            if (zonecenter == 0)
            {
                textBox1.Text += Environment.NewLine + "Try to alighn to line...";
                Align();

                #region Forget and delete
                //if ((zone1 > 0)||(zone2 > 0))
                //{
                //    #region Zone 1 and 2
                //    if (zone1 > 0)
                //    {
                //        //летим вперед
                //    }
                //    else
                //    {
                //        //летим назад
                //    }
                //    #endregion
                //}
                //else if((zone3 > 0) || (zone4 > 0))
                //{
                //    #region Zone 3 and 4
                //    if (zone3 > 0)
                //    {
                //        //летим вправо
                //    }
                //    else
                //    {
                //        //летим влево
                //    }
                //    #endregion
                //}
                //else
                //{
                //    //ищим линию
                //}

                #endregion

            }
            else
            {
                textBox1.Text += Environment.NewLine + "Try to Follow  line...";
                //Moving();
                //выбираем другой алгоритм движения
                //FollowingLineCondition = FollowingLine.MovementOnLine;
            }
        }

        void Align()
        {
            double zone1 = 0, zone2 = 0, zone3 = 0, zone4 = 0;

            zone1 = InformationForm.GetZonePercentage(DetectedZone.Zone1);
            zone2 = InformationForm.GetZonePercentage(DetectedZone.Zone2);
            zone3 = InformationForm.GetZonePercentage(DetectedZone.Zone3);
            zone4 = InformationForm.GetZonePercentage(DetectedZone.Zone4);

            double threshhold = 0.05;

            float pitch = 0;
            float roll = 0;

            float dpitch = 0.01f;
            float droll = 0.01f;

            if(zone1 > threshhold)
            {
                textBox1.Text += Environment.NewLine + "First rule works...";
                roll += droll;
                pitch -= dpitch;
            }

            if(zone2 > threshhold)
            {
                textBox1.Text += Environment.NewLine + "Second rule works...";
                roll += droll;
                pitch += dpitch;
            }

            if(zone3 > threshhold)
            {
                textBox1.Text += Environment.NewLine + "Third rule works...";
                roll += -droll;
                pitch += dpitch;
            }
            if(zone4 > threshhold)
            {
                textBox1.Text += Environment.NewLine + "Fouth rule works...";
                roll += droll;
                pitch += dpitch;
            }


            SetUpRequre(roll, pitch);
            //задать воздействие
        }

        void SetUpRequre(float roll, float pitch)
        {
            myReqPos.Pitch = pitch;
            myReqPos.Roll = roll;

            SetRequareMoving(WhichControlToUse.PitchRoll);
        
        }

        void Moving()
        {
            double zoneTop = 0, zoneTopLeft = 0, zoneTopRight = 0, zoneRight = 0, zoneLeft = 0, zonecenter = 0;
            double tolerance = 0;
            ///


            ///

            float pitch = 0, roll = 0, yaw = 0;

            float dyaw = 0;
            float dpitch = 0, droll = 0;

            if(zoneTop > tolerance)
            {
                pitch -= dpitch;
            }

            if(zoneTopRight > tolerance)
            {
                yaw -= dyaw;
            }

            if(zoneTopLeft > tolerance)
            {
                yaw += dyaw;
            }

            if(zoneRight > tolerance)
            {
                roll -= droll;
            }

            if(zoneLeft > tolerance)
            {
                roll += droll;
            }
        }

        void SetUpRequre(float roll, float pitch, float yaw)
        {
            myReqPos.Pitch = pitch;
            myReqPos.Roll = roll;
            myReqPos.Yaw = myReqPos.Yaw + yaw;

            SetRequareMoving(WhichControlToUse.PitchRoll);

        }

        void Rotation()
        {
            double zone1 = 0, zone2 = 0, zone3 = 0, zone4 = 0, zone5 = 0, zonecenter = 0;

            if((zone1 > zone2)||(zone3>zone2))
            {
                if((zone1 > zone2))
                {
                    //поворачиваемся влево
                }
                else
                {
                    //поворачиваемся вправо
                }
            }
            else if((zone4 != 0) || (zone5 != 0))
            {
                if ((zone4 > zone5))
                {
                    //поворачиваемся влево
                }
                else
                {
                    //поворачиваемся вправо
                }
            }
            else
            {
                //ищим линию
            }


        }

        void MovementOnLine()
        {
        }

        void GetLineToCenter()
        {

        }

        #endregion

        void AlgorythmGetYaw()
        {
            textBox1.Text = "Getting Yaw: " + myReqPos.Yaw;
            myReqPos.Yaw = mySets[CurrentSets].yaw;

            SetRequareMoving(WhichControlToUse.YawControl);

        }

        void AlgorythmHoverPosition()
        {
            textBox1.Text = "Hover Position";
            SetRequareMoving(WhichControlToUse.Hover);
        }

        void AlgorythmGoingDown()
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

        #endregion

        #region Unused func
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
            MyProcess = WhichProcess.Hover;
        }

        #endregion



        #region база условий смены состояния
        void ConditionHeight()
        {
            if ((Math.Abs((mySensorData.Height - myReqPos.Height)) < 0.05f) && (Math.Abs(mySensorData.Speed.Z) < 0.05f))
            {

                ChangeSets();

            }
        }

        void ConditionYaw()
        {
            if ((Math.Abs(mySensorData.Yaw - myReqPos.Yaw) < 0.01f))
            {
                ChangeSets();
            }
        }

        void ConditionHover()
        {
            if ((Math.Abs((mySensorData.Speed.Z)) < 0.05f) && (Math.Abs((mySensorData.Speed.X)) < 0.05f) && (Math.Abs((mySensorData.Speed.Y)) < 0.05f) && (Math.Abs((mySensorData.AngularSpeed.Z)) < 0.05f))
            {

                ChangeSets();

            }
        }

        void ConditionStraightFly()
        {

        }

        void ConditionPitchRoll()
        {
            if(workCompleted)
            {
                myReqPos.Pitch = 0f;
                myReqPos.Roll = 0f;
                ChangeSets();
                workCompleted = false;
            }
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
            timer1.Enabled = true;

            InitializeSets();
        }
    }


    public class Setpoint
    {
        public WhichProcess currentProcces;
        public float height;
        public float yaw;
    }
}
