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
    public partial class TacticLevel : Form
    {

        #region Глобальные переменные
        QuadrocopterController copter;
        RequiredPosition myReqPos;
        SensorData mySensorData;

        WhichProcess MyProcess = WhichProcess.Nothing;
        FollowingLine FollowingLineCondition;

        Form1 ControlForm;
        Form4 InformationForm;
        StrategLevel Form1;

        List<Setpoint> mySets = new List<Setpoint>();
        int CurrentSets = 0;


        string CurrentOperation = "";
        #endregion


        public TacticLevel(List<Setpoint> s, StrategLevel st)
        {
            InitializeComponent();
            initial();
            timer1.Enabled = true;
            mySets = s;
        }

        public TacticLevel()
        {
            InitializeComponent();
            initial();
            timer1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        public void  GetCommand(Setpoint currentPoint)
        {
            mySets.Add(currentPoint);
        }
       
        

        void initial()
        {
            MyProcess = WhichProcess.Height;
            copter = new QuadrocopterController(7777);
            myReqPos = new RequiredPosition();
            mySensorData = new SensorData();


            copter.Start();
            copter.AddDrive("Quadricopter_propeller1", "dVelocity1");
            copter.AddDrive("Quadricopter_propeller2", "dVelocity2");
            copter.AddDrive("Quadricopter_propeller3", "dVelocity3");
            copter.AddDrive("Quadricopter_propeller4", "dVelocity4");


            copter.AddSensor("Vision_sensor1");
            copter.AddSensor("Visionfront_sensor");


           

            mySensorData = new SensorData();
            mySensorData = copter.GetSensorData();
            mySensorData = copter.GetSensorData();
            myReqPos.Yaw = mySensorData.Yaw;



            InformationForm = new Form4(copter);
            ControlForm = new Form1(InformationForm, copter, myReqPos);

            InitializeSets();

            InformationForm.Show();
            ControlForm.Show();
        }


        

        void InitializeSets()
        {
            Setpoint sp = new Setpoint();
            sp.currentProcces = WhichProcess.Height;
            sp.height = 0.3f;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.Hover;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.PitchRoll;
            mySets.Add(sp);

            CurrentSets = 0;

        }

        Setpoint CurrentSet;

        void ChangeSets()
        {
            CurrentSets++;
            if (CurrentSets < (mySets.Count))
            {
                CurrentSet = mySets[CurrentSets];
                PrepareToMoving();
            }
            else if(CurrentSets == (mySets.Count))
            {
                CurrentSet = new Setpoint();
                CurrentSet.currentProcces = WhichProcess.Hover;
               
            }
            else
            {
                CurrentSets--;
                complete();
            }

        }


        void complete()
        {

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

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            GetInformationFromCopter();
            switch (CurrentSet.currentProcces)
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

                case WhichProcess.Coordinates:
                    AlgorithmForReachCoordinates();
                    ConditionCoordinates();
                    break;

            }
            SetTextBoxes();
        }


        void SetTextBoxes()
        {
            label1.Text = "Текущая операция:" + CurrentOperation;
        }


        #region База алгоритмов

        void ConditionHoverPosition()
        {
            ;
        }

        void AlgorythmGetHeight()
        {
            myReqPos.Height = CurrentSet.height;
            CurrentOperation = "Getting Height: " + myReqPos.Height.ToString();
            //textBox1.Text = "Getting Height: " + myReqPos.Height.ToString();

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
            double zonefull = InformationForm.GetZonePercentage(DetectedZone.All);
            if (zonefull > 0.0001f)
            {
                    Control();
            }
            else
            {
                SetRequareMoving(WhichControlToUse.Hover);
                myReqPos.Pitch = 0;
                myReqPos.Roll = 0;
                CurrentOperation = "Follow line. Can't find  line. I'm hovering";
                //textBox1.Text += Environment.NewLine + "Can't find  line...";

            }
        }

        #region Helpful func for PitchRoll

        void SearchLine()
        {


            double zone1 = 0, zone2 = 0, zone3 = 0, zone4 = 0;
            double zonecenter = 0;
            double zonefull = InformationForm.GetZonePercentage(DetectedZone.All);


            zonecenter = InformationForm.GetZonePercentage(DetectedZone.Center);

            // определение значений каждой зоны

            //

            myReqPos.Speed.Z = 0;

            if (zonefull > 0.0001f)
            {
                if (zonecenter == 0)
                {
                    textBox1.Text += Environment.NewLine + "Try to alighn to line...";
                    //SetRequareMoving(WhichControlToUse.Hover);
                    //myReqPos.Pitch = 0;
                    //myReqPos.Roll = 0;
                    //Align();
                    Control();

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
                    //SetRequareMoving(WhichControlToUse.Hover);
                    //myReqPos.Pitch = 0;
                    //myReqPos.Roll = 0;
                    Control();
                    // Moving();
                    //выбираем другой алгоритм движения
                    //FollowingLineCondition = FollowingLine.MovementOnLine;
                }
            }
            else
            {
                SetRequareMoving(WhichControlToUse.Hover);
                myReqPos.Pitch = 0;
                myReqPos.Roll = 0;
                textBox1.Text += Environment.NewLine + "Can't find  line...";
                //MoveToLineZone();
            }
        }

        void Control()
        {
            var zoneTopLeft = InformationForm.GetZonePercentage(DetectedZone.TopLeft);
            var zoneTopRight = InformationForm.GetZonePercentage(DetectedZone.TopRight);

            var zoneLeft = InformationForm.GetZonePercentage(DetectedZone.Left);
            var zoneRight = InformationForm.GetZonePercentage(DetectedZone.Right);
            var zoneBottomLeft = InformationForm.GetZonePercentage(DetectedZone.BottomLeft);
            var zoneBottomRight = InformationForm.GetZonePercentage(DetectedZone.BottomRight);
            var zoneBottom = InformationForm.GetZonePercentage(DetectedZone.Bottom);

            var zoneCenter = InformationForm.GetZonePercentage(DetectedZone.Center);

            var zoneTop = InformationForm.GetZonePercentage(DetectedZone.Top);

            float yaw = 0;
            float roll = 0;
            float pitch = 0;


            float tolerance = 0.01f;


            if (zoneCenter < tolerance)
            {
                if ((zoneTopLeft > tolerance) || (zoneLeft > tolerance) || (zoneBottomLeft > tolerance))
                {

                    roll += -0.001f;
                }
                if ((zoneTopRight > tolerance) || (zoneRight > tolerance) || (zoneBottomRight > tolerance))
                {

                    roll += 0.001f;
                }

                if ((zoneTopLeft > tolerance) || (zoneTop > tolerance) || (zoneTopRight > tolerance))
                {

                    pitch += 0.001f;
                }
                if ((zoneBottomLeft > tolerance) || (zoneBottom > tolerance) || (zoneBottomRight > tolerance))
                {

                    pitch += -0.001f;
                }
            }
            else
            {
                if (zoneTop > tolerance)
                {
                    pitch = 0.0005f;
                }

                if ((zoneTopLeft > tolerance) && (zoneTop < tolerance))
                {
                    yaw = -0.1f;
                }
                else if ((zoneTopRight > tolerance) && (zoneTop < tolerance))
                {
                    yaw = 0.1f;
                }

                if ((zoneLeft > tolerance) && (zoneTop < tolerance))
                {
                    yaw = -0.1f;
                }
                else if ((zoneRight > tolerance) && (zoneTop < tolerance))
                {
                    yaw = 0.1f;
                }
            }

            SetRequareMoving(WhichControlToUse.PitchRoll);
            myReqPos.Speed.Z = yaw;
            myReqPos.Roll = roll;
            myReqPos.Pitch = pitch;

            //textBox1.Text += Environment.NewLine + "Control yaw... " + yaw.ToString();
            //textBox1.Text += Environment.NewLine + "Control roll... " + roll.ToString();
            //textBox1.Text += Environment.NewLine + "Control pitch... " + pitch.ToString();

            //textBox1.Text += Environment.NewLine + zoneTopLeft.ToString() + " " + zoneTop.ToString() + " " + zoneTopRight.ToString();
            //textBox1.Text += Environment.NewLine + zoneLeft.ToString() + " " + zoneCenter.ToString() + " " + zoneRight.ToString();
            //textBox1.Text += Environment.NewLine + zoneBottomLeft.ToString() + " " + zoneBottom.ToString() + " " + zoneBottomRight.ToString();
        }

        #endregion

        void AlgorythmGetYaw()
        {
            CurrentOperation = "Getting Yaw: " + myReqPos.Yaw;
            //textBox1.Text = "Getting Yaw: " + myReqPos.Yaw;
            myReqPos.Yaw = CurrentSet.yaw;

            SetRequareMoving(WhichControlToUse.YawControl);

        }

        void AlgorythmHoverPosition()
        {
            textBox1.Text = "Hover Position";
            SetRequareMoving(WhichControlToUse.Hover);
        }

        void AlgorythmGoingDown()
        {
            //if (Math.Abs((mySensorData.Height - 0)) < 0.5f && Math.Abs((mySensorData.Height - 0)) < 0.1f)
            //{
            //    FallDown();
            //    textBox1.Text = "Fall down";
            //}
            //else if (Math.Abs((mySensorData.Height - 0)) < 0.1f)
            //{
            //    Offcopter();
            //    textBox1.Text = "Off";
            //}
        }


        void AlgorithmForReachCoordinates()
        {
            SetPointCoordinates set = (SetPointCoordinates)CurrentSet;

            if (!isNear(set.Coordinates[0], mySensorData.Coordinates, 0.5f))
            {
                set.Coordinates.RemoveAt(0);
            }

            if (set.Coordinates.Count != 0)
            {
                var pos = set.Coordinates[0];
                var dangle = GetAngle(mySensorData.Coordinates, set.Coordinates[0], mySensorData.Yaw);
                var lenght = Sqrt(mySensorData.Coordinates, set.Coordinates[0]);

                float vang = 0;
                float pitch = 0;


                if (Math.Abs(dangle) > 0.01f)
                {
                    if (dangle > 0)
                    {
                        vang = 0.5f;
                    }
                    else
                    {
                        vang = -0.5f;
                    }

                }
                else
                {
                    pitch = -0.01f;
                }

                CurrentOperation = "Fly to Coordinate " + set.Coordinates[0].X.ToString() + " " + set.Coordinates[0].Y.ToString();
                //textBox1.Text = "Fly to Coordinate " + set.Coordinates[0].X.ToString() + " " + set.Coordinates[0].Y.ToString();
                SetRequareMoving(WhichControlToUse.CoordinateControl);
                myReqPos.Speed.Z = vang;
                myReqPos.Roll = 0;
                myReqPos.Pitch = pitch;
            }
        }

        bool isNear(Points3 p1, Points3 p2, float n)
        {
            var r = Sqrt(p1, p2);



            return (r < n);
        }

        float Sqrt(Points3 p1, Points3 p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        float GetAngle(Points3 copter, Points3 target, float anglecopter)
        {
            var dangle = 0f;
            var lenght = (float)Math.Sqrt(Math.Pow((double)(target.X - copter.X), 2) + Math.Pow((double)(target.Y - copter.Y), 2));
            if (target.X - copter.X != 0)
            {

                var angle = (float)Math.Atan((double)((target.Y - copter.Y) / (target.X - copter.X)));
                if ((target.X - copter.X) < 0)
                    if ((target.Y - copter.Y) > 0)
                        angle = angle + (float)Math.PI;
                    else if ((target.Y - copter.Y) < 0)
                        angle = -angle + (float)Math.PI;

                var nAngle = anglecopter;


                dangle = angle - nAngle;
                if (dangle > (float)Math.PI)
                    dangle = -2 * (float)Math.PI + dangle;
                if (dangle < -(float)Math.PI)
                    dangle = 2 * (float)Math.PI + dangle;

                ;
            }
            else
            {
                dangle = (float)Math.PI / 2;
            }
            return dangle;
        }

        #endregion


        #region база условий смены состояния

        void ConditionCoordinates()
        {
            SetPointCoordinates set = (SetPointCoordinates)CurrentSet;
            if (set.Coordinates.Count != 0)
            {
                ChangeSets();
            }
        }
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
            if (workCompleted)
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

       

       
    }
}
