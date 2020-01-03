using UnityEngine;
using System.Collections;

public class SaisenBox: MonoBehaviour
{
    public float speed = 5f;
    Rigidbody rigidBody;
    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 objPosition = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 mouseMaxPosition = Camera.main.WorldToScreenPoint(new Vector3(8, 0, 0));
            Vector3 mouseMinPosition = Camera.main.WorldToScreenPoint(new Vector3(-8, 0, 0));

            //x_s = x_w*r + a 
            float Screen2WorldRate = (mouseMaxPosition.x - mouseMinPosition.x);

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            float dX = mouseWorldPosition.x - transform.position.x;
            if (Mathf.Abs(dX) > speed)
            {
                rigidBody.MovePosition(new Vector3(Mathf.Sign(dX) * speed + transform.position.x, 0, 0));
            }
            else
            {
                rigidBody.MovePosition(new Vector3(mouseWorldPosition.x, 0, 0));
            }
        }
    }
}
