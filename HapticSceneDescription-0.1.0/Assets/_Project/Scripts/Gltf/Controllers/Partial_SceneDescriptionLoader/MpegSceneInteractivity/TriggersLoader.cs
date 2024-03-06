using Cysharp.Threading.Tasks;
using HapticSceneDescription.Gltf.Adapters.MpegSceneInteractivity;
using HapticSceneDescription.Gltf.Properties;
using HapticSceneDescription.Gltf.Properties.MpegSceneInteractivity;
using LitJson;
using UnityEngine;

namespace HapticSceneDescription.Gltf.Controllers
{
    public static partial class SceneDescriptionLoader
    {
        private static class TriggersLoader
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
                if (!mpegSceneInteractivity.ContainsKey("triggers"))
                {
                    return;
                }
                JsonData triggers = mpegSceneInteractivity["triggers"];
                foreach (JsonData triggerJson in triggers)
                {
                    ITrigger trigger = TriggerFactory(sd, triggerJson);
                    sd.Triggers.Add(trigger);
                }
                await UniTask.Yield();
            }

            private static ITrigger TriggerFactory(SceneDescription sd, JsonData triggerJson)
            {
                string type = (string)triggerJson["type"];
                ITrigger trigger = null;
                switch (type)
                {
                    case "TRIGGER_COLLISION":
                        trigger = CollisionTriggerFactory(sd, triggerJson);
                        break;
                }
                return trigger;
            }

            private static CollisionTrigger CollisionTriggerFactory(SceneDescription sd, JsonData triggerJson)
            {
                CollisionTrigger trigger = new CollisionTrigger();
                foreach (JsonData nodeIdx in triggerJson["nodes"])
                {
                    trigger.Nodes.Add((int)nodeIdx);
                }

                foreach (JsonData primitiveJson in triggerJson["primitives"])
                {
                    string primitiveType = (string)primitiveJson["type"];
                    switch (primitiveType)
                    {
                        case "BV_SPHEROID":
                            CollisionTrigger.BvSpheroid spheroid = new CollisionTrigger.BvSpheroid
                            {
                                Radius = (float)(double)primitiveJson["radius"],
                                Centroid = new Vector3
                                {
                                    x = (float)(double)primitiveJson["centroid"][0],
                                    y = (float)(double)primitiveJson["centroid"][1],
                                    z = (float)(double)primitiveJson["centroid"][2]
                                }
                            };
                            trigger.Primitives.Add(spheroid);
                            break;
                    }
                }

                for (int i = 0; i < trigger.Nodes.Count; i++)
                {
                    var go = sd.NodeAdapters[sd.Nodes[trigger.Nodes[i]]].gameObject;
                    if (i >= trigger.Primitives.Count)
                    {
                        break;
                    }
                    var rb = go.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;

                    var primitive = trigger.Primitives[i];

                    switch (primitive)
                    {
                        case CollisionTrigger.BvSpheroid spheroid:
                            var sphereCollider = go.AddComponent<SphereCollider>();
                            sphereCollider.isTrigger = true;
                            sphereCollider.radius = spheroid.Radius;
                            sphereCollider.center = spheroid.Centroid;
                            break;
                    }
                    var triggerAdapter = go.AddComponent<CollisionTriggerAdapter>();
                    trigger.OnUpdate += triggerAdapter.IsTriggered;
                    sd.TriggerAdapters.Add(trigger, triggerAdapter);
                }

                return trigger;
            }

        }
    }
}