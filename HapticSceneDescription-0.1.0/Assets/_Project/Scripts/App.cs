using HapticSceneDescription.Gltf.Adapters;
using HapticSceneDescription.Gltf.Controllers;
using HapticSceneDescription.Gltf.Properties;
using HapticSceneDescription.Test;
using UnityEngine;

namespace HapticSceneDescription
{
    public class App : MonoBehaviour
    {
        public static PlayerController.InputType inputType = PlayerController.InputType.Keyboard;
        public static OutputType outputType = OutputType.None;
        [SerializeField]
        private GameObject _ndInitializerPrefab;
        private SceneDescription _sd;
        private InteractivityController _interactivity;
        private HapticAdapter _hapticAdapter;
        private PlayerController _playerController;

        private async void Start()
        {
            _sd = await SceneDescriptionLoader.LoadAsync("sd.gltf");
            _hapticAdapter = _sd.HapticAdapter;
            _hapticAdapter.UseHapticDevice = true;
            IHapticDriver hapticDriver = null;
            switch (outputType)
            {
                case OutputType.AvatarVR:
                    hapticDriver = new AvatarVRController();
                    Instantiate(_ndInitializerPrefab);
                    break;
                case OutputType.GeomagicTouch:
                    hapticDriver = new GeomagicTouchController();
                    break;
                case OutputType.None:
                    hapticDriver = default;
                    _hapticAdapter.UseHapticDevice = false;
                    break;
            };
            _hapticAdapter.HapticDriver = hapticDriver;
            _hapticAdapter.Start();
            _interactivity = new InteractivityController(_sd);
            // * Add a PlayerController to the player GameObject
            _playerController = new PlayerController
            {
                Transform = GameObject.Find("player").transform,
                inputType = inputType
            };
            if (inputType == PlayerController.InputType.GeomagicTouch)
            {
                if (outputType == OutputType.GeomagicTouch)
                {
                    _playerController.GeomagicTouch = hapticDriver as GeomagicTouchController;
                }
                else
                {
                    var geomagicTouch = new GeomagicTouchController();
                    _playerController.GeomagicTouch = geomagicTouch;
                }
            }
            _playerController?.Start();
        }

        private void FixedUpdate()
        {
            _playerController?.Update();
            _interactivity?.Update();
            _hapticAdapter?.Update();
        }

        private void OnDisable()
        {
            _hapticAdapter?.Stop();
            _playerController?.Stop();
        }

        public enum OutputType
        {
            None,
            AvatarVR,
            GeomagicTouch
        }
    }
}