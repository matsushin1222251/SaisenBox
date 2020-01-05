using UnityEngine;
using System.Collections;

public class ScoreText : MonoBehaviour
{

    private int age=0;
    private int endAge = 0;
    private float speed;
    private float basicSpeed;
    // Use this for initialization
    void Start()
    {
        age = 0;
        endAge = 60;
        speed = 0.2f;
        basicSpeed = speed;
    }
    public void SetScore(string str, float size=1.0f)
    {
        GetComponent<TextMesh>().text = str;
        GetComponent<TextMesh>().fontSize = (int)(8 + 4 * size);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + new Vector3(0, speed, -0.05f);
        speed -= basicSpeed / endAge;
        age++;
        if (age > endAge)
        {
            Destroy(gameObject);
        }
    }
}
