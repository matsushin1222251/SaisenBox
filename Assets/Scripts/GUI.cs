using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GUI : MonoBehaviour
{
    public GameObject titleTextObj = null;
    public GameObject scoreTextObj=null;
    public GameObject scoreTextPanel = null;
    public GameObject messageTextObj = null;
    public GameObject stageObj = null;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Stage stage = stageObj.GetComponent<Stage>();
        if (stage.state == Stage.StageState.START)
        {
            titleTextObj.SetActive(true);
            scoreTextPanel.SetActive(false);
        }
        else
        {
            titleTextObj.SetActive(false);
            scoreTextPanel.SetActive(true);
        }
        string scoreText=
            "お賽銭：\n" + stage.GetScore().ToString() + "円\n"
            + "過去最高額：\n" + stage.GetHighScore().ToString() + "円\n"
            + "参拝者数：\n" + stage.GetWave().ToString() + "/" + stage.endWave +"人\n"
            + "収集率:\n" + ((int)(stage.GetRate() * 100)).ToString() + "%\n"
            + "ご利益ポイント:\n" + ((int)(stage.GetBonusCount() * 100)).ToString() + "%\n"
            + "ミス:\n" + stage.GetMissCount().ToString() +"/" + stage.missCountMax.ToString() + "\n";
        scoreTextObj.GetComponent<Text>().text = scoreText;
        messageTextObj.GetComponent<Text>().text = stage.message;

    }
}
