using UnityEngine;
using System.Collections;

public class JohnScript : MonoBehaviour
{

    public bool Climbing = false;
    public bool InWallBounds = false;

    public GameObject Climber;
    public PlayerScript Player;

    public Animator ClimberAnim;


    // Use this for initialization
    void Start()
    {

    }

    void ToggleClimber(bool state)
    {
        foreach (var renderer in Player.Renderers)
        {
            renderer.enabled = !state;
        }
        Climber.SetActive(state);
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

        if (Input.GetKey(KeyCode.S))
        {
            if (Climbing)
            {
                transform.Translate(new Vector3(0, -Time.deltaTime, 0));
                ClimberAnim.enabled = true;
            }
        }

        if (InWallBounds)
        {
            CheckGrounded();
        }

        if (!Player.onGround && InWallBounds)
        {
            if (Player.RigidBody.velocity.y < -0.5f)
            {
                Player.RigidBody.isKinematic = true;
                Player.RigidBody.velocity = new Vector3(0, 0, 0);
                Climbing = true;
                ToggleClimber(true);
            }
            else
            {
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
        if (Player.onGround)
        {
            var touching = Physics2D.RaycastAll(Player.LegCollider.position, new Vector2(0, -1), 1.4f + Player.DistToGround);

            var vari = false;
            if (touching.Length > 0)
            {
                foreach (var t in touching)
                {
                    //if ((t.distance - distToGround) < 0.70055f)
                    if (t.collider.name != "legs")
                    {
                        vari = true;
                    }
                }
            }

            if (!vari)
            {
                Player.onGround = false;
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
