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
        Bitmap FrontCamera;
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
            //FrontCamera = cptr.getDataFromISensor(1);
            //if (bmp != null)
            //    infoFromCamera = vs.SearchCycrles(bmp);

        }


       

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

        public Bitmap GetPictureFromFrontCamera
        {
            get { return FrontCamera; }
        }
        #endregion 
    }
}
