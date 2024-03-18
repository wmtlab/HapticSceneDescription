using UnityEngine;

namespace HapticSceneDescription.Test
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 5.0f;
        public InputType inputType = InputType.Keyboard;
        public GeomagicTouchController GeomagicTouch { get; set; }
        private Vector3 _startPosition;

        private void Start()
        {
            _startPosition = transform.position;
        }

        void Update()
        {
            if (inputType == InputType.Keyboard)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                Vector3 dir = new Vector3(h, v, 0).normalized;
                transform.Translate(_speed * Time.deltaTime * dir);
            }
            else if (inputType == InputType.GeomagicTouch)
            {
                if (GeomagicTouch != null && GeomagicTouch.TryGetTouchPosition(out var geomagicPosition))
                {
                    var position = new Vector3(
                        _startPosition.x - geomagicPosition.x,
                        _startPosition.y + geomagicPosition.y,
                        _startPosition.z + geomagicPosition.z);

                    transform.position = position;
                }
            }
        }

        public enum InputType
        {
            Keyboard,
            GeomagicTouch
        }
    }
}