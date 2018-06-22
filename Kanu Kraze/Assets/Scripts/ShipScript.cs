using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{

    public class ShipScript : MonoBehaviour
    {
        public Transform BiFrost;
        public SpriteRenderer BiFrostRenderer;
        public Animator BiFrostAnimator;
        private Dictionary<Vector3, GameObject> _pickUpPoints = new Dictionary<Vector3, GameObject>();
        private bool _pickUpTime = false;
        private int _speedMultiplier = 1;

        private readonly float _transitionDuration = 2.5f;

        private Vector3 _targetPos;

        bool _goingDown;
        bool _goingUp;

        public GameObject Citizen;

        private float _flashTime = 0;

        // Use this for initialization
        void Start()
        {
            _targetPos = transform.position;
        }

        IEnumerator Retract()
        {
            yield return new WaitForSeconds(1.25f);

            // move passengers up to make space
            var passengers = gameObject.GetComponentsInChildren<Transform>().Where(c => c.tag == "Citizen" || c.tag == "Player");
            foreach (var p in passengers)
            {
                p.Translate(new Vector3(-1f, 0, 0));
            }

            _goingUp = true;
            if (Citizen)
            {
                var rigBod = Citizen.GetComponent<Rigidbody2D>();
                if (rigBod) { rigBod.isKinematic = true; }
            }
        }

        public void Activate()
        {
            BiFrostAnimator.enabled = true;
            _goingDown = true;
            _goingUp = false;
        }

        public void Pickup()
        {
            _pickUpTime = true;
            _speedMultiplier = 2;
            Citizen = _pickUpPoints.FirstOrDefault().Value;
            Activate(_pickUpPoints.FirstOrDefault().Key, true);
            _pickUpPoints.Remove(_pickUpPoints.FirstOrDefault().Key);
        }

        public void NewCollectionPoint(Vector3 position, GameObject player)
        {
            _pickUpPoints.Add(position, player);
            _pickUpPoints = _pickUpPoints.OrderBy(p => p.Key.x).ToDictionary(p => p.Key, p => p.Value);
        }

        // Update is called once per frame
        void Update()
        {
            _flashTime += Time.deltaTime;

            if (_goingDown)
            {
                BiFrostRenderer.transform.localScale += new Vector3(0, .9f, 0) * Time.deltaTime * _speedMultiplier;
                BiFrost.transform.localPosition -= new Vector3(0, 2.2f, 0) * Time.deltaTime * _speedMultiplier;
                if (BiFrostRenderer.transform.localScale.y > 1.8f)
                {
                    _goingDown = false;
                    StartCoroutine(Retract());
                }
            }

            if (_goingUp)
            {
                Ascend();
            }
        }

        void Ascend()
        {
            BiFrostRenderer.transform.localScale -= new Vector3(0, .9f, 0) * Time.deltaTime * _speedMultiplier;
            BiFrost.transform.localPosition += new Vector3(0, 2.2f, 0) * Time.deltaTime * _speedMultiplier;
            if (Citizen)
                Citizen.transform.Translate(new Vector3(0, 3.5f, 0) * Time.deltaTime * _speedMultiplier);

            if (BiFrostRenderer.transform.localScale.y < 0f)
            {
                _goingUp = false;
                BiFrostAnimator.enabled = false;

                if (Citizen)
                {
                    Citizen.transform.parent = transform;
                }
                if (!_pickUpTime || _pickUpPoints.Count == 0)
                {
                    Activate(transform.position + new Vector3(80, 15), false);
                }
                else
                {
                    Pickup();
                }
            }
        }

        internal void Activate(Vector3 target, bool activate)
        {
            if (activate && !_pickUpTime)
            {
                transform.position = target - new Vector3(75, -15);
                _speedMultiplier = 1;
            }
            _targetPos = target;
            StartCoroutine(Transition(activate));
        }

        IEnumerator Transition(bool activate)
        {
            if (!activate)
                yield return new WaitForSeconds(1.2f);

            var t = 0.0f;
            var startingPos = transform.position;

            var duration = _transitionDuration / _speedMultiplier;

            while (t < 1f)
            {
                t += Time.deltaTime * (Time.timeScale / duration);

                var destination = new Vector3(_targetPos.x, _targetPos.y, -40);

                transform.position = Vector3.Lerp(startingPos, destination, t);
                yield return 0;
            }

            if (activate)
            {
                yield return new WaitForSeconds(1.2f);
                Activate();
                _targetPos.x = -9999;
            }

            if (_pickUpTime && _pickUpPoints.Count == 0 && _targetPos.x != -9999)
            {
                GameObject.Find("GameController").GetComponent<GameControllerScript>().LevelCompleted();
            }

        }
    }
}