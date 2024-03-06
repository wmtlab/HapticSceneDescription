using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HapticSceneDescription.Gltf.Properties;
using HapticSceneDescription.Gltf.Properties.MpegSceneInteractivity;
using LitJson;
using UnityEngine;

namespace HapticSceneDescription.Gltf.Controllers
{
    public static partial class SceneDescriptionLoader
    {
        private static class ActionsLoader
        {
            public static async UniTask LoadAsync(SceneDescription sd, JsonData sdJson)
            {
                if (!sdJson.ContainsKey("extensions"))
                {
                    return;
                }
                JsonData extensions = sdJson["extensions"];
                if (!extensions.ContainsKey("MPEG_scene_interactivity"))
                {
                    return;
                }
                JsonData mpegSceneInteractivity = extensions["MPEG_scene_interactivity"];
                if (!mpegSceneInteractivity.ContainsKey("actions"))
                {
                    return;
                }
                JsonData actions = mpegSceneInteractivity["actions"];
                foreach (JsonData actionJson in actions)
                {
                    IAction action = ActionFactory(sd, actionJson);
                    sd.Actions.Add(action);
                }
                await UniTask.Yield();
            }

            private static IAction ActionFactory(SceneDescription sd, JsonData actionJson)
            {
                string type = (string)actionJson["type"];
                IAction action = null;
                switch (type)
                {
                    case "ACTION_SET_HAPTIC":
                        action = SetHapticActionFactory(sd, actionJson);
                        break;
                }
                return action;
            }

            private static SetHapticAction SetHapticActionFactory(SceneDescription sd, JsonData actionJson)
            {
                SetHapticAction action = new SetHapticAction();
                action.ActionHandler += (actionData) => Debug.Log($"SetHapticAction{actionData.HapticActionNodes[0].HapticActionMedias[0].MediaIndex}");
                action.ActionHandler += sd.HapticAdapter.TryPlay;
                if (actionJson.ContainsKey("hapticActionNodes"))
                {
                    JsonData hapticActionNodes = actionJson["hapticActionNodes"];
                    foreach (JsonData hapticActionNode in hapticActionNodes)
                    {
                        SetHapticAction.HapticActionNode node = new SetHapticAction.HapticActionNode
                        {
                            Node = (int)hapticActionNode["node"],
                            HapticActionMedias = new List<SetHapticAction.HapticActionNode.HapticActionMedia>()
                        };
                        JsonData hapticActionMedias = hapticActionNode["hapticActionMedias"];
                        foreach (JsonData hapticActionMedia in hapticActionMedias)
                        {
                            SetHapticAction.HapticActionNode.HapticActionMedia media = new SetHapticAction.HapticActionNode.HapticActionMedia
                            {
                                MediaIndex = (int)hapticActionMedia["mediaIndex"],
                                PerceptionIndices = new List<int>()
                            };
                            JsonData perceptionIndices = hapticActionMedia["perceptionIndices"];
                            foreach (JsonData perceptionIndex in perceptionIndices)
                            {
                                media.PerceptionIndices.Add((int)perceptionIndex);
                            }
                            node.HapticActionMedias.Add(media);
                        }
                        action.HapticActionNodes.Add(node);
                    }
                }
                return action;
            }
        }
    }
}