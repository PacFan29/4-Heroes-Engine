using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MegaMan11HUD : HUDCommon
{
    public MegaManHealthBar bossHealth;
    [Header("数値")]
    public Text scoreText;
    public Text timeText;
    public Text coinsText;

    public override void ScoreDisp() {
        scoreText.text = GameManager.Score.ToString();
    }
    public override void TimeDisp() {
        timeText.text = timeValues[0].ToString("00") + ":" + timeValues[1].ToString("00") + ":" + timeValues[2].ToString("00");
    }
    public override void CoinsDisp() {
        coinsText.text = GameManager.Coins.ToString();
    }

    public override void BossHealth() {
        bossHealth.maxValue = GameManager.bossInfo.maxHp;
        bossHealth.currentValue = GameManager.bossInfo.totalHp;
    }
}
