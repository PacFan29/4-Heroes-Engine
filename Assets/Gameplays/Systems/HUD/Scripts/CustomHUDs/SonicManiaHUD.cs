using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicManiaHUD : HUDCommon
{
    [Header("数値")]
    public SpriteNumber scoreNum;
    public SpriteNumber[] timeNums = new SpriteNumber[3];
    public SpriteNumber coinsNum;

    public override void ScoreDisp() {
        scoreNum.value = GameManager.Score;
    }
    public override void TimeDisp() {
        for (int i = 0; i < timeValues.Length; i++) {
            timeNums[i].value = timeValues[i];
        }
    }
    public override void CoinsDisp() {
        coinsNum.value = GameManager.Coins;
    }
}
