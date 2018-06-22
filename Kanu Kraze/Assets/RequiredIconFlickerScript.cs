using UnityEngine;

namespace Assets
{
    public class RequiredIconFlickerScript : MonoBehaviour
    {
        private bool _increasing = false;
        private SpriteRenderer _renderer;

        // Use this for initialization
        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_increasing)
            {
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, _renderer.color.a + 0.012f);
                if (_renderer.color.a > 1) { _increasing = false; }
            }
            else
            {
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, _renderer.color.a - 0.012f);
                if (_renderer.color.a < 0) { _increasing = true; }
            }
        }
    }
}