using HapticSceneDescription.Gltf.Properties;

namespace HapticSceneDescription.Gltf.Controllers
{
    public class InteractivityController
    {
        private SceneDescription _sd;

        public InteractivityController(SceneDescription sd)
        {
            _sd = sd;
        }

        public void Update()
        {
            UpdateTriggers();
            UpdateBehaviors();
        }

        private void UpdateTriggers()
        {
            foreach (var trigger in _sd.Triggers)
            {
                trigger.Update();
            }
        }

        private void UpdateBehaviors()
        {
            foreach (var behavior in _sd.Behaviors)
            {
                behavior.TryTrigger(_sd);
            }
        }

    }
}