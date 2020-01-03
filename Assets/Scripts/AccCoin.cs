using UnityEngine;
using System.Collections;

public class AccCoin : BasicCoin
{
    float maxSpeed;
    float minSpeed;
    float acc;
    override public void SetParam(float spd, float dir)
    {
        base.SetParam(spd, dir);
        minSpeed = spd;
        maxSpeed = spd * 2;
        acc = (maxSpeed - minSpeed) / 300;
    }

    override protected void Move()
    {
        speed += acc;
        base.Move();

    }
}
