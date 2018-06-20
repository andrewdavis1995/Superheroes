using System;
using UnityEngine;

namespace Assets.Scripts
{

    public class FollowScript : MonoBehaviour
    {
        private float _xPosition = -9999;
        private float _timeCount = 1;

        private PlayerScript _target;
        private PlayerScript _player;

        private float _interval = 2.5f;
        private float _boundaries = 2.5f;

        // Use this for initialization
        void Start()
        {
            _player = GetComponent<PlayerScript>();
        }

        public void SetTarget(PlayerScript target)
        {
            _target = target;
        }

        // Update is called once per frame
        void Update()
        {
            // don't follow if dead
            if (_target == null || !_target.Alive)
                return;

            // don't follow if busy
            if (_player.JohnScript != null && _player.JohnScript.Climbing)
                return;
            if (_player.AndrewScript != null && _player.AndrewScript.Hacking)
                return;

            _timeCount += Time.deltaTime;
            if (_timeCount > _interval)
            {
                _timeCount = 0;
                if (_target.onGround || (_target.JohnScript != null && _target.JohnScript.Climbing))
                {
                    int skipFactor = UnityEngine.Random.Range(0, 7);
                    if (skipFactor != 0) // randomise
                    {
                        _xPosition = _target.transform.position.x;
                        _interval = UnityEngine.Random.Range(3f, 7f);
                        _boundaries = UnityEngine.Random.Range(1f, 2f);
                    }
                }
            }

            if (_xPosition == -9999) return;

            if (transform.position.x < _xPosition - _boundaries)
            {
                if (!_player.Walking)
                    _player.BeginWalk();
                transform.Translate(new Vector3(2.7f * Time.deltaTime, 0, 0));
                foreach (var r in _player.Renderers)
                {
                    r.flipX = false;
                }
                _player.Walking = true;
            }
            else if (transform.position.x > _xPosition + _boundaries)
            {
                if (!_player.Walking)
                    _player.BeginWalk();
                transform.Translate(new Vector3(-2.7f * Time.deltaTime, 0, 0));
                foreach (var r in _player.Renderers)
                {
                    r.flipX = true;
                }
                _player.Walking = true;
            }
            else
            {
                _xPosition = -9999;
                if (_player.Walking)
                    _player.StopWalk();
                _player.Walking = false;
            }
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "JumpTrigger" && _player != null && !_player.Active)
            {
                _player.Jump(1);
            }
            if (collision.transform.tag == "JumpStopper" && _player != null && !_player.Active)
            {
                var bounceBack = UnityEngine.Random.Range(2.2f, 4.5f);
                _xPosition = transform.position.x < _xPosition ? transform.position.x - bounceBack : transform.position.x + bounceBack;
            }
        }

        internal void Reset()
        {
            enabled = false;
            _xPosition = -9999;
        }
    }
}