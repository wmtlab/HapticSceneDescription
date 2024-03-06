using System.Collections.Generic;

namespace HapticSceneDescription.Gltf.Properties
{
    public class Scene
    {
        public string Name { get; set; } = string.Empty;
        public List<int> Nodes { get; set; } = new List<int>();
    }
}