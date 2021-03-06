﻿using UnityEngine;

namespace Assets.Scripts.Platform
{
    public class HorizontalMovingPlatform : BasePlatformScript
    {
        public int LeftX = 0;
        public int RightX = 0;

        bool MovingRight = false;

        
        void Update()
        {
            if (Active)
            {
                if (MovingRight)
                {
                    transform.Translate(new Vector3(Speed * Time.deltaTime, 0));
                    if (transform.position.x > RightX)
                    {
                        MovingRight = false;
                    }
                }
                else
                {
                    transform.Translate(new Vector3(-Speed * Time.deltaTime, 0));
                    if (transform.position.x < LeftX)
                    {
                        MovingRight = true;
                    }
                }
            }
        }
    }
}