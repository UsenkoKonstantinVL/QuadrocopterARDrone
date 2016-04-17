using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using remoteApiNETWrapper;

namespace Controller
{
    public class VRepController
    {
        private int clientID = -1;
        private int port = 0;

        public VRepController(int _port = 0)
        {
            port = _port;
        }

        public void ConnectionStart()
        {
            clientID = VREPWrapper.simwStart("127.0.0.1", port);
        }

        public void ConnectionStop()
        {
            VREPWrapper.simwFinish(clientID);
        }

        public int ObjectHandle(string nameOfObj)
        {
            int oHandle = 0;
            VREPWrapper.simwGetObjectHandle(clientID, nameOfObj, out oHandle);
            return oHandle;
        }

        public void SetVelocityToObject(int obj, float vel)
        {
            VREPWrapper.simwSetJointTargetVelocity(clientID, obj, vel);
        }

        public void setStringSignal(string nSignal, string signal)
        {
            VREPWrapper.simwSetStringSignal(clientID, nSignal, signal);
        }

        public float[] getObjectPosition(int obj)
        {
            return VREPWrapper.simwGetObjectPosition(clientID, obj);
        }

        public void setFloatSignal(string nsignal, float val)
        {
            VREPWrapper.simwSetFloatSignal(clientID, nsignal, val);
        }

        public float getJointPosition(int obj)
        {
            return VREPWrapper.simwGetJointPosition(clientID, obj);
        }

        public float[] getObjectOrientation(int obj, int relative)
        {
            return VREPWrapper.simwGetObjectOrientation(clientID, obj, relative);
        }

        public void getStringSignal(string nSignal)
        {

        }

        public bool isClientConnected()
        {
            return VREPWrapper.isConnected(clientID);
        }

        public int getIntSignal(string nSignal)
        {
            int ret = 0;


            return ret;
        }

        public float getFloatSignal(string nSignal)
        {
            float ret = VREPWrapper.simwGetFloatSignal(clientID ,nSignal);

            return ret;
        }

        public VelocityArrayData GetVelocity(int j)
        {
            return VREPWrapper.simwGetObjectVelocity(clientID, j);
        }

        public Bitmap GetVisionImage(int obj)
        {
            IntPtr myinpt;
            int resol = 0;
            myinpt = VREPWrapper.simwGetVisionSensorImage(clientID, obj, ref resol);
            Bitmap bmp = null;
            if (resol != 0)
            {
                bmp = new Bitmap(resol, resol, 3 * resol, System.Drawing.Imaging.PixelFormat.Format24bppRgb, myinpt);
                //bmp = getNewBmp(bmp);
            }
            return bmp;
        }

        private Bitmap getNewBmp(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)

                {
                    Color clr = Color.FromArgb(bmp.GetPixel(i, j).B, bmp.GetPixel(i, j).G, bmp.GetPixel(i, j).R);
                    bmp.SetPixel(i, j, clr);
                }
            }

            return bmp;
        }

    }
}
