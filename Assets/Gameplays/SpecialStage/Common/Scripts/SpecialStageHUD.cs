using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SSPlayerType {
    Lives,
    Health
}
public enum SSParameter {
    None,
    Score,
    Time,
    Coins
}
public class SpecialStageHUD : MonoBehaviour
{
    [Header("プレイヤー情報")]
    public SSPlayerType playerType;
    public GameObject livesDisp;
    public GameObject healthDisp;
    [Header("主要パラメーター")]
    public SSParameter primaryParameter;
    public Text primaryHeader;
    public Text primaryValue;
    [Header("副パラメーター")]
    public SSParameter subParameter;
    public GameObject subGroup;
    public Text subHeader;
    public Text subValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤー情報
        livesDisp.SetActive(playerType == SSPlayerType.Lives);
        healthDisp.SetActive(playerType == SSPlayerType.Health);

        //タイム
        int minutes = (int)Math.Floor(SpecialStageManager.TotalTime / 60000f);
        int seconds = (int)Math.Floor(SpecialStageManager.TotalTime / 1000f) % 60;
        int milli = (int)Math.Floor(SpecialStageManager.TotalTime / 10f) % 100;
        string timeValue = minutes.ToString("00") + "\'" + 
                           seconds.ToString("00") + "\"" + 
                           milli.ToString("00");

        //主要パラメーター
        switch (primaryParameter) {
            case SSParameter.None:
            case SSParameter.Score:
            primaryHeader.text = "SCORE";
            primaryValue.text = (SpecialStageManager.Score).ToString("###,###,##0");
            break;
            
            case SSParameter.Time:
            primaryHeader.text = "TIME";
            primaryValue.text = timeValue;
            break;

            case SSParameter.Coins:
            primaryHeader.text = "COINS";
            primaryValue.text = (SpecialStageManager.Coins).ToString("###,###,##0");
            break;
        }

        //副パラメーター
        subGroup.SetActive(subParameter != SSParameter.None);
        switch (subParameter) {
            case SSParameter.None:
            case SSParameter.Score:
            subHeader.text = "SCORE";
            subValue.text = (SpecialStageManager.Score).ToString("###,###,##0");
            break;
            
            case SSParameter.Time:
            subHeader.text = "TIME";
            subValue.text = timeValue;
            break;

            case SSParameter.Coins:
            subHeader.text = "COINS";
            subValue.text = (SpecialStageManager.Coins).ToString("###,###,##0");
            break;
        }
    }
}
