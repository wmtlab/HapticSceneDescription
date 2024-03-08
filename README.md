# HapticSceneDescription
Haptic system with scene description system based on MPEG standard.

## 1. Related Hardware
   - Keyboard (position input)
   - Tactile glove (haptic playback, NeuroDigital AvatarVR in this project)

## 2. Related Software
   - Unity 2020 (2020.3.48f1c1 recommended)

## 3. Quick Start
1. Clone all the project in this repository to your local machine.
2. Open project `HapticSceneDescription-0.1.0` in Unity and set `Assets/_Project/Scenes/Main.unity` as active scene.
3. If AvatarVR glove is not available, please set `_hapticAdapter.UseHapticDevice` to `false` in `App.cs` in order to disable the haptic device. For example:
```csharp
    public class App : MonoBehaviour
    {
        private HapticAdapter _hapticAdapter;
        private async void Start()
        {
            _sd = await SceneDescriptionLoader.LoadAsync("sd.gltf");
            _hapticAdapter = _sd.HapticAdapter;
            _hapticAdapter.UseHapticDevice = false;
        }
    }
```
4. Scene description file `sd.gltf` and Haptic Medias `*.hmpg` are located in `Assets/StreamingAssets/`. Haptic Medias are grouped in folder `Assets/StreamingAssets/HapticMedias/`.
5. Click play button in the project and you can see the scene designed in `sd.gltf`. There are 2 spheres, 1 smaller and 1 larger, in the scene. The smaller sphere is position-controllable, while the larger sphere is fixed. You can move the smaller sphere by pressing WSAD or arrow keys on the keyboard. When the smaller sphere penetrates in or out of the larger sphere, the tactile glove will render different tactile feedback.

## 4. License
This project is licensed under the BSD 3-Clause License - see the LICENSE file for details.

Referenced projects:

1. `UniTask` MIT https://github.com/Cysharp/UniTask
2. `LitJSON` unlicensed https://github.com/LitJSON/litjson
3. `AvatarVR SDK` NeuroDigital Technologies https://www.neurodigital.es