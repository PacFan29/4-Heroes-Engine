using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mario3DWorldHUD : HUDCommon
{
    [Header("数値")]
    public Text scoreText;
    public Text timeText;
    public Text coinsText;

    public override void ScoreDisp() {
        scoreText.text = GameManager.Score.ToString("000000000");
    }
    public override void TimeDisp() {
        timeText.text = timeValues[0].ToString("00") + ":" + timeValues[1].ToString("00");
    }
    public override void CoinsDisp() {
        coinsText.text = GameManager.Coins.ToString("×00");
    }
}
