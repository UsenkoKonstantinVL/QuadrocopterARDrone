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
    public partial class Form4 : Form
    {
        #region

        QuadrocopterController copter;
        InformationSystem infSys;
        VisionControl vc;


        #endregion
        public Form4()
        {
            InitializeComponent();
        }

        public Form4(QuadrocopterController cp)
        {
            InitializeComponent();
            copter = cp;
            init();
        }

        public double GetZonePercentage(DetectedZone myZone)
        {
            double percentage = 0;
            int xe = 0, ye = 0, xs = 0, ys = 0;


            switch(myZone)
            {
                case DetectedZone.Center:
                    xs = ys = 39;
                    xe = ye = 59;
                    break;

                case DetectedZone.Bottom:
                    xs = 39;
                    ys = 59;
                    xe = 59;
                    ye = 99;
                    break;

                case DetectedZone.Top:
                    xs = 39;
                    ys = 0;
                    xe = 59;
                    ye = 39;
                    break;

                case DetectedZone.Right:
                    xs = 59;
                    ys = 39;
                    xe = 99;
                    ye = 59;

                    break;

                case DetectedZone.TopRight:
                    xs = 59;
                    ys = 0;
                    xe = 99;
                    ye = 39;

                    break;

                case DetectedZone.BottomRight:
                    xs = 59;
                    ys = 59;
                    xe = 99;
                    ye = 99;

                    break;

                case DetectedZone.Left:
                    xs = 0;
                    ys = 39;
                    xe = 39;
                    ye = 59;
                    break;

                case DetectedZone.TopLeft:
                    xs = 0;
                    ys = 0;
                    xe = 39;
                    ye = 39;
                    break;

                case DetectedZone.BottomLeft:
                    xs = 0;
                    ys = 59;
                    xe = 39;
                    ye = 99;
                    break;

                case DetectedZone.Zone1:
                    xs = 0;
                    ys = 0;
                    xe = 49;
                    ye = 49;
                    break;

                case DetectedZone.Zone2:
                    xs = 0;
                    ys = 49;
                    xe = 49;
                    ye = 99;
                    break;

                case DetectedZone.Zone3:
                    xs = 49;
                    ys = 0;
                    xe = 99;
                    ye = 49;
                    break;

                case DetectedZone.Zone4:
                    xs = 49;
                    ys = 49;
                    xe = 99;
                    ye = 99;
                    break;

                case DetectedZone.All:
                    xs = 0;
                    ys = 0;
                    xe = 99;
                    ye = 99;
                    break;
            }

            percentage = GetPercentage(xs, ys, xe, ye);

            return percentage;
        }

        double GetPercentage(int xs, int ys, int xe, int ye)
        {
            double res = 0;
            res =  vc.GetPercentageOfProbabilityLine(graybmp, xs, ys, xe, ye);
            return res;
        }

        void init()
        {
            infSys = new InformationSystem(copter);
            vc = new VisionControl();
        }

        Bitmap graybmp;
        Bitmap grayFrontbmp;

        private void timer1_Tick(object sender, EventArgs e)
        {
            infSys.UpdateSensorDara();
            if (infSys.GetPictureFromCamera != null) { 
                var bmp = (Bitmap)infSys.GetPictureFromCamera.Clone();
                pictureBox1.Image = bmp;
                graybmp = vc.GetAreaOfLineFromBitmap(bmp);
                pictureBox2.Image = graybmp;
                //var bmp2 = (Bitmap)infSys.GetPictureFromCamera.Clone();
                //pictureBox3.Image = bmp2;
                //grayFrontbmp = vc.GetAreaOfLineFromBitmap(bmp2);
                //pictureBox4.Image = grayFrontbmp;
                textBox1.Text = vc.GetPercentageOfProbabilityLine(graybmp, 0,0, graybmp.Width - 1, graybmp.Height - 1).ToString() + Environment.NewLine + graybmp.Width.ToString() + Environment.NewLine + graybmp.Height.ToString()+  Environment.NewLine + infSys.MySensorData.PrintResult();
            }
        }

        public SensorData GetDataFromCopter()
        {

            return infSys.MySensorData;
        }
    }
}
