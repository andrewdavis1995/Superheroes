using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{

    public class JohnScript : MonoBehaviour
    {

        public bool Climbing { get; private set; }
        public bool InWallBounds { get; private set; }

        public GameObject Climber;
        public PlayerScript Player;

        public Animator ClimberAnim;

        void ToggleClimber(bool state)
        {
            foreach (var renderer in Player.Renderers)
            {
                renderer.enabled = !state;
            }
            Climber.SetActive(state);
        }

        private void Start()
        {
            Climbing = false;
            InWallBounds = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!Player.Active) return;

            if (Input.GetKey(KeyCode.W))
            {
                if (Climbing)
                {
                    transform.Translate(new Vector3(0, Time.deltaTime, 0));
                    ClimberAnim.enabled = true;
                }
            }

            if (Input.GetKey(KeyCode.S) && Climbing)
            {
                transform.Translate(new Vector3(0, -Time.deltaTime, 0));
                ClimberAnim.enabled = true;
            }

            if (InWallBounds)
            {
                CheckGrounded();
            }

            if (!Player.OnGround && InWallBounds)
            {
                if (Player.RigidBody.velocity.y < -0.5f)
                {
                    Player.RigidBody.isKinematic = true;
                    Player.RigidBody.velocity = new Vector3(0, 0, 0);
                    Climbing = true;
                    ToggleClimber(true);
                }
            }
            else
            {
                Player.RigidBody.isKinematic = false;
                Climbing = false;
                ToggleClimber(false);
            }
        }

        void CheckGrounded()
        {
            if (Player.OnGround)
            {
                var touching = Physics2D.RaycastAll(Player.LegCollider.position, new Vector2(0, -1), 1.4f + Player.DistToGround);

                var vari = false;
                if (touching.Length > 0)
                {
                    foreach (var t in touching)
                    {
                        if (t.collider.name != "legs")
                        {
                            vari = true;
                        }
                    }
                }

                if (!vari)
                {
                    Player.OnGround = false;
                    Player.Renderers[1].sprite = Player.HeadImages[1];
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.tag == "Wall")
            {
                InWallBounds = false;
            }
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "Wall")
            {
                InWallBounds = true;
            }
        }
    }
}