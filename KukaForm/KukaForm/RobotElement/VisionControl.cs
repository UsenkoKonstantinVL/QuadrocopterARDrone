using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;


namespace KukaForm
{
   public  class VisionControl
    {
        Bitmap CaptureFromRobot;
        Bitmap ModelOfSearchRobot;

       // Image<Bgr, Byte> My_Image;


        string FileOfModelOfSearchRobot = "";

        public VisionControl()
        {

        }

        #region Public method
        public Bitmap GetAreaOfLineFromBitmap(Bitmap inputBMP)
        {
           
           var  My_Image = new Image<Bgr, byte>(inputBMP);
            My_Image = My_Image.Resize(100, 100, Inter.Linear);
           
            My_Image = My_Image.SmoothMedian(5);// + My_Image.SmoothBlur(1, 1);          
            double maxintesity = 20;
            double minintesity = 0;
            var im = (RGBFilter(My_Image, new Gray(minintesity), new Gray(maxintesity), new Gray(minintesity), new Gray(maxintesity), new Gray(minintesity), new Gray(maxintesity), 1));

            return im.Bitmap;

        }

        public double GetPercentageOfProbabilityLine(Bitmap bmp, int xs, int ys, int xe, int ye)
        {
            double percentage = 0;

            double allcount = 0;
            double whitecount = 0;

            var img = new Image<Gray, byte>(bmp);

            for(int i = xs; i <= xe; i++)
            {
                for(int j = ys; j<=ye; j++)
                {
                    if(img.Data[i, j, 0] > 200)
                    {
                        whitecount++;
                    }
                    allcount++;
                }
            }

            percentage = whitecount / allcount;
            return percentage;
        }

      
        #endregion

        #region Private method
        private Image<Gray, Byte> RGBFilter(Image<Bgr, Byte> input, Gray Rmin, Gray Rmax, Gray Gmin, Gray Gmax, Gray Bmin, Gray Bmax, int dialate)
        {
            Image<Gray, Byte> result = new Image<Gray, byte>(input.Width, input.Height);
            Image<Gray, byte>[] chanels = input.Split();
            chanels[0] = chanels[0].InRange(Rmin, Rmax);
            chanels[1] = chanels[1].InRange(Gmin, Gmax);
            chanels[2] = chanels[2].InRange(Bmin, Bmax);
            result = chanels[0].And(chanels[1]);
            result = result.And(chanels[2]);
            //result = result.Dilate(dialate);
            return result;
        }

        #endregion

    }


    public class InformationFromPicture
    {
        public Bitmap bmp;
        public List<float> cCyrclX = new List<float>();
        public List<float> cCyrclY = new List<float>();
        public List<float> radius = new List<float>();
        public InformationFromPicture()
        { }
        public InformationFromPicture(Bitmap _bmp, float _cx, float _cy, float _r)
        {
            bmp = _bmp;
            cCyrclX.Add(_cx);
            cCyrclY.Add(_cy);
            radius.Add(_r);
        }
    }
}
