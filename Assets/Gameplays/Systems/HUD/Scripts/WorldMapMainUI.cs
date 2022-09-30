using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapMainUI : MonoBehaviour
{
    [Header("データ")]
    public StageInfos stageInfos;
    public GameData data;
    [Header("テキスト")]
    public Text worldAB;
    public Text percent;
    public Text totalCoins;
    public Text greenStars;
    public Text maxGreenStars;
    public Text sRanks;
    public Text maxSRanks;

    public static int step = 0;
    [Header("ステージ情報")]
    public GameObject stageDetailUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int world = 0;

        //ワールドA-B
        worldAB.text = "WORLD " + (world+1).ToString();
        //達成率
        percent.text = stageInfos.getPercent(world).ToString() + "% コンプリート";
        //コイン
        totalCoins.text = (data.coins).ToString("###,###,##0");
        //グリーンスター
        greenStars.text = stageInfos.getCurrentGreenStars(world).ToString();
        maxGreenStars.text = "/" + stageInfos.getAllGreenStars(world).ToString();
        //Ｓランク
        sRanks.text = stageInfos.getCurrentBestRankCount(world).ToString();
        maxSRanks.text = "/" + stageInfos.getAllBestRankCount(world).ToString();

        stageDetailUI.SetActive(step > 0);
    }
}
