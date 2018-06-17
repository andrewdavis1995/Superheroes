using UnityEngine;

public class AndrewScript : MonoBehaviour
{
    public PlayerScript Player;
    private FirewallScript _inPanelBounds;
    public SpriteRenderer Hacker;
    public bool Hacking;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (_inPanelBounds && Player.Active)
            {
                _inPanelBounds.Activate(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Panel" && Player.Active)
        {
            _inPanelBounds = null;
            collision.GetComponentInParent<FirewallScript>().Instruction.SetActive(false);
        }
    }

    internal void Hack()
    {
        Hacking = true;
        foreach(var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = false;
        }
        Hacker.enabled = true;
    }

    internal void DoneHacking()
    {
        Hacking = false;
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = true;
        }
        Hacker.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Panel" && Player.Active)
        {
            var firewall = collision.gameObject.GetComponentInParent<FirewallScript>();
            if (!firewall.Pressed)
            {
                _inPanelBounds = firewall;
                firewall.Instruction.SetActive(true);
            }
        }
    }
}