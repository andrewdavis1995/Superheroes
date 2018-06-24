using UnityEngine;

namespace Assets.Scripts.Platform
{
    public class VerticalMovingPlatform : BasePlatformScript
    {
        public int TopY = 0;
        public int BottomY = 0;

        bool MovingUp = true;

        
        void Update()
        {
            if (Active)
            {
                if (MovingUp)
                {
                    transform.Translate(new Vector3(0, Speed * Time.deltaTime));
                    if (transform.position.y > TopY)
                    {
                        MovingUp = false;
                    }
                }
                else
                {
                    transform.Translate(new Vector3(0, -Speed * Time.deltaTime));
                    if (transform.position.y < BottomY)
                    {
                        MovingUp = true;
                    }
                }
            }
        }
    }
}