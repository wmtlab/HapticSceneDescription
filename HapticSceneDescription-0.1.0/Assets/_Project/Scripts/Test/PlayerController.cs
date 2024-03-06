using UnityEngine;

namespace HapticSceneDescription.Test
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 5.0f;

        void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 dir = new Vector3(h, v, 0).normalized;
            transform.Translate(_speed * Time.deltaTime * dir);
        }
    }
}