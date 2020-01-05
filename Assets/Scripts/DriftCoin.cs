using UnityEngine;
using System.Collections;

public class DriftCoin : BasicCoin
{
    float maxSpeed;
    float minSpeed;
    float acc;
    float age;

    override protected void Reflect()
    {

    }

    override protected void Move()
    {
        if (GetComponent<Rigidbody>().position.y < 20)
        {
            direction = 90 * Mathf.Cos((age * 2.0f + 45) * Mathf.Deg2Rad);
        }
        else
        {
            direction = 0;
        }
        float theta = Mathf.Deg2Rad * direction;
        float dy = -1 * speed * Mathf.Cos(theta);        
        float dx = -1 * speed * Mathf.Sin(theta);
        if (Mathf.Abs(dx) > 4)
        {
            dx = Mathf.Sign(dx) * 4;
        }
        Vector3 moveVector = new Vector3(dx, dy, 0);
        Vector3 nextPosition = rigidBody.position + moveVector;
        rigidBody.velocity = moveVector;
        age++;
    }
}
