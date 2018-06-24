using UnityEngine;

namespace Assets
{
    public class RequiredIconFlickerScript : MonoBehaviour
    {
        private bool _increasing = false;
        private SpriteRenderer _renderer;

        
        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        
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