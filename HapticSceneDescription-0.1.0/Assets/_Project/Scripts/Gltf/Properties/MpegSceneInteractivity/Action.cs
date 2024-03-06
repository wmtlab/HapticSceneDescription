using System;
using System.Collections.Generic;

namespace HapticSceneDescription.Gltf.Properties.MpegSceneInteractivity
{
    public interface IAction
    {
        void Invoke();
    }
    public interface IAction<T> : IAction where T : IAction<T>
    {
        event Action<T> ActionHandler;
    }
    public class SetHapticAction : IAction<SetHapticAction>
    {
        public List<HapticActionNode> HapticActionNodes { get; set; } = new List<HapticActionNode>();
        public event Action<SetHapticAction> ActionHandler;
        public void Invoke()
        {
            ActionHandler?.Invoke(this);
        }
        public class HapticActionNode
        {
            public int Node { get; set; } = -1;
            public List<HapticActionMedia> HapticActionMedias { get; set; } = new List<HapticActionMedia>();
            public class HapticActionMedia
            {
                public int MediaIndex { get; set; } = -1;
                public List<int> PerceptionIndices { get; set; } = new List<int>();
            }
        }
    }
}