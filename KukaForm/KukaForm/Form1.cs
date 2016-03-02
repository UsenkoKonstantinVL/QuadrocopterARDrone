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

        int clientID = 0;
        int rmHandle = 0;
        int lmHandle = 0;
        int camera = 0;
        Thread workerThread = null;
        VisionSensor vs;
        VRepController vr;
        Drive dr1;
        Drive dr2;

        public Form1()
        {
            InitializeComponent();

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (buttonStart.Text == "Старт")
            {
                vr = new VRepController(7777);
                vr.ConnectionStart();
                //clientID = VREPWrapper.simwStart("127.0.0.1", 7777);
                string cam = "Vision_sensor";
                string c = "Quadricopter_floorCamera";

                lmHandle = vr.ObjectHandle("Pioneer_p3dx_leftMotor");
                rmHandle = vr.ObjectHandle("Pioneer_p3dx_rightMotor");
                //camera = vr.ObjectHandle(cam);
                /*VREPWrapper.simwGetObjectHandle(clientID, "Pioneer_p3dx_leftMotor", out lmHandle);
                VREPWrapper.simwGetObjectHandle(clientID, "Pioneer_p3dx_rightMotor", out rmHandle);
                VREPWrapper.simwGetObjectHandle(clientID, cam, out camera);*/

                dr1 = new Drive(vr, "Pioneer_p3dx_leftMotor");
                dr2 = new Drive(vr, "Pioneer_p3dx_rightMotor");
                vs = new VisionSensor(vr, cam);
               
                buttonStart.Text = "Стоп";
            }
            else
            {
                vr.ConnectionStop();
                buttonStart.Text = "Старт";
            }


        }

        void SetDistance(object myKukiForDistance)
        {
            //myKuki.SetVelocity(3.14f, 3.14f, 3.14f, 3.14f);



            while (true)
            {
                /*flfao.Calculate(myKuki.getFloat(Names.forthBoard), myKuki.getFloat(Names.leftBoard), myKuki.getFloat(Names.rightBoard), myKuki.computeAngle());
                lBoard = flfao.CalculateLS();
                rBoard = flfao.CalculateRS();
                
                myKuki.drive(rBoard, lBoard);*/

                //myKuki.computeAngle();
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        string newLine()
        {
            return "\r\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float pi = 3.14f;
            dr1.setVelocityToDrive(pi);
            dr2.setVelocityToDrive(pi);
            /*VREPWrapper.simwSetJointTargetVelocity(clientID, lmHandle, pi);
            VREPWrapper.simwSetJointTargetVelocity(clientID, rmHandle, pi);*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            float pi = -3.14f;
            VREPWrapper.simwSetJointTargetVelocity(clientID, lmHandle, pi);
            VREPWrapper.simwSetJointTargetVelocity(clientID, rmHandle, pi);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            float pi = 3.14f;
            VREPWrapper.simwSetJointTargetVelocity(clientID, lmHandle, pi);
            VREPWrapper.simwSetJointTargetVelocity(clientID, rmHandle, -pi);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            float pi = 3.14f;
            VREPWrapper.simwSetJointTargetVelocity(clientID, lmHandle, -pi);
            VREPWrapper.simwSetJointTargetVelocity(clientID, rmHandle, pi);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            workerThread = new Thread(setPicture);
            workerThread.Start();
            /*
                 IntPtr myinpt;
                 int resol = 0;
                 myinpt = VREPWrapper.simwGetVisionSensorImage(clientID, camera, ref resol);
                 Bitmap bmp;
             if (resol != 0)
             {
                 bmp = new Bitmap(resol, resol, 3 * resol, System.Drawing.Imaging.PixelFormat.Format24bppRgb, myinpt);
                 bmp = getNewBmp(bmp);
                 // bmp = byteArrayToImage(convertSamples(new Image(bmp), resol, resol));
                 pictureBox1.Image = bmp;// byteArrayToImage(convertSamples(imageToByteArray(bmp), resol,resol));
             }*/
        }

        public void setPicture()
        {
            while(vr.isClientConnected())
            {
                Bitmap bmp = vs.getImageFromVisionSensor();
                pictureBox1.Image = bmp;
                Thread.Sleep(50);
            }
            /*while (VREPWrapper.isConnected(clientID)) {
                IntPtr myinpt;
                int resol = 0;
                myinpt = VREPWrapper.simwGetVisionSensorImage(clientID, camera, ref resol);
                Bitmap bmp;
                if (resol != 0)
                {
                    bmp = new Bitmap(resol, resol, 3 * resol, System.Drawing.Imaging.PixelFormat.Format24bppRgb, myinpt);
                    bmp = getNewBmp(bmp);
                    // bmp = byteArrayToImage(convertSamples(new Image(bmp), resol, resol));
                    pictureBox1.Image = bmp;// byteArrayToImage(convertSamples(imageToByteArray(bmp), resol,resol));
                }
                Thread.Sleep(100);
            }*/
        }


        public Bitmap getNewBmp(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            for(int i = 0; i< w; i++)
            {
                for(int j = 0; j < h; j++)

                {
                    Color clr = Color.FromArgb(bmp.GetPixel(i, j).B, bmp.GetPixel(i, j).G, bmp.GetPixel(i, j).R);
                    //clr. = bmp.GetPixel(i, j).B;
                    bmp.SetPixel(i, j, clr);
                }
            }

            return bmp;
        }

        private byte[] convertSamples(Image dt, int width, int height)
        {
            byte[] data = ImageToByte(dt);
            int stride = 3* width;
            const int samplesPerPixel = 3;

            for (int y = 0; y < height; y++)
            {
                int offset = stride * y;
                int strideEnd = offset + width * samplesPerPixel;

                for (int i = offset; i < strideEnd; i += samplesPerPixel)
                {
                    byte temp = data[i + 2];
                    data[i + 2] = data[i];
                    data[i] = temp;
                }
                
            }
            //Bitmap bmp = new Bitmap(data);
            return data;
        }

        public static  byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            float pi = 0f;
            VREPWrapper.simwSetJointTargetVelocity(clientID, lmHandle, -pi);
            VREPWrapper.simwSetJointTargetVelocity(clientID, rmHandle, pi);
        }
    }
}
