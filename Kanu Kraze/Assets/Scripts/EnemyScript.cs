using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{

    public class EnemyScript : MonoBehaviour
    {
        public bool Punching = false;
        public SpriteRenderer _renderer;
        public Animator _animator;
        public int Health;
        public int Damage;

        private Collision2D _collision;

        private PlayerScript _fightingPlayer;

        public float Reflexes;

        public Sprite[] PunchImages;

        Quaternion rotation;

        
        void Start()
        {
            rotation = transform.rotation;
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        IEnumerator DoPunch()
        {
            _animator.enabled = false;

            // loop through images
            for (int i = 0; i < PunchImages.Length; i++)
            {
                _renderer.sprite = PunchImages[i];
                yield return new WaitForSeconds(.025f);
            }

            _animator.enabled = true;
            Punching = false;
            yield return 0;
        }

        public void Punch(PlayerScript player, Collision2D collision)
        {
            _fightingPlayer = player;
            _collision = collision;
            StartCoroutine(WaitForPunch());
        }

        private IEnumerator WaitForPunch()
        {
            yield return new WaitForSeconds(Reflexes);
            if (_fightingPlayer.PunchRequired)
            {
                Punching = true;
                StartCoroutine(DoPunch());
                StartCoroutine(_fightingPlayer.Hit(_collision, Damage));
            }
            else
            {
                Health -= 40;
                if (Health <= 0)
                {
                    var coll = GetComponentsInChildren<Collider2D>();
                    foreach (var c in coll)
                    {
                        c.enabled = false;
                    }
                }
                else
                {
                    Vector3 dir = _collision.contacts[0].point - new Vector2(transform.position.x, transform.position.y);
                    dir = -dir.normalized;
                    GetComponent<Rigidbody2D>().AddForce(dir * 2200);
                }

            }
            _fightingPlayer.PunchRequired = false;
            _fightingPlayer = null;
            _collision = null;
        }

        
        void Update()
        {
            transform.rotation = rotation;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "Death")
            {
                Destroy(gameObject);
            }
        }
    }
}