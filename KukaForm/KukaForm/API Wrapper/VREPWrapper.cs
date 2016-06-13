using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace remoteApiNETWrapper
{
    public static class VREPWrapper
    {
        static bool fc = false;
        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void simxFinish(int clientID);
        public static void simwFinish(int id)
        {
            simxFinish(id);
        }

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]

        public static extern int simxGetConnectionId(int clientID);
        public static bool isConnected(int cliId)
        {
            return simxGetConnectionId(cliId) == -1 ? false : true;
        }

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxGetFloatSignal(int clientID, string signalName, ref float value, simx_opmode opmode);


        public static float simwGetFloatSignal(int clientID, string signalName)
        {
            clientID = 0;
            float p = -1;// = IntPtr.Zero;
            /*var l1 = 0;
            var l2 = 0;*/
            var e = simxGetFloatSignal(clientID, signalName, ref p,  simx_opmode.streaming);
            Thread.Sleep(10);
            e = simxGetFloatSignal(clientID, signalName, ref p, simx_opmode.buffer);
            //Console.WriteLine("Signal {0} -> {1}/{2}", signalName, l1, l2);
            if (e == simx_error.noerror )
            {
                //var s = Marshal.(p, l2);
                //Marshal.Release(p);
                return p;
            }
            return -1f;
        }

        //
        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxGetObjectVelocity(int clientID, int jointHandle, float[] linearVelocity, float[] angularVelocity, simx_opmode operationMode);


        public static VelocityArrayData simwGetObjectVelocity(int clientID, int jointHandle)
        {
            clientID = 0;
            float p = -1;// = IntPtr.Zero;
            VelocityArrayData myData = new VelocityArrayData();
            /*float[] linearVelocity = new float[3];
            float[] angularVelocity = new float[3];*/

            var e = simxGetObjectVelocity(clientID, jointHandle, myData.linearVelocity, myData.angularVelocity, simx_opmode.streaming);
           
            e = simxGetObjectVelocity(clientID, jointHandle, myData.linearVelocity, myData.angularVelocity, simx_opmode.buffer);
            //Console.WriteLine("Signal {0} -> {1}/{2}", signalName, l1, l2);
            if (e == simx_error.noerror)
            {
                return myData;
            }
            else
            {
                myData.angularVelocity = new float[3] { 0, 0, 0 };
                myData.linearVelocity = new float[3] { 0, 0, 0 };
            }
            return myData;
        }

        /*[DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxGetVelocity(int clientID, int jointHandle, float[] linearVelocity, float[] angularVelocity, simx_opmode operationMode);


        public static VelocityArrayData simwGetVelocity(int clientID, int jointHandle)
        {
            clientID = 0;
            float p = -1;// = IntPtr.Zero;
            VelocityArrayData myData = new VelocityArrayData();        
            //float[] linearVelocity = new float[3];
            //float[] angularVelocity = new float[3];

            var e = simxGetObjectVelocity(clientID, jointHandle, myData.linearVelocity, myData.angularVelocity, simx_opmode.streaming);

            e = simxGetObjectVelocity(clientID, jointHandle, myData.linearVelocity, myData.angularVelocity, simx_opmode.buffer);
            //Console.WriteLine("Signal {0} -> {1}/{2}", signalName, l1, l2);
            if (e == simx_error.noerror)
            {
                return myData;
            }
            else
            {
                myData.angularVelocity = new float[3] { 0, 0, 0 };
                myData.linearVelocity = new float[3] { 0, 0, 0 };
            }
            return myData;
        }*/
        //

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxGetIntegerSignal(int clientID, string signalName, ref int value, simx_opmode opmode);

       

        public static int simwGetIntegerSignal(int clientID, string signalName)
        {
            int v = -1;
            simxGetIntegerSignal(clientID, signalName, ref v, simx_opmode.streaming);
            Thread.Sleep(150);
            simxGetIntegerSignal(clientID, signalName, ref v, simx_opmode.buffer);
            return v;
        }


        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxSetFloatSignal(int clientID, string signalName, float signalValue, simx_opmode opmode);
        public static void simwSetFloatSignal(int clID, string signN, float signVal)
        {
            var er = simxSetFloatSignal(clID, signN, signVal, simx_opmode.oneshot);
        }

        
        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxSetStringSignal(int clientID, string signalName, string value, int length, simx_opmode opmode);
        public static void simwSetStringSignal(int clientID, string signalName, string signal)
        {

        }


        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxGetStringSignal(int clientID, string signalName, ref IntPtr pointerToValue, ref int signalLength, simx_opmode opmode);

        public static string simwGetStringSignal(int clientID, string signalName)
        {
            clientID = 0;
            IntPtr p = IntPtr.Zero;
            var l1 = 0;
            var l2 = 0;
            var e = simxGetStringSignal(clientID, signalName, ref p, ref l1, simx_opmode.streaming);
            Thread.Sleep(150);
            e = simxGetStringSignal(clientID, signalName, ref p, ref l2, simx_opmode.buffer);
            //Console.WriteLine("Signal {0} -> {1}/{2}", signalName, l1, l2);
            if (e == simx_error.noerror && p != IntPtr.Zero)
            {
                var s = Marshal.PtrToStringAnsi(p, l2);
                Marshal.Release(p);
                return s;
            }
            return "";
        }

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxGetAndClearStringSignal(int clientID, string signalName, ref IntPtr pointerToValue, ref int signalLength, simx_opmode opmode);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxGetJointPosition(int clientID, int jointHandle, ref float targetPosition, simx_opmode opmode);

        public static float simwGetJointPosition(int clientId, int jObj)
        {
            float f = 0;
            simxGetJointPosition(clientId, jObj, ref f, simx_opmode.streaming);
            simxGetJointPosition(clientId, jObj, ref f, simx_opmode.buffer);
            return f;
        }
        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxGetObjectIntParameter(int clientID, int objectHandle, int parameterID, ref int parameterValue, simx_opmode opmode);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxGetObjectFloatParameter(int clientID, int objectHandle, int parameterID, ref float parameterValue, simx_opmode opmode);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxGetObjectOrientation(int clientID, int jointHandle, int relativeToHandle, float[] orientations, simx_opmode opmode);

        public static float[] simwGetObjectOrientation(int clientId, int obj, int relativeTo)
        {
            float[] f = new float[3];
            simxGetObjectOrientation(clientId, obj, relativeTo, f, simx_opmode.streaming);
            simxGetObjectOrientation(clientId, obj, relativeTo, f, simx_opmode.buffer);
            return f;
        }

        
        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxGetObjectPosition(int clientID, int jointHandle, int relativeToHandle, float[] positions, simx_opmode opmode);

        public static float[] simwGetObjectPosition(int clientID, int obj)
        {
            //bool testc = true;
            float[] pos = new float[3];

            if (!fc)
            {
                simxGetObjectPosition(clientID, obj, -1, pos, simx_opmode.streaming);
                fc = true;
            }
             
            simxGetObjectPosition(clientID, obj, -1, pos, simx_opmode.buffer);
            if (pos.Length != 3)
            {
                pos = new float[3];
                pos[0] = 0;
                pos[1] = 0;
                pos[2] = 0;
            }
            return pos;
        }

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxPauseCommunication(int cliendID, int pause);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public extern static simx_error simxReadProximitySensor(int clientID, int sensorHandle,
                                                         ref char detectionState, float[] detectionPoint, ref int objectHandle, float[] normalVector, simx_opmode opmode);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxSetJointTargetPosition(int clientID, int jointHandle, float targetPosition, simx_opmode opmode);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxSetJointTargetVelocity(int clientID, int jointHandle, float velocity, simx_opmode opmode);

        public static void simwSetJointTargetVelocity(int clientID, int jointHandle, float velocity)
        {
            simxSetJointTargetVelocity(clientID, jointHandle, velocity, simx_opmode.oneshot);
        }

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxSetObjectFloatParameter(int clientID, int objectHandle, int parameterID, float parameterValue, simx_opmode opmode);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxSetObjectIntParameter(int clientID, int objectHandle, int parameterID, int parameterValue, simx_opmode opmode);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int simxStart(string ip, int port, bool waitForConnection, bool reconnectOnDisconnect, int timeoutMS, int cycleTimeMS);

        public static int simwStart(string ip, int port)
        {
            return simxStart(ip, port, true, true, 2000, 5);
        }

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        public static extern simx_error simxGetUIEventButton(int clientID, int uiHandle, ref int uiEventButtonID, IntPtr aux, simx_opmode opmode);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll")]
        // public static extern simx_error simxGetUIHandle(int clientID, string uiName, out int handle, simx_opmode opmode);
        public static extern simx_error simxGetUIHandle(int clientID, string uiName, IntPtr p, simx_opmode opmode);

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxGetObjectHandle(int clientID, string objectName, out int handle, simx_opmode opmode);
        public static void simwGetObjectHandle(int clientID, string objectName, out int handle)
        {
            simxGetObjectHandle(clientID, objectName, out handle, simx_opmode.oneshot_wait);
        }


        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxGetVisionSensorImage(int clientID, int sensorHandle, ref int resolution, ref IntPtr image, char options, int operationMode);
        public static IntPtr simwGetVisionSensorImage(int clientID, int obj, ref int resolution)
        {
            int intResolution = 0;
            char option = (char)0;///'\0';
            IntPtr imageIntPtr = IntPtr.Zero;
            simxGetVisionSensorImage(clientID, obj, ref resolution, ref imageIntPtr, option, (int)simx_opmode.streaming);
            return imageIntPtr;
        }


        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxStartSimulation(int clientID,  int operationMode);
        public static void simwStartSimulation(int clientID)
        {
        
            simxStartSimulation(clientID,  (int)simx_opmode.oneshot);
         
        }

        [DllImport(@"C:\Program Files (x86)\V-REP3\V-REP_PRO_EDU\programming\remoteApiBindings\lib\lib\32Bit\remoteApi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern simx_error simxStopSimulation(int clientID, int operationMode);
        public static void simwStopSimulation(int clientID)
        {
           
            simxStopSimulation(clientID, (int)simx_opmode.oneshot);
           
        }
    }

    public class VelocityArrayData
    {
        public float[] linearVelocity = new float[3];
        public float[] angularVelocity = new float[3];
    }
}