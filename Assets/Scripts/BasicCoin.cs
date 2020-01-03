using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicCoin : MonoBehaviour
{
    public enum CoinState {Run, Got, Fallen, Idle};


    public float direction=0;
    public float speed=2f;
    [SerializeField]
    public int value=1;
    protected bool finished =false;
    protected TextMesh valueText;
    protected Rigidbody rigidBody;
    public GameObject saisenBox;
    public CoinState state=CoinState.Run;

    virtual public void SetParam(float spd, float dir)
    {
        speed = spd;
        direction = dir;

    }
    public void SetCrashed()
    {
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().useGravity = true;
        state = CoinState.Fallen;

    }
    public CoinState GetState()
    {
        return state;
    }
    public bool IsFinished()
    {
        return finished;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (state == CoinState.Run)
        {
            if (collision.gameObject.CompareTag("Coin"))
            {

            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                direction *= -1;
            }
            else if (collision.gameObject.CompareTag("SaisenHead"))
            {                
                state = CoinState.Got;
                GetComponent<Collider>().isTrigger = true;
                rigidBody.useGravity = true;
                rigidBody.MovePosition(new Vector3(-20, -20, 0));
                //rigidBody.AddForce(new Vector3(0f, 5f, -10.0f), ForceMode.Impulse);
            }
            else
            {
                SetCrashed();
                Vector3 vector = (collision.gameObject.transform.position - transform.position);
                rigidBody.AddForce(-10f*vector.normalized, ForceMode.Impulse);
            }
        }
    }
    protected void Init()
    {

    }
    protected void Run()
    {
        SetText();
        if (state==CoinState.Run)
        {
            Move();
        }
        else
        {
            if (transform.position.y < -5)
            {
                transform.position = new Vector3(0, -20, -20);
                finished = true;
                if (state == CoinState.Idle){
                    Destroy(gameObject);
                }
            }
        }
        transform.Rotate(new Vector3(0, 0, 10));
    }
    private void SetText()
    {
        valueText.text = value.ToString();
    }
    virtual protected void Move()
    {
        float theta = Mathf.Deg2Rad * direction;
        float dy = -1 * speed * Mathf.Cos(theta);
        float dx = -1 * speed * Mathf.Sin(theta);
        Vector3 moveVector = new Vector3(dx, dy, 0);
        Vector3 nextPosition = rigidBody.position + moveVector;
        rigidBody.velocity= moveVector;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0, Random.Range(-45, 45),0));
        rigidBody = GetComponent<Rigidbody>();
        valueText = transform.GetChild(0).GetComponent<TextMesh>();
        Init();
    }
    // Update is called once per frame
    void Update()
    {
        Run();
    }
}
