using NDAPIWrapperSpace;
using UnityDLL.Haptic;

namespace HapticSceneDescription
{
    public class AvatarVRController : IHapticDriver
    {
        public void Start()
        {
        }

        public void OnDestroy()
        {
            HapticSystem.StopActuators();
        }

        public void PlayValue(float intensity, int perceptionIndex)
        {
            HapticSystem.PlayValue(intensity, (Actuator)perceptionIndex);
        }

        public void PlayValue(float intensity)
        {
            HapticSystem.PlayValue(intensity);
        }

        public void Stop(int perceptionIndex)
        {
            HapticSystem.StopActuators((Actuator)perceptionIndex);
        }

        public void Stop()
        {
            HapticSystem.StopActuators();
        }
    }

}