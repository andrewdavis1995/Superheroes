using UnityEngine;

public class BasePlatformScript : MonoBehaviour
{
    public bool Active = false;
    public GameObject JumpBlocker;
    public int Speed = 0;

    // Use this for initialization
    void Start()
    {

    }

    public void Activate()
    {
        Active = true;
        // disable blockers?
    }

    // Update is called once per frame
    void Update()
    {

    }
}