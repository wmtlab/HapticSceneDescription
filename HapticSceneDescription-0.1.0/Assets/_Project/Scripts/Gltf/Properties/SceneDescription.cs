using System.Collections.Generic;
using HapticSceneDescription.Gltf.Adapters;
using HapticSceneDescription.Gltf.Adapters.MpegSceneInteractivity;
using HapticSceneDescription.Gltf.Properties.MpegSceneInteractivity;

namespace HapticSceneDescription.Gltf.Properties
{
    public class SceneDescription
    {
        public int Scene { get; set; } = 0;
        public Scene SceneRef => Scene < Scenes.Count ? Scenes[Scene] : null;
        public List<Scene> Scenes { get; set; } = new List<Scene>();
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<ITrigger> Triggers { get; set; } = new List<ITrigger>();
        public List<IAction> Actions { get; set; } = new List<IAction>();
        public List<Behavior> Behaviors { get; set; } = new List<Behavior>();
        public Dictionary<Node, NodeAdapter> NodeAdapters { get; set; } = new Dictionary<Node, NodeAdapter>();
        public Dictionary<ITrigger, ITriggerAdapter> TriggerAdapters { get; set; } = new Dictionary<ITrigger, ITriggerAdapter>();
        public HapticAdapter HapticAdapter { get; set; }
    }
}