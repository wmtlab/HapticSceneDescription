using System.IO;
using Cysharp.Threading.Tasks;
using HapticSceneDescription.Gltf.Adapters;
using HapticSceneDescription.Gltf.Properties;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

namespace HapticSceneDescription.Gltf.Controllers
{
    public static partial class SceneDescriptionLoader
    {
        public static async UniTask<SceneDescription> LoadAsync(string dir)
        {
            string path = Path.Combine(Application.streamingAssetsPath, dir);
            UnityWebRequest sdRequest = UnityWebRequest.Get(path);
            await sdRequest.SendWebRequest();
            if (sdRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load scene description from {path}");
                return null;
            }
            string jsonStr = sdRequest.downloadHandler.text;
            return await FromJsonAsync(jsonStr);
        }

        public static async UniTask<SceneDescription> FromJsonAsync(string jsonStr)
        {
            SceneDescription sd = new SceneDescription();
            JsonData sdJson = JsonMapper.ToObject(jsonStr);

            #region Haptic Controller
            sd.HapticAdapter = new HapticAdapter();
            #endregion

            #region GLTF
            await NodesLoader.LoadAsync(sd, sdJson);
            await ScenesLoader.LoadAsync(sd, sdJson);
            #endregion

            #region MPEG Scene Interactivity
            await TriggersLoader.LoadAsync(sd, sdJson);
            await ActionsLoader.LoadAsync(sd, sdJson);
            await BehaviorsLoader.LoadAsync(sd, sdJson);
            #endregion

            return sd;
        }

    }
}