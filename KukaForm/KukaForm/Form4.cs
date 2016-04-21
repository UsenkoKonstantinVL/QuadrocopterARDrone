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

        void init()
        {
            infSys = new InformationSystem(copter);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            infSys.UpdateSensorDara();
            pictureBox1.Image = infSys.GetPictureFromCamera;
            textBox1.Text = infSys.MySensorData.PrintResult();
        }

        public SensorData GetDataFromCopter()
        {

            return infSys.MySensorData;
        }
    }
}
