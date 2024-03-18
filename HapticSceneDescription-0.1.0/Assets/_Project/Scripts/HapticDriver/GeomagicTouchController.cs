using System.Runtime.InteropServices;
using UnityEngine;

namespace HapticSceneDescription
{
    public class GeomagicTouchController : IHapticDriver
    {
        private string _deviceIdentifier = "Default Device";
        private double[] _vibrationGDir = new double[3] { 0.0, 0.0, 1.0 };
        private int _vibrationGFrequency = 100;
        private float _scaleFactor = 0.02f;

        public void Start()
        {
            initDevice(_deviceIdentifier);
            startSchedulers();
        }

        public void OnDestroy()
        {
            disconnectAllDevices();
        }

        public void PlayValue(float intensity, int perceptionIndex)
        {
            PlayValue(intensity);
        }

        public void PlayValue(float intensity)
        {
            setVibrationValues(_deviceIdentifier, _vibrationGDir, intensity, _vibrationGFrequency, 0.0);
        }

        public void Stop(int perceptionIndex)
        {
            Stop();
        }

        public void Stop()
        {
            setVibrationValues(_deviceIdentifier, _vibrationGDir, 0.0, 0, 0.0);
        }

        public bool TryGetTouchPosition(out Vector3 position)
        {
            double[] matInput = new double[16];
            getTransform(_deviceIdentifier, matInput);

            for (int ii = 0; ii < 16; ii++)
                if (ii % 4 != 3)
                    matInput[ii] *= _scaleFactor;

            Matrix4x4 mat;
            mat.m00 = (float)matInput[0];
            mat.m01 = (float)matInput[1];
            mat.m02 = (float)matInput[2];
            mat.m03 = (float)matInput[3];
            mat.m10 = (float)matInput[4];
            mat.m11 = (float)matInput[5];
            mat.m12 = (float)matInput[6];
            mat.m13 = (float)matInput[7];
            mat.m20 = (float)matInput[8];
            mat.m21 = (float)matInput[9];
            mat.m22 = (float)matInput[10];
            mat.m23 = (float)matInput[11];
            mat.m30 = (float)matInput[12];
            mat.m31 = (float)matInput[13];
            mat.m32 = (float)matInput[14];
            mat.m33 = (float)matInput[15];
            position = mat.transpose.ExtractPosition();
            if (position.x == float.NaN || position.y == float.NaN || position.z == float.NaN)
            {
                position = Vector3.zero;
                return false;
            }
            if (float.IsInfinity(position.x) || float.IsInfinity(position.y) || float.IsInfinity(position.z))
            {
                position = Vector3.zero;
                return false;
            }
            return true;
        }

        [DllImport("HapticsDirect")] public static extern int initDevice(string deviceName);
        [DllImport("HapticsDirect")] public static extern void startSchedulers();
        [DllImport("HapticsDirect")] public static extern void setVibrationValues(string configName, double[] direction3, double magnitude, double frequency, double time);
        [DllImport("HapticsDirect")] public static extern void disconnectAllDevices();

        [DllImport("HapticsDirect")] public static extern void getTransform(string configName, double[] matrix16);
    }
}