using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Platform;

namespace Assets.Scripts
{

    public class PlayerScript : MonoBehaviour
    {
        public GameControllerScript Controller;

        public Image StatusImage;
        public Image HealthImage;
        public Sprite[] StatusImages;   // inactive, active, dead
        public Sprite[] HeadImages;   // inactive, active, dead

        // components
        public SpriteRenderer[] Renderers;

        public Animator[] animators; // 0 = close arm, 1 = far arm, 2 legs
        public Rigidbody2D RigidBody;
        Quaternion rotation;
        public Transform LegCollider;

        // status

        public bool OnGround { get; set; }
        public bool Stunned { get; private set; }
        public bool Camera { get; private set; }

        public bool Active { get; private set; }
        public bool Alive { get; private set; }
        public bool Complete { get; private set; }
        public bool PunchRequired { get; set; }
        public bool Walking { get; set; }

        // player scripts
        public JohnScript JohnScript;
        public AndrewScript AndrewScript;
        public FraserScript FraserScript;
        public FollowScript FollowScript;

        // attributes
        readonly float WALK_SPEED = 2.7f;
        readonly float JUMP_SPEED = 1000;
        public float LEFT_LIMIT;

        public Sprite[] PunchImages;

        private readonly List<GameObject> InPunchRange = new List<GameObject>();
        private GameObject ActiveButton;
        private float Health = 100;

        // misc
        public float DistToGround;


        // Use this for initialization
        void Start()
        {
            // bool init
            Stunned = true;
            Alive = true;
            Complete = false;
            PunchRequired = false;
            Camera = true;
            Walking = true;

            RigidBody = transform.GetComponent<Rigidbody2D>();
            rotation = transform.rotation;
            animators = transform.GetComponentsInChildren<Animator>();

            LegCollider = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.gameObject.name == "legs");

            DistToGround = LegCollider.GetComponent<Collider2D>().bounds.extents.y;

            Renderers = GetComponentsInChildren<SpriteRenderer>();

            DisablePlayerCollisions();
        }

        private void DisablePlayerCollisions()
        {
            var currentColliders = GetComponentsInChildren<BoxCollider2D>();

            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                var playerColliders = GetComponentsInChildren<BoxCollider2D>();
                foreach (var pC in playerColliders)
                {
                    foreach (var cC in currentColliders)
                    {
                        Physics2D.IgnoreCollision(cC, pC);
                    }
                }
            }
        }

        public void Follow(PlayerScript target)
        {
            if (!Alive) return;

            Active = false;
            // enable following
            FollowScript.enabled = true;
            FollowScript.SetTarget(target);
            StatusImage.sprite = StatusImages[0];
        }

        public void BeginWalk()
        {
            Walking = true;
            animators[2].SetTrigger("Walk");
            animators[1].SetTrigger("Walk");
            animators[0].SetTrigger("Walk");
        }

        public void Activate()
        {
            // disable following
            FollowScript.Reset();
            // set follow target to -9999
            Active = true;
            StatusImage.sprite = StatusImages[1];
        }

        public void StopWalk()
        {
            Walking = false;
            animators[2].SetTrigger("Stop");
            animators[1].SetTrigger("Stop");
            animators[0].SetTrigger("Stop");
        }

        // Update is called once per frame
        void Update()
        {
            transform.rotation = rotation;

            if (Active && !Stunned)
            {
                if (transform.position.x < LEFT_LIMIT)
                {
                    transform.position = new Vector3(LEFT_LIMIT, transform.position.y, transform.position.z);
                }

                var dampener = 1f;

                if (JohnScript != null && JohnScript.Climbing)
                {
                    // slow down
                    dampener = .5f;
                    JohnScript.ClimberAnim.enabled = true;
                }

                if (AndrewScript != null && AndrewScript.Hacking) { return; } // can't do anything
                if (FraserScript != null && FraserScript.Building) { return; } // can't do anything

                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
                {
                    if (!Walking)
                    {
                        BeginWalk();
                    }

                    if (Input.GetKey(KeyCode.D))
                    {
                        transform.Translate(new Vector2(dampener * WALK_SPEED * Time.deltaTime, 0));
                        foreach (var r in Renderers)
                        {
                            r.flipX = false;
                        }
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        transform.Translate(new Vector2(dampener * -WALK_SPEED * Time.deltaTime, 0));
                        foreach (var r in Renderers)
                        {
                            r.flipX = true;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.W) && OnGround)
                {
                    Jump(1 * dampener);
                }

                if (!OnGround)
                {
                    StartCoroutine(JumpAnim());
                }

                if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    if (Walking)
                        StopWalk();

                    if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                    {
                        if (JohnScript != null)
                        {
                            JohnScript.ClimberAnim.enabled = false;
                        }
                    }

                }

                HandlePunch();
                HandleButtonPush();
            }
        }

        private void HandleButtonPush()
        {
            if (Input.GetKeyDown(KeyCode.E) && ActiveButton != null)
            {
                ActiveButton.GetComponent<ButtonScript>().Press();
                ActiveButton = null;
            }
        }

        private bool _punching;

        private void HandlePunch()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_punching)
            {
                StartCoroutine(DoPunch());
            }
        }

        IEnumerator DoPunch()
        {
            _punching = true;
            PunchRequired = false;
            animators[1].enabled = false;

            for (var i = 0; i < InPunchRange.Count; i++)
            {
                var impact = InPunchRange[i];

                if (impact != null)
                {
                    // get enemy and remove health
                    // get statue
                    var citizen = impact.GetComponent<CitizenScript>();
                    if (citizen != null)
                    {
                        var destroyed = citizen.Hit(15);
                        if (destroyed) InPunchRange.Remove(impact);
                    }
                }
                else
                {
                    // remove already removed items
                    InPunchRange.RemoveAt(i);
                }
            }

            // loop through images
            for (int i = 0; i < PunchImages.Length; i++)
            {
                Renderers[3].sprite = PunchImages[i];
                yield return new WaitForSeconds(.025f);
            }

            animators[1].enabled = true;
            _punching = false;
            yield return new WaitForSeconds(0);
        }

        private IEnumerator JumpAnim()
        {
            yield return new WaitForEndOfFrame();
        }

        public void Jump(float scale)
        {
            CheckGrounded();

            if (OnGround)
            {
                RigidBody.AddForce(new Vector2(0, JUMP_SPEED * scale));
                OnGround = false;
                Renderers[1].sprite = HeadImages[1];
            }
        }

        void CheckGrounded()
        {
            if (OnGround)
            {
                var touching = Physics2D.RaycastAll(LegCollider.position, new Vector2(0, -1), 1.4f + DistToGround);

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
                    OnGround = false;
                    Renderers[1].sprite = HeadImages[1];
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            InPunchRange.Remove(collision.gameObject);

            if (collision.transform.tag == "Button")
            {
                if (!collision.gameObject.GetComponent<ButtonScript>().Pressed)
                {
                    ActiveButton = null;
                    collision.gameObject.GetComponent<ButtonScript>().Instruction.SetActive(false);
                }
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            InPunchRange.Add(collision.gameObject);

            if (collision.transform.tag == "Button")
            {
                if (!collision.gameObject.GetComponent<ButtonScript>().Pressed)
                {
                    ActiveButton = collision.gameObject;
                    collision.gameObject.GetComponent<ButtonScript>().Instruction.SetActive(true);
                }
            }

            var beamer = collision.GetComponentsInParent<Transform>();
            if (collision.transform.tag == "BeamMeUp" && Active && !collision.name.Contains("COMPLETE") && beamer.LastOrDefault().name.ToLower().Contains(name.ToLower()))
            {
                var width = collision.GetComponentsInParent<Transform>().LastOrDefault().localScale.x;
                var left = collision.GetComponentsInParent<Transform>().LastOrDefault().localPosition.x;
                Camera = false;
                var playerWidth = transform.localScale.x;
                var widthDifference = width - playerWidth;
                Complete = true;
                transform.position = new Vector3((left + widthDifference / 2) - 0.12f, transform.position.y, transform.position.z);
                collision.GetComponentInParent<Animator>().SetTrigger("Stop");
                collision.name += "COMPLETE";
                StartCoroutine(SummonShip());
                StatusImage.sprite = StatusImages[3];
            }

            if (collision.transform.tag == "Death")
            {
                Die();
            }
            if (collision.transform.tag == "Stalagtite")
            {
                collision.GetComponent<Rigidbody2D>().isKinematic = false;
            }

            if (collision.transform.tag == "EnemyDestroy")
            {
                collision.gameObject.GetComponentsInParent<BoxCollider2D>().LastOrDefault().enabled = false;

                int force = 2000;
                GetComponent<Rigidbody2D>().AddForce(new Vector3(0, force));
                collision.gameObject.GetComponentsInParent<Rigidbody2D>().LastOrDefault().AddForce(new Vector2(0, -force * 5));
            }
        }

        public void Die()
        {
            Alive = false;
            Health = 0;
            HealthImage.fillAmount = 0;

            bool playersLeft = true;
            if (!Controller.Players.Any(p => p.Alive))
            {
                // UH OH! Game over
                if (Controller.Players.Any(p => p.Complete))
                {
                    Debug.Log("Completed");
                    ShipScript ship = GameObject.Find("Ship").GetComponent<ShipScript>();
                    GameObject.Find("Main Camera").GetComponent<CameraScript>().ChangePlayer(null);
                    ship.Pickup();
                    enabled = false;
                }
                else
                {
                    GameObject.Find("GameController").GetComponent<GameControllerScript>().LevelFailedPopup.SetActive(true);
                }

                playersLeft = false;

                StatusImage.sprite = StatusImages[2];
            }

            if (playersLeft)
            {
                if (Active)
                {
                    var player = Controller.NextPlayer();

                    foreach (var pl in Controller.Players)
                    {
                        pl.FollowScript.SetTarget(null);
                    }

                    while (!player.Alive)
                    {
                        player = Controller.NextPlayer();
                    }
                    player.Activate();
                }
                Controller.CameraScript.ChangePlayer(Controller.Players[Controller.SelectedPlayer]);
            }
            StatusImage.sprite = StatusImages[2];
            foreach (var bc in GetComponentsInChildren<BoxCollider2D>())
            {
                bc.isTrigger = true;
            }
            Active = false;

            var beamUp = GameObject.Find("beamUp" + name);
            if (beamUp)
            {
                beamUp.GetComponent<Animator>().SetTrigger("Stop");
                beamUp.name += "COMPLETE";
            }

            if (FraserScript)
                FraserScript.Die();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var vel = collision.relativeVelocity;

            if (collision.gameObject.tag.ToLower().Contains("platform") && vel.y > 0) //  && vel.y < 0
            {
                Renderers[1].sprite = HeadImages[0];
                OnGround = true;
                Stunned = false;
                if (!collision.gameObject.name.ToLower().Contains("temp"))
                    transform.parent = collision.transform;
            }

            if (collision.transform.tag == "BouncyPlatform" && vel.y < 0)
            {
                var anim = collision.transform.GetComponent<Animator>();
                anim.SetTrigger("Land");
                Jump(1.4f);
                anim.SetTrigger("Stop");
            }

            if (collision.transform.tag == "Stalagtite")
            {
                HealthLost(40);
                collision.gameObject.GetComponent<StalagtiteScript>().Crash(true);
            }

            if (collision.transform.tag == "Enemy")
            {
                PunchRequired = true;
                collision.gameObject.GetComponent<EnemyScript>().Punch(this, collision);
            }
        }

        public IEnumerator Hit(Collision2D collision, int damage)
        {
            HealthLost(damage);
            Stunned = true;

            int force = 1000;

            Vector3 dir = collision.contacts[0].point - new Vector2(transform.position.x, transform.position.y);
            dir = -dir.normalized;
            GetComponent<Rigidbody2D>().AddForce(dir * force);
            yield return 0;
        }

        IEnumerator SummonShip()
        {
            Active = false;
            Alive = false;
            foreach (var a in animators) { a.SetTrigger("Stop"); }
            foreach (var r in Renderers) { r.flipX = false; }
            Camera = false;

            ShipScript ship = GameObject.Find("Ship").GetComponent<ShipScript>();
            ship.NewCollectionPoint(new Vector3(transform.position.x + 1.44f, transform.position.y + 6.917f, -20), gameObject);

            var player = Controller.NextPlayer();

            foreach (var pl in Controller.Players)
            {
                pl.FollowScript.SetTarget(null);
            }

            if (Controller.Players.Any(p => p.Alive))
            {
                while (!player.Alive)
                {
                    player = Controller.NextPlayer();
                }
                player.Activate();
                player.Camera = true;
            }
            else
            {
                GameObject.Find("Main Camera").GetComponent<CameraScript>().ChangePlayer(null);
                ship.Pickup();
                enabled = false;
                yield return 0;
            }

            FollowScript.enabled = false;

            GameObject.Find("Main Camera").GetComponent<CameraScript>().ChangePlayer(player);

            enabled = false;

            yield return 0;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag.ToLower().Contains("platform")) //  && vel.y < 0
            {
                transform.parent = null;
            }
        }

        private void HealthLost(int amount)
        {
            Health -= amount;
            HealthImage.fillAmount = Health / 100;

            if (Health < 1)
            {
                Die();
            }
        }

        public void HealthGained(int amount)
        {
            Health += amount;
            if (Health > 100)
            {
                Health = 100;
            }
            HealthImage.fillAmount = Health / 100;
        }

    }
}