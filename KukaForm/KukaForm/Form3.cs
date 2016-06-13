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
                copter.AddSensor("Visionfront_sensor");
                

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
            sp.height = 0.9f;
            mySets.Add(sp);

            sp = new Setpoint();
            sp.currentProcces = WhichProcess.Hover;
            mySets.Add(sp);

            SetPointCoordinates p = new SetPointCoordinates();
            p.currentProcces = WhichProcess.Coordinates;
            Points3 p1, p2;
            p1 = new Points3();
            p2 = new Points3();

            p1.X = 1f;
            p1.Y = 2f;

            p2.X = 2f;
            p2.Y = 2f;

            p.Coordinates = new List<Points3>();

            p.Coordinates.Add(p1);
            p.Coordinates.Add(p2);
            mySets.Add(p);

            //sp = new Setpoint();
            //sp.currentProcces = WhichProcess.PitchRoll;
            //mySets.Add(sp);

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

            CurrentSet = mySets[CurrentSets];

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
            else
            {
                CurrentSet = new Setpoint();
                CurrentSet.currentProcces = WhichProcess.Hover;
                CurrentSets--;
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
            switch (/*mySets[CurrentSets]*/CurrentSet.currentProcces)
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
            
        }


        #region База алгоритмов

        void ConditionHoverPosition()
        {
            ;
        }

        void AlgorythmGetHeight()
        {
            myReqPos.Height = CurrentSet.height;
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

           
            if(zoneCenter < tolerance)
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
                if ((zoneBottomLeft > tolerance) || (zoneBottom> tolerance) || (zoneBottomRight> tolerance))
                {
                    
                    pitch += -0.001f;
                }
            }
            else
            {
                if(zoneTop > tolerance)
                {
                    pitch = 0.0005f;
                }

                if ((zoneTopLeft > tolerance)&& (zoneTop < tolerance))
                {
                    yaw = -0.1f;
                }
                else if((zoneTopRight > tolerance) && (zoneTop < tolerance))
                {
                    yaw = 0.1f;
                }

                if((zoneLeft > tolerance) && (zoneTop < tolerance))
                {
                    yaw = -0.1f;
                }
                else if ((zoneRight > tolerance)&& (zoneTop < tolerance))
                {
                    yaw = 0.1f;
                }
            }


            
            SetRequareMoving(WhichControlToUse.PitchRoll);
            myReqPos.Speed.Z = yaw;
            myReqPos.Roll = roll;
            myReqPos.Pitch = pitch;

            textBox1.Text += Environment.NewLine + "Control yaw... " + yaw.ToString() ;
            textBox1.Text += Environment.NewLine + "Control roll... " + roll.ToString();
            textBox1.Text += Environment.NewLine + "Control pitch... " + pitch.ToString();

            textBox1.Text += Environment.NewLine + zoneTopLeft.ToString() + " " + zoneTop.ToString() + " " + zoneTopRight.ToString();
            textBox1.Text += Environment.NewLine + zoneLeft.ToString() + " " + zoneCenter.ToString() + " " + zoneRight.ToString();
            textBox1.Text += Environment.NewLine + zoneBottomLeft.ToString() + " " + zoneBottom.ToString() + " " + zoneBottomRight.ToString();
        }
        #region close
        void MoveToLineZone()
        {
            float x, y;
            x = mySensorData.Coordinates.X;
            y = mySensorData.Coordinates.Y;


            float xs = 0, ys = 0, xe = 0, ye = 0;
            if(((xs < x) && (ys < y)) && ((xe  > x)&&(ye > y)))
            {

                float _x = (xe + xs) / 2;
                float _y = (ye + ys) / 2;
                if(x < xs)
                {

                }
                if(y < ys)
                {

                }

                if(x > xe)
                {

                }
                if(y > ye)
                {

                }

                float yaw = 0, pitch = 0, roll = 0;

                //yaw = 
            }
            else
            {

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
                roll -= droll;
                pitch -= dpitch;
            }
            if(zone4 > threshhold)
            {
                textBox1.Text += Environment.NewLine + "Fouth rule works...";
                roll -= droll;
                pitch += dpitch;
            }

            textBox1.Text += Environment.NewLine + "Roll..." + roll.ToString();
            textBox1.Text += Environment.NewLine + "Pitch..." + pitch.ToString();
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

            SetUpRequre(roll, pitch, yaw);
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
        #endregion

        void AlgorythmGetYaw()
        {
            textBox1.Text = "Getting Yaw: " + myReqPos.Yaw;
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

        void AlgorithmForReachCoordinates()
        {
            SetPointCoordinates set = (SetPointCoordinates)CurrentSet;

            if(!isNear(set.Coordinates[0], mySensorData.Coordinates, 0.5f))
            {
                set.Coordinates.RemoveAt(0);
            }

            if(set.Coordinates.Count != 0)
            {
                var pos = set.Coordinates[0];
                var dangle = GetAngle(mySensorData.Coordinates, set.Coordinates[0], mySensorData.Yaw);
                var lenght = Sqrt(mySensorData.Coordinates, set.Coordinates[0]);

                float vang = 0;
                float pitch = 0;


                if (Math.Abs(dangle) > 0.01f)
                {
                    if(dangle > 0)
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

                textBox1.Text = "Fly to Coordinate " + set.Coordinates[0].X.ToString() + " " + set.Coordinates[0].Y.ToString();
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

        private void button3_Click(object sender, EventArgs e)
        {
            if(button3.Text == "Старт")
            {
                copter.getVrepController().StartSimulation();
                button3.Text = "Стоп";
            }
            else
            {
                copter.getVrepController().StopSimulation();
                button3.Text = "Старт";
            }
        }
    }


    public class Setpoint
    {
        public WhichProcess currentProcces;
        public float height;
        public float yaw;
    }

    public class SetPointCoordinates: Setpoint
    {
        public List<Points3> Coordinates;

    }
}
