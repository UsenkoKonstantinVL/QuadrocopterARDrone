using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KukaForm
{
    public abstract  class Command
    {
        protected string Name;
        protected string Condition;

        public Command(string _name, string _condition)
        {
            Name = _name;
            Condition = _condition;
        }

        public  bool isRight(string _name)
        {
            string newName = Name.ToLower();
            string _newname = _name.ToLower();
            return newName == _newname;
        }

         public abstract Setpoint GetCommand(string[] _condition);
    }

    public class CommandLanding: Command
    {
        public CommandLanding():base("Посадка", " ")
        {

        }

        public override Setpoint GetCommand(string[] _condition)
        {
            Setpoint sp = new Setpoint();
            sp.currentProcces = WhichProcess.Height;
            try {
                sp.height = (float)Convert.ToDouble(_condition[1]);
            }
            catch(Exception e)
            {
                return null;
            }

            return sp;
        }
    }

    public class CommandTurn : Command
    {
        public CommandTurn() : base("Поворот", " ")
        {

        }

        public override Setpoint GetCommand(string[] _condition)
        {
            Setpoint sp = new Setpoint();
            sp.currentProcces = WhichProcess.Yaw;
            try
            {
                sp.yaw = (float)Convert.ToDouble(_condition[1]);
            }
            catch (Exception e)
            {
                return null;
            }

            return sp;
        }
    }

    public class CommandFollowLine : Command
    {
        public CommandFollowLine() : base("СледованиеЛинии", " ")
        {

        }

        public override Setpoint GetCommand(string[] _condition)
        {
            Setpoint sp = new Setpoint();
            sp.currentProcces = WhichProcess.PitchRoll;
           

            return sp;
        }
    }
}
