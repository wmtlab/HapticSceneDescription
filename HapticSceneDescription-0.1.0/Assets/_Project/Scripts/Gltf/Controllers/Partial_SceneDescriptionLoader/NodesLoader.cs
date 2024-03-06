using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HapticSceneDescription.Gltf.Adapters;
using HapticSceneDescription.Gltf.Properties;
using LitJson;
using UnityEngine;

namespace HapticSceneDescription.Gltf.Controllers
{
    public static partial class SceneDescriptionLoader
    {
        private static class NodesLoader
        {
            public static async UniTask LoadAsync(SceneDescription sd, JsonData sdJson)
            {
                sd.Nodes = new List<Node>();
                sd.NodeAdapters = new Dictionary<Node, NodeAdapter>();

                // 直接加载sd中的nodes，简单实例化node adapter
                foreach (JsonData nodeJson in sdJson["nodes"])
                {
                    Node node = new Node
                    {
                        Name = (string)nodeJson["name"],
                        Rotation = new Quaternion(
                            (float)(double)nodeJson["rotation"][0],
                            (float)(double)nodeJson["rotation"][1],
                            (float)(double)nodeJson["rotation"][2],
                            (float)(double)nodeJson["rotation"][3]
                        ),
                        Scale = new Vector3(
                            (float)(double)nodeJson["scale"][0],
                            (float)(double)nodeJson["scale"][1],
                            (float)(double)nodeJson["scale"][2]
                        ),
                        Translation = new Vector3(
                            (float)(double)nodeJson["translation"][0],
                            (float)(double)nodeJson["translation"][1],
                            (float)(double)nodeJson["translation"][2]
                        ),
                        Children = new List<int>(),
                        Mesh = nodeJson.ContainsKey("mesh") ? (int)nodeJson["mesh"] : -1
                    };

                    if (nodeJson.ContainsKey("children"))
                    {
                        foreach (JsonData childIdx in nodeJson["children"])
                        {
                            node.Children.Add((int)childIdx);
                        }
                    }

                    NodeAdapter adapter = new GameObject().AddComponent<NodeAdapter>();
                    sd.NodeAdapters.Add(node, adapter);
                    sd.Nodes.Add(node);
                }

                // 追加parent
                for (int i = 0; i < sd.Nodes.Count; i++)
                {
                    Node node = sd.Nodes[i];
                    foreach (int childIdx in node.Children)
                    {
                        sd.Nodes[childIdx].Parent = i;
                    }
                }

                // 设置node adapters
                for (int i = 0; i < sd.Nodes.Count; i++)
                {
                    Node node = sd.Nodes[i];
                    var adapter = sd.NodeAdapters[node];
                    adapter.Init(sd, node);
                }

                await UniTask.Yield();
            }
        }
    }
}