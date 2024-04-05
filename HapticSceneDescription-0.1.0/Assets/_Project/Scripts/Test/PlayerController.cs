using UnityEngine;

namespace HapticSceneDescription.Test
{
    public class PlayerController
    {
        private float _speed = 5.0f;
        public InputType inputType = InputType.Keyboard;
        public GeomagicTouchController GeomagicTouch { get; set; }
        public Transform Transform { get; set; }

        public void Start()
        {
            if (inputType == InputType.GeomagicTouch)
            {
                GeomagicTouch?.Start();
            }
        }

        private readonly static Quaternion _rotationLeftCache = Quaternion.Inverse(Quaternion.Euler(0, 180, 0));
        private readonly static Quaternion _rotationRightCache = Quaternion.Euler(-90, 180, 0);
        public void Update()
        {
            if (inputType == InputType.Keyboard)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                Vector3 dir = new Vector3(h, v, 0).normalized;
                Transform.Translate(_speed * Time.deltaTime * dir);
            }
            else if (inputType == InputType.GeomagicTouch)
            {
                if (GeomagicTouch != null && GeomagicTouch.TryGetRawMatrix(out var rawMatrix))
                {
                    var matrix = rawMatrix;
                    Vector3 lossyScale = Transform.lossyScale;
                    Transform.FromMatrix(matrix);
                    Transform.localScale = lossyScale;
                    Transform.rotation = _rotationLeftCache * Transform.rotation * _rotationRightCache;
                    var position = Transform.position;
                    position.x = -position.x;
                    position.z = -position.z;
                    Transform.position = position;
                }
            }
        }

        public void Stop()
        {
            GeomagicTouch?.OnDestroy();
        }

        public enum InputType
        {
            Keyboard,
            GeomagicTouch
        }
    }
}