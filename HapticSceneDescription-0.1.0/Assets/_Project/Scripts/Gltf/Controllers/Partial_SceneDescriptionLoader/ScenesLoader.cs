using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HapticSceneDescription.Gltf.Properties;
using LitJson;
using UnityEngine;

namespace HapticSceneDescription.Gltf.Controllers
{
    public static partial class SceneDescriptionLoader
    {
        private static class ScenesLoader
        {
            public static async UniTask LoadAsync(SceneDescription sd, JsonData sdJson)
            {
                Transform app = GameObject.Find("App").transform;
                Transform scenesRoot = new GameObject("ScenesRoot").transform;
                scenesRoot.SetParent(app);
                sd.Scene = (int)sdJson["scene"];
                sd.Scenes = new List<Scene>();
                foreach (JsonData sceneJson in sdJson["scenes"])
                {
                    Scene scene = new Scene
                    {
                        Name = (string)sceneJson["name"],
                        Nodes = new List<int>()
                    };
                    Transform oneSceneRoot = new GameObject(scene.Name).transform;
                    oneSceneRoot.SetParent(scenesRoot);
                    foreach (JsonData nodeIdx in sceneJson["nodes"])
                    {
                        scene.Nodes.Add((int)nodeIdx);
                        sd.NodeAdapters[sd.Nodes[(int)nodeIdx]].SetSceneRoot(oneSceneRoot);
                    }
                    sd.Scenes.Add(scene);
                }

                await UniTask.Yield();
            }
        }
    }
}