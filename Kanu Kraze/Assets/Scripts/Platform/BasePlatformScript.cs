using UnityEngine;

public class BasePlatformScript : MonoBehaviour
{
    public bool Active = false;
    public GameObject JumpBlocker;
    public int Speed = 0;

    public void Activate()
    {
        Active = true;
        // disable blockers?
    }
}