using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class PID
    {
        public float P;
        public float I;
        public float D;

        float Derr;
        float Integr;

        public PID()
        {
            P = I = D = 0;
        }

        public PID(float _P, float _I, float _D)
        {
            P = _P;
            I = _I;
            D = _D;
            Derr = 0;
            Integr = 0;
        }

        public float getEffect(float err)
        {
            float Pv = P * err;
            float Ival;
            float Dval;

            Integr += err;
            if (Integr > 10)
                Integr = 10;
            Ival = I * Integr;

            Dval = D * (err - Derr);
            Derr = err;

            return Pv + Ival + Dval;

        }
    }

    public class FuzzyPID:PID
    {
        public float[] fP;
        public float[] fI;
        public float[] fD;
        public float[] Condition;

        public FuzzyPID()
        {
            fP = new float[3];
            fI = new float[3];
            fD = new float[3];
            Condition = new float[2];
        }

        public FuzzyPID(float[] _P, float[] _I, float[] _D, float[] _C)
        {
            fP = _P;
            fI = _I;
            fD = _D;
            Condition = _C;
        }

        public float GetEffect(float error)
        {
            int i = 0;
            if(Math.Abs(error) < Condition[0])
            {
                i = 0;
            }
            else if(Math.Abs(error) < Condition[1] && Math.Abs(error) > Condition[0])
            {
                i = 1;
            }
            else
            {
                i = 2;
            }

            setPIDCoefficient(fP[i], fI[i], fD[i]);
            return getEffect(error);
        }

        void setPIDCoefficient(float _p, float _i, float _d)
        {
            P = _p;
            I = _i;
            D = _d;
        }
    }
}
