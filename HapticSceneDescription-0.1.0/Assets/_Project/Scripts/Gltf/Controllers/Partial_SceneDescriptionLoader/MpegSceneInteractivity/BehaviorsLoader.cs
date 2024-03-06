using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HapticSceneDescription.Gltf.Properties;
using HapticSceneDescription.Gltf.Properties.MpegSceneInteractivity;
using LitJson;

namespace HapticSceneDescription.Gltf.Controllers
{
    public static partial class SceneDescriptionLoader
    {
        private static class BehaviorsLoader
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
                if (!mpegSceneInteractivity.ContainsKey("behaviors"))
                {
                    return;
                }
                JsonData behaviors = mpegSceneInteractivity["behaviors"];
                foreach (JsonData behaviorJson in behaviors)
                {
                    Behavior behavior = new Behavior
                    {
                        Triggers = new List<int>(),
                        Actions = new List<int>(),
                        TriggersCombinationControl = (string)behaviorJson["triggersCombinationControl"],
                        TriggersActivationControl = (string)behaviorJson["triggersActivationControl"] switch
                        {
                            "TRIGGER_ACTIVATE_FIRST_ENTER" => Behavior.TriggersActivationControlType.FirstEnter,
                            "TRIGGER_ACTIVATE_EACH_ENTER" => Behavior.TriggersActivationControlType.EachEnter,
                            "TRIGGER_ACTIVATE_ON" => Behavior.TriggersActivationControlType.On,
                            "TRIGGER_ACTIVATE_FIRST_EXIT" => Behavior.TriggersActivationControlType.FirstExit,
                            "TRIGGER_ACTIVATE_EACH_EXIT" => Behavior.TriggersActivationControlType.EachExit,
                            "TRIGGER_ACTIVATE_OFF" => Behavior.TriggersActivationControlType.Off,
                            _ => Behavior.TriggersActivationControlType.None
                        },
                        ActionsControl = (string)behaviorJson["actionsControl"] switch
                        {
                            "ACTION_ACTIVATE_SEQUENTIAL" => Behavior.ActionsControlType.Sequential,
                            "ACTION_ACTIVATE_PARALLEL" => Behavior.ActionsControlType.Parallel,
                            _ => Behavior.ActionsControlType.Sequential
                        },
                        Priority = (int)behaviorJson["priority"]
                    };
                    foreach (JsonData trigger in behaviorJson["triggers"])
                    {
                        behavior.Triggers.Add((int)trigger);
                    }
                    foreach (JsonData action in behaviorJson["actions"])
                    {
                        behavior.Actions.Add((int)action);
                    }
                    sd.Behaviors.Add(behavior);
                }
                await UniTask.Yield();
            }
        }
    }
}