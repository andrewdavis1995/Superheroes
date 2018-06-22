using UnityEngine;

namespace Assets.Scripts.Platform
{
    public class BasePlatformScript : MonoBehaviour
    {
        public bool Active { get; private set; }
        public GameObject JumpBlocker;
        public int Speed = 0;

        private void Start()
        {
            Active = false;
        }

        public void Activate()
        {
            Active = true;
            // disable blockers?
        }
    }
}