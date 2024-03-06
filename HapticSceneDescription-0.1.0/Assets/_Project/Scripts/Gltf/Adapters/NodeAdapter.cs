using HapticSceneDescription.Gltf.Properties;
using UnityEngine;

namespace HapticSceneDescription.Gltf.Adapters
{
    public class NodeAdapter : MonoBehaviour
    {
        private static Mesh _defaultMesh;
        private static Material _defaultMaterial;
        private Node _node;
        private MeshFilter _filter;
        private MeshRenderer _renderer;
        public void Init(SceneDescription sd, Node node)
        {
            _node = node;
            gameObject.name = node.Name;
            if (node.Parent >= 0)
            {
                transform.SetParent(sd.NodeAdapters[sd.Nodes[node.Parent]].transform);
            }
            transform.localPosition = node.Translation;
            transform.localRotation = node.Rotation;
            transform.localScale = node.Scale;

            if (node.Mesh >= 0)
            {
                PrepareDefault();
                _filter = gameObject.AddComponent<MeshFilter>();
                _filter.sharedMesh = _defaultMesh;
                _renderer = gameObject.AddComponent<MeshRenderer>();
                _renderer.sharedMaterial = _defaultMaterial;
            }

        }

        public void SetSceneRoot(Transform root)
        {
            if (_node.Parent < 0)
            {
                transform.SetParent(root);
            }
            else
            {
                Debug.LogWarning("Node " + _node.Name + " is not a root node");
            }
        }

        private static void PrepareDefault()
        {
            if (_defaultMesh == null || _defaultMaterial == null)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _defaultMesh = go.GetComponent<MeshFilter>().sharedMesh;
                _defaultMaterial = new Material(go.GetComponent<MeshRenderer>().sharedMaterial)
                {
                    color = new Color(1f, 1f, 1f, 0.5f)
                };
                #region RenderingMode Transparent
                _defaultMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                _defaultMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _defaultMaterial.SetInt("_ZWrite", 0);
                _defaultMaterial.DisableKeyword("_ALPHATEST_ON");
                _defaultMaterial.DisableKeyword("_ALPHABLEND_ON");
                _defaultMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                _defaultMaterial.renderQueue = 3000;
                #endregion
                Destroy(go);
            }
        }
    }
}