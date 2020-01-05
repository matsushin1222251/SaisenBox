using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CoinType
{
    public GameObject prefab;
    public int value;
    public int pase;
};

public class Stage : MonoBehaviour
{
    public enum StageState {START, READY, RUN, GAMEOVER };
    private int wave;
    private int score;
    private int last_score;
    private int wave_score;
    private int highScore;
    private float gotRate;
    public List<GameObject> coins;

    public List<CoinType> coinTypes;
    private List<int> totalCoin;

    public GameObject saisenBox;
    public StageState state;
    public float bonusCount;
    public uint missCount;
    public int dumpCount;

    [SerializeField]
    public int startWave = 1;
    [SerializeField]
    public int endWave = 2;
    [SerializeField]
    float minSpeed = 4;
    [SerializeField]
    float maxSpeed = 32;
    [SerializeField]
    float bonusCost = 3;
    [SerializeField]
    public float missCountMax = 3;


    float GetCurrentSpeed()
    {
        int start = startWave, end = endWave;
        float progress = (1.0f * wave - start) / (end - start);
        if (progress > 1.0f)
        {
            progress = 1.0f;
        }
        return minSpeed + (maxSpeed - minSpeed) * progress;
    }

    void CreateRandomCoins()
    {
        for (int j = 0; j < coins.Count; j++)
        {
            Destroy(coins[j]);
        }
        coins.Clear();
        float dir = 0;
        if (wave > 1)
        {
            int r = Random.Range(0, 2);
            if (r == 0)
            {
                dir = 45.0f * Random.Range(-1, 1);
            }
        }
        float x = Random.Range(-5, 5);
        float y = 40f;
        for (int i = 0; i < 50; i++)
        {
            x += Random.Range(-3, 3);
            if (x < -5)
            {
                x = -2;
            }else if (x > 5)
            {
                x = 2;
            }
            y += 2f;
            for(int j = coinTypes.Count - 1; j >= 0; j--)
            {
                if(i% coinTypes[j].pase == coinTypes[j].pase-1)
                {
                    GameObject obj = Instantiate(coinTypes[j].prefab);
                    obj.GetComponent<Rigidbody>().position=new Vector3(x, y, 0);
                    obj.GetComponent<BasicCoin>().saisenBox = saisenBox;
                    float cSpeed = GetCurrentSpeed();
                    obj.GetComponent<BasicCoin>().SetParam(cSpeed, dir);
                    coins.Add(obj);
                    break;
                }
            }
            
        }
    }
    void CreateArowCoins(bool bonus=false)
    {
        for (int j = 0; j < coins.Count; j++)
        {
            Destroy(coins[j]);
        }
        coins.Clear();
        float dir = 0;
        float y = 40f;
        int n = 7;
        float minCol = -5, maxCol = 5;
        int interval = (int)((maxCol - minCol) / n);
        float[] col = new float[n];
        for (int i = 0; i < n; i++)
        {
            col[i]=minCol + (i+0.5f) * interval;
        }
        col = col.OrderBy(i => System.Guid.NewGuid()).ToArray();
        for (int i = 0; i < n; i++)
        {
            float x = col[i];
            int length = 10;
            for (int j=0;j< length; j++)
            {
                int ctype = 0;
                if (bonus)
                {
                    ctype = coinTypes.FindIndex(e => e.value == 100);
                }
                else if (j>4)
                {
                    ctype=coinTypes.FindIndex(e => e.value == 10);
                }
                y += 1f;
                GameObject obj = Instantiate(coinTypes[ctype].prefab);
                obj.GetComponent<Rigidbody>().position = new Vector3(x, y, 0);
                obj.GetComponent<BasicCoin>().saisenBox = saisenBox;
                
                float cSpeed = GetCurrentSpeed();
                obj.GetComponent<BasicCoin>().SetParam(cSpeed, dir);                
                coins.Add(obj);
            }
            y += length;
        }
    }
    void CreateConcentratedCoins(bool bonus=false)
    {
        for (int j = 0; j < coins.Count; j++)
        {
            Destroy(coins[j]);
        }
        coins.Clear();
        float dir = 0;
        float x = 0;
        float y = 40f;
        float theta = Random.Range(-45.0f, 45.0f);
        int length = 70;
        for (int i = 0; i < length; i++)
        {
            float progress = (i + 1.0f) / length;
            x = (1+4.0f* progress) * Mathf.Sin((theta+i * 30.0f) * Mathf.Deg2Rad);
            y = 40f + 4.0f * i;
            float cSpeed = GetCurrentSpeed();
            int ctype = 0;
            if (bonus)
            {
                ctype = coinTypes.FindIndex(e => e.value == 100);
            }
            else if (i%10 > 4)
            {
                ctype = coinTypes.FindIndex(e => e.value == 10);
            }

            GameObject obj = Instantiate(coinTypes[ctype].prefab);
            obj.GetComponent<Rigidbody>().position = new Vector3(x, y, 0);
            obj.GetComponent<BasicCoin>().saisenBox = saisenBox;
            obj.GetComponent<BasicCoin>().SetParam(cSpeed, dir);
            coins.Add(obj);
        }
    }
    bool CheckCoins()
    {
        bool result = true;
        int cwave_score = 0;
        int got_count = 0;
        List<int> waveCoin=new List<int>();
        for (int j =0; j<coins.Count; j++)
        {
            BasicCoin coin = coins[j].GetComponent<BasicCoin>();
            BasicCoin.CoinState state = coin.GetState();
            if (state == BasicCoin.CoinState.Run)
            {
                result = false;
            }
            else {
                if (!coin.IsFinished())
                {
                    result = false;
                }
                else if (state == BasicCoin.CoinState.Got)
                {
                    int coinTypeIdx = coinTypes.FindIndex(n => n.value == coin.value);
                    waveCoin.Add(coinTypeIdx);
                    cwave_score += coin.value;
                    got_count++;
                }
            }
        }
        if (wave_score < cwave_score)
        {
            wave_score = cwave_score;
        }
        score = last_score + wave_score;
        if (coins.Count > 0)
        {
            gotRate = 1.0f * got_count / coins.Count;
        }
        else
        {            
            gotRate = 1.0f ;
        }
        if (result)
        {
            totalCoin.AddRange(waveCoin);
        }
        return result;
    }
    public int GetWave()
    {
        return wave;
    }
    public int GetScore()
    {
        return score;
    }
    public float GetRate()
    {
        return gotRate;
    }
    public uint GetMissCount()
    {
        return missCount;
    }
    public float GetBonusCount()
    {
        return bonusCount/bonusCost;
    }
    public int GetHighScore()
    {
        return highScore;
    }
    void CreateTempCoins(bool bonus=false)
    {
        int r = Random.Range(0, 2);
        switch (r)
        {
            case 1:
                CreateArowCoins(bonus);
                break;
            default:
                CreateConcentratedCoins(bonus);
                break;
        }        
    }
    void CreateCoins(bool bonus)
    {
        int cwave = wave;
        if (bonus)
        {
            CreateTempCoins(true);
        }
        else if (cwave % 3 == 0)
        {
            CreateTempCoins();
        }
        else
        {
            CreateRandomCoins();
        }
    }
    private bool CheckResult()
    {
        bool result = true;
        if(gotRate < 0.4)
        {
            missCount++;
            result = false;
        }
        bonusCount+=gotRate;
        if(bonusCount > bonusCost)
        {
            bonusCount = bonusCost;
        }
        return result;
    }
    private void DumpCoin()
    {

    }
    // Use this for initialization
    void Start()
    {
        wave = 0;
        score = 0;
        bonusCount = 0;
        missCount = 0;
        highScore = 0;
        coins = new List<GameObject>();
        totalCoin = new List<int>();
        state = StageState.START;
    }
    private int waitTime;
    public string message = "";
    // Update is called once per frame
    void Update()
    {
        message = "";
        if (state == StageState.START)
        {
            wave = 0;
            score = 0;
            bonusCount = 0;
            missCount = 0;
            last_score = 0;
            wave_score = 0;
            gotRate = 0;
            totalCoin.Clear();
            dumpCount = 0;
            message = "クリックで開始。";
            if (Input.GetMouseButtonDown(0))
            {
                state = StageState.READY;
                waitTime = 0;
            }
        }
        else if (state == StageState.READY)
        {
            int customerTime = 240;
            int callBackTime = 120;
            bool bonus = bonusCount >= bonusCost;
            if(waitTime < callBackTime && wave >= startWave)
            {
                if (gotRate < 0.4)
                {
                    message = "ご利益ポイントをあまりもらえませんでした・・・。";
                }
                else if(gotRate < 0.8)
                {
                    message = "ご利益ポイントをもらいました。";
                }
                else
                {
                    message = "ご利益ポイントを大量にもらいました！";
                }
            }
            else if (waitTime < customerTime)
            {
                if (bonus)
                {
                    message = "気前のいい参拝客が来ました!";
                }
                else
                {
                    message = "参拝客が来ました。";
                }
            }
            else if (waitTime > customerTime)
            {
                wave++;   
                CreateCoins(bonus);
                if(bonus)bonusCount = 0;
                state = StageState.RUN;
                waitTime = 0;
            }
            waitTime++;
        }
        else if (state == StageState.RUN)
        {
            if (CheckCoins())
            {
                last_score = score;
                wave_score = 0;
                CheckResult();
                if (missCount >= missCountMax || wave>=endWave)
                {
                    state = StageState.GAMEOVER;
                    waitTime = 0;
                }
                else
                {
                    state = StageState.READY;
                    waitTime = 0;
                }
            }
            else
            {
                waitTime = 0;

            }
        }
        else if (state == StageState.GAMEOVER)
        {
            int coinCount = totalCoin.Count;
            if (waitTime < 180)
            {
                if (gotRate < 0.4)
                {
                    message = "参拝客が皆帰ってしまいました・・・。";
                }
                else
                {
                    message = "参拝客が参拝を終えました！";
                }
                message += "\n集めたお賽銭を納めてください。";
            }
            else if (dumpCount < coinCount)
            {
                if (waitTime % 2 == 0)
                {
                    GameObject obj = Instantiate(coinTypes[totalCoin[dumpCount]].prefab);
                    obj.GetComponent<Collider>().isTrigger = true;
                    //obj.GetComponent<Rigidbody>().useGravity = true;
                    obj.GetComponent<Rigidbody>().position = new Vector3(0, 60, 0);
                    obj.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    obj.GetComponent<BasicCoin>().saisenBox = saisenBox;
                    obj.GetComponent<BasicCoin>().state = BasicCoin.CoinState.Idle;
                    obj.GetComponent<Rigidbody>().useGravity = true;
                    Vector3 vector = new Vector3(0, 1, -1);
                    obj.GetComponent<Rigidbody>().position = saisenBox.GetComponent<Rigidbody>().position;
                    obj.GetComponent<Rigidbody>().AddForce(
                        new Vector3(Random.Range(-5,5), 10f+ Random.Range(-2, 2), -10.0f), ForceMode.Impulse);
                    if (dumpCount % 8 == 0)
                    {
                        obj.GetComponent<AudioSource>().PlayOneShot(obj.GetComponent<BasicCoin>().sound1);
                    }
                    dumpCount++;
                    
                }
            }
            else {
                message = "今日のお賽銭は" + score.ToString() + "円です。";
                if (score > highScore && highScore>0)
                {
                    message += "\n過去最高額でした！";
                    message += "\n明日も最高の日でありますように!";
                }
                else if (wave>missCountMax)
                {
                    message += "\n明日もいい日でありますように。";
                }
                else
                {
                    message += "\n・・・来世はいい日がありますように。";
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (score > highScore)
                    {
                        highScore = score;
                    }
                    state = StageState.START;
                    waitTime = 0;
                }
            }

            waitTime++;
        }
    }
}
