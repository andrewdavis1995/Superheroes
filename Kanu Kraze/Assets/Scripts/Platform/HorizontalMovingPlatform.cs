using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovingPlatform : BasePlatformScript
{
    public int LeftX = 0;
    public int RightX = 0;

    bool MovingRight = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            if (MovingRight)
            {
                transform.Translate(new Vector3(Speed * Time.deltaTime, 0));
                if(transform.position.x > RightX)
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