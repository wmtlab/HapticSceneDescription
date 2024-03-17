using System.Runtime.InteropServices;

namespace HapticSceneDescription.GeomagicTouch
{
    public class GeomagicTouchController
    {
        private string _deviceIdentifier = "Default Device";
        private double[] _vibrationGDir = new double[3] { 0.0, 0.0, 1.0 };
        private int _vibrationGFrequency = 50;

        public void Start()
        {
            initDevice(_deviceIdentifier);
            startSchedulers();
        }

        public void OnDestroy()
        {
            disconnectAllDevices();
        }

        public void PlayValue(float intensity)
        {
            setVibrationValues(_deviceIdentifier, _vibrationGDir, intensity, _vibrationGFrequency, 0.0);
        }

        public void Stop()
        {
            setVibrationValues(_deviceIdentifier, _vibrationGDir, 0.0, 0, 0.0);
        }

        [DllImport("HapticsDirect")] public static extern int initDevice(string deviceName);
        [DllImport("HapticsDirect")] public static extern void startSchedulers();
        [DllImport("HapticsDirect")] public static extern void setVibrationValues(string configName, double[] direction3, double magnitude, double frequency, double time);
        [DllImport("HapticsDirect")] public static extern void disconnectAllDevices();
    }
}