using System;
using System.Collections.Generic;
using UnityEngine;

namespace HapticSceneDescription.Gltf.Properties.MpegSceneInteractivity
{
    public interface ITrigger
    {
        bool IsTriggered();
        event Func<bool> OnUpdate;
        bool Update();
    }
    public class CollisionTrigger : ITrigger
    {
        public List<int> Nodes { get; set; } = new List<int>();
        public List<ICollisionPrimitive> Primitives { get; set; } = new List<ICollisionPrimitive>();
        private bool _isTriggered = false;
        public bool IsTriggered()
        {
            return _isTriggered;
        }
        public event Func<bool> OnUpdate;
        public bool Update()
        {
            if (OnUpdate != null)
            {
                bool whenAny = false;
                foreach (var update in OnUpdate.GetInvocationList())
                {
                    whenAny |= (bool)update.DynamicInvoke();
                }
                _isTriggered = whenAny;
            }
            return _isTriggered;
        }
        public interface ICollisionPrimitive { }
        public class BvSpheroid : ICollisionPrimitive
        {
            public float Radius { get; set; } = 1.0f;
            public Vector3 Centroid { get; set; } = Vector3.zero;
        }
    }
}