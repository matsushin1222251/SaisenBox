using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GUI : MonoBehaviour
{
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Stage stage;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "お賽銭総額：" + stage.GetScore().ToString() + "円"
                        + "\n参拝者数：" + stage.GetWave().ToString() + "人"
                        + "\n幸福ポイント:" + ((int)(stage.GetRate() * 100)).ToString() + "%"
                        + "\nボーナス:" + stage.GetBonusCount().ToString() + "%"
                        + "\n失敗:" + stage.GetMissCount().ToString() + "%";
    }
}
