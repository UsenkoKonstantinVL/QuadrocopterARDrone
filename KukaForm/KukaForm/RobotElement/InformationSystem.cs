using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using Controller;

namespace KukaForm
{
    public class InformationSystem
    {
        
        QuadrocopterController cptr;
        RequiredPosition myReqPos;
        SensorData mySensorData;
        InformationFromPicture infoFromCamera;
        VisionControl vs;
        Bitmap bmp;
        Thread myWorkThread;

       public  delegate void DelegateForGettingPicture(InformationFromPicture bmp);


        public InformationSystem(QuadrocopterController _cptr)
        {
            cptr = _cptr;
            InitializeData();
        }


        public void InitializeData()
        {
            myReqPos = new RequiredPosition();
            mySensorData = new SensorData();
            vs = new VisionControl();
            infoFromCamera = new InformationFromPicture();
            
            
        }

        public void UpdateSensorDara()
        {
            mySensorData = cptr.GetSensorData();
            bmp = cptr.getDataFromISensor(0);
            //if (bmp != null)
            //    infoFromCamera = vs.SearchCycrles(bmp);

        }


        public void DetectObjectFromData(DelegateForGettingPicture del)
        {
           /* if (bmp == null)
                bmp = null;
            if (myWorkThread == null)
            {
                if (bmp != null)
                    myWorkThread = new Thread(delegate () { SearchCircles(del, new Bitmap(bmp)); });
                else
                    myWorkThread = new Thread(delegate () { SearchCircles(del, null); }); ;
            }
            if (myWorkThread.ThreadState == ThreadState.Stopped)
            {
                //myWorkThread.Abort();
                myWorkThread = new Thread(delegate () { SearchCircles(del, new Bitmap(bmp)); });
                myWorkThread.Start();
            
            }
            else if(myWorkThread.ThreadState == ThreadState.Unstarted)
            {
                
                myWorkThread.Start();
            }*/
            
        }
        //TODO: Написать функцию, выводящую через делегат в другом потоке картинку на PictureBox


        //void SearchCircles(DelegateForGettingPicture del, Bitmap obj)
        //{
        //    var d = (DelegateForGettingPicture)del;

        //    if (bmp != null)
        //    {
        //        Bitmap b = new Bitmap(bmp);
        //        d(vs.SearchCycrles(b));
        //    }
        //}

        #region Возвращаемые значения и классы

        public RequiredPosition MyReqPos
        {
            set { myReqPos = value; }

            get { return myReqPos; }
        }

        public SensorData MySensorData
        {
            set { mySensorData = value; }

            get { return mySensorData; }
        }

        public InformationFromPicture InformationFromCamera
        {
            get { return infoFromCamera; }
        }

        public Bitmap GetPictureFromCamera
        {
            get { return bmp; }
        }
        #endregion 
    }
}
