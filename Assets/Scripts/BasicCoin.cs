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

    [SerializeField]
    public GameObject scoreText;

    [SerializeField]
    public AudioClip sound1;
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
    virtual protected void Reflect()
    {
        direction *= -1;

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
                Reflect();
            }
            else if (collision.gameObject.CompareTag("SaisenHead"))
            {
                GameObject scoreTextObj = Instantiate(scoreText);
                scoreTextObj.transform.position = saisenBox.GetComponent<Rigidbody>().position;
                float fscale = 0.5f * Mathf.Log10(value) + 1;
                scoreTextObj.GetComponent<ScoreText>().SetScore("+" + value.ToString(), fscale);

                state = CoinState.Got;
                GetComponent<Collider>().isTrigger = true;
                rigidBody.useGravity = true;
                rigidBody.MovePosition(new Vector3(-20, -20, 0));
                GetComponent<AudioSource>().PlayOneShot(sound1);
            }
            else
            {
                SetCrashed();
                Vector3 vector = (collision.gameObject.transform.position - transform.position);
                rigidBody.AddForce(-10f*vector.normalized, ForceMode.Impulse);

                GameObject scoreTextObj = Instantiate(scoreText);
                scoreTextObj.transform.position = GetComponent<Rigidbody>().position;
                float fscale = 0.5f;
                scoreText.GetComponent<ScoreText>().SetScore("miss!", fscale);
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
