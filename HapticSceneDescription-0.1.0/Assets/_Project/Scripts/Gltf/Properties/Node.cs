using System.Collections.Generic;
using UnityEngine;

namespace HapticSceneDescription.Gltf.Properties
{
    public class Node
    {
        public string Name { get; set; } = string.Empty;
        public Quaternion Rotation { get; set; } = Quaternion.identity;
        public Vector3 Scale { get; set; } = Vector3.one;
        public Vector3 Translation { get; set; } = Vector3.zero;
        public int Parent { get; set; } = -1;
        public List<int> Children { get; set; } = new List<int>();
        public int Mesh { get; set; } = -1;
    }
}