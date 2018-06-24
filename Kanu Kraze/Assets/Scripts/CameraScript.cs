using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{

    public class CameraScript : MonoBehaviour
    {
        private PlayerScript _target;
        private bool _followShip = false;
        private bool _transitioning;
        private readonly float _transitionDuration = .65f;
        private Transform _ship;
        public Camera Camera;
        public static bool Zooming { get; set; }

        
        void Start()
        {
            Zooming = false;
            _ship = GameObject.Find("Ship").transform;
        }

        
        void Update()
        {
            if (!_transitioning && _target && _target.Alive)
            {
                transform.position = new Vector3(_target.transform.position.x + 5f, _target.transform.position.y + 2f, -50);
                if (!Zooming)
                    Camera.orthographicSize = 6.12f;
            }
            else if (_followShip)
            {
                transform.position = new Vector3(_ship.transform.position.x + 5f, _ship.transform.position.y + -2f, -60);
                if (!Zooming)
                    Camera.orthographicSize = 8.1f;
            }
        }

        public void ChangePlayer(PlayerScript target)
        {
            if (target)
            {
                _target = target;
            }
            else
            {
                _followShip = true;
                _target = null;
            }
            _transitioning = true;
            StartCoroutine(Transition());
        }


        public bool IsTransitioning() { return _transitioning; }

        IEnumerator Transition()
        {
            Transform target = _target ? _target.transform : _ship;

            var t = 0.0f;
            var startingPos = transform.position;
            while (t < 1f)
            {
                t += Time.deltaTime * (Time.timeScale / _transitionDuration);

                var destination = new Vector3(target.transform.position.x + 5f, target.transform.position.y + 2f, -60);

                transform.position = Vector3.Lerp(startingPos, destination, t);
                yield return 0;
            }
            _transitioning = false;
        }
    }
}