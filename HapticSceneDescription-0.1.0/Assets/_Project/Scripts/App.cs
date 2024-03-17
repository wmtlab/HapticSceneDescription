using HapticSceneDescription.Gltf.Adapters;
using HapticSceneDescription.Gltf.Controllers;
using HapticSceneDescription.Gltf.Properties;
using HapticSceneDescription.Test;
using UnityEngine;

namespace HapticSceneDescription
{
    public class App : MonoBehaviour
    {
        private SceneDescription _sd;
        private InteractivityController _interactivity;
        private HapticAdapter _hapticAdapter;

        private async void Start()
        {
            _sd = await SceneDescriptionLoader.LoadAsync("sd.gltf");
            _hapticAdapter = _sd.HapticAdapter;
            _hapticAdapter.UseHapticDevice = true;
            _hapticAdapter.Start();
            _interactivity = new InteractivityController(_sd);
            // * Add a PlayerController to the player GameObject
            GameObject.Find("player").AddComponent<PlayerController>();
        }

        private void FixedUpdate()
        {
            _interactivity?.Update();
            _hapticAdapter?.Update();
        }

        private void OnDisable()
        {
            _hapticAdapter?.Stop();
        }
    }
}