using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PacManWorldHUD : HUDCommon
{
    [Header("数値")]
    public Text scoreText;
    public Text timeText;
    public Text cookiesText;

    public override void ScoreDisp() {
        scoreText.text = GameManager.Score.ToString("###,###,##0");
    }
    public override void TimeDisp() {
        int totalTime = timeValues[0] * 60;
        totalTime += timeValues[1];
        totalTime = Mathf.Clamp(totalTime, 0, 999);

        timeText.text = totalTime.ToString("000");
    }
    public override void CoinsDisp() {
        cookiesText.text = GameManager.Coins.ToString("###,###,##0");
    }
}
