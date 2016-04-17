using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    class PID
    {
        float P;
        float I;
        float D;

        float Derr;
        float Integr;
        //float 

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
}
