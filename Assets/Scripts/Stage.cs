using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private float gotRate;
    public List<GameObject> coins;
    //public List<GameObject> coinPrefabs;
    //public List<int> coinPases;

    public List<CoinType> coinTypes;
    private List<int> totalCoin;

    public GameObject saisenBox;
    public StageState state;
    public uint bonusCount;
    public uint missCount;
    public int dumpCount;
    void CreateRandomCoins()
    {
        for (int j = 0; j < coins.Count; j++)
        {
            Destroy(coins[j]);
        }
        coins.Clear();
        float x = Random.Range(-5, 5);
        float y = 20f;
        for (int i = 0; i < 50; i++)
        {
            x += Random.Range(-5, 5);
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
                    int start = 0, end = 50;
                    float progress = (1.0f*wave - start )/ (end - start);
                    if (progress > 1.0f)
                    {
                        progress = 1.0f;
                    }
                    obj.GetComponent<BasicCoin>().SetParam(4 + 28.0f * progress, 0f);
                    coins.Add(obj);
                    break;
                }
            }
            
        }
    }
    void CreateArowCoins()
    {
        for (int j = 0; j < coins.Count; j++)
        {
            Destroy(coins[j]);
        }
        coins.Clear();
        float y = 40f;
        for (int i = 0; i < 5; i++)
        {
            float x = Random.Range(-5, 5);
            for (int j=0;j<20;j++)
            {
                int ctype = 0;
                if (j == 0)
                {
                    ctype=coinTypes.FindIndex(e => e.value == 100);
                }
                else if(j>15)
                {
                    ctype=coinTypes.FindIndex(e => e.value == 10);
                }
                y += 1f;
                GameObject obj = Instantiate(coinTypes[ctype].prefab);
                obj.GetComponent<Rigidbody>().position = new Vector3(x, y, 0);
                obj.GetComponent<BasicCoin>().saisenBox = saisenBox;
                int start = 0, end = 50;
                float progress = (1.0f * wave - start) / (end - start);
                if (progress > 1.0f)
                {
                    progress = 1.0f;
                }
                obj.GetComponent<BasicCoin>().SetParam(8 + 56.0f * progress, 0f);
                coins.Add(obj);
            }
            y += 10f-i;
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
    public uint GetBonusCount()
    {
        return bonusCount;
    }

    void CreateCoins()
    {
        if (bonusCount > 5)
        {
            CreateRandomCoins();
            bonusCount = 0;
        }
        else if (wave % 10 == 0)
        {
            CreateArowCoins();
        }
        else
        {
            CreateRandomCoins();
        }
    }
    private void CheckResult()
    {
        if(gotRate > 0.8)
        {
            bonusCount++;
        }
        else if(gotRate < 0.4)
        {
            missCount++;
        }
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
        coins = new List<GameObject>();
        totalCoin = new List<int>();
        state = StageState.START;
    }
    private int waitTime;
    // Update is called once per frame
    void Update()
    {
        if (state == StageState.START)
        {
            wave = 0;
            score = 0;
            bonusCount = 0;
            missCount = 0;
            last_score = 0;
            wave_score = 0;
            if (Input.GetMouseButtonDown(0))
            {
                state = StageState.READY;
                waitTime = 0;
            }
        }
        else if (state == StageState.READY)
        {
            if (waitTime > 60)
            {
                CreateCoins();
                state = StageState.RUN;
                waitTime = 0;
            }
            else
            {
                waitTime++;
            }
        }
        else if (state == StageState.RUN)
        {
            if (CheckCoins())
            {
                last_score = score;
                wave_score = 0;
                CheckResult();
                if (missCount >= 1)
                {
                    state = StageState.GAMEOVER;
                    waitTime = 0;
                }
                else
                {
                    state = StageState.READY;
                    waitTime = 0;
                    wave++;
                }
            }
        }
        else if (state == StageState.GAMEOVER)
        {
            int coinCount = totalCoin.Count;
            if (waitTime == 0)
            {
                for (int j = 0; j < coins.Count; j++)
                {
                    Destroy(coins[j]);
                }
                coins.Clear();
                for (int i = 0; i < coinCount; i++)
                {
                    GameObject obj = Instantiate(coinTypes[totalCoin[i]].prefab);
                    obj.GetComponent<Collider>().isTrigger = true;
                    //obj.GetComponent<Rigidbody>().useGravity = true;
                    obj.GetComponent<Rigidbody>().position = new Vector3(0,60,0);
                    obj.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    obj.GetComponent<BasicCoin>().saisenBox = saisenBox;
                    obj.GetComponent<BasicCoin>().state = BasicCoin.CoinState.Idle;
                    coins.Add(obj);
                }
            }
            if (dumpCount < coinCount)
            {
                if (waitTime % 4 == 0)
                {
                    coins[dumpCount].GetComponent<Rigidbody>().useGravity = true;
                    Vector3 vector = new Vector3(0, 1, -1);
                    coins[dumpCount].GetComponent<Rigidbody>().position = saisenBox.GetComponent<Rigidbody>().position;
                    coins[dumpCount].GetComponent<Rigidbody>().AddForce(
                        new Vector3(Random.Range(-5,5), 10f+ Random.Range(-2, 2), -10.0f), ForceMode.Impulse);
                    dumpCount++;
                }
            }
            else {
                    if (Input.GetMouseButtonDown(0))
                {
                    state = StageState.START;
                    waitTime = 0;
                }
            }

            waitTime++;
        }
    }
}
