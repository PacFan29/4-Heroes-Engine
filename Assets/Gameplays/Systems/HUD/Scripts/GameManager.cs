using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DimensionType {
	Normal3D,
	XWay2D,
	ZWay2D,
    FreeWay3D
}
public class PlayerStatus
{
    public int ID;
    public int HP;
    public int MaxHP;
    public int Lives;
    public int Score;

    public PlayerStatus(){
        ID = 0;
        MaxHP = 40;
        HP = MaxHP;
        Lives = 2;
        Score = 0;
    }
    public void setStatus(int setID, int setMaxHP, int setHP, int setLives, int setScore){
        ID = setID;
        MaxHP = setMaxHP;
        HP = setHP;
        Lives = setLives;
        Score = setScore;
    }
    public int[] getStatus(){
        int[] allStatus = {ID, MaxHP, HP, Lives, Score};
        return allStatus;
    }
}

public class GameManager : MonoBehaviour
{
    public static int Score = 0;
    public static int Coins = 0;
    public static float TotalTime = 0;
    public static int Gold = 0; //標準目標：100,000ドル
    public static bool[] keyItems = new bool[7];

    public static int targetScore = 50000; //標準目標：50,000点
    public static int targetTime = (600) * 1000; //標準目標：10分

    public static bool timeAction = true;
    public static bool gameOver = false;
    private bool coins1up = false;
    public static bool death = false;
    public static bool falling = false;
    public static DimensionType dimension = DimensionType.Normal3D; //０＝3D １＝2D(X方向) ２＝2D(Z方向)

    public static int maxCombo = 32;
    public static int checkPointCount = 0;
    public static float totalSpeed = 0f; //速度60 → 1200km/h（20倍）
    public static float averageSpeed = 0f; //速度60 → 1200km/h（20倍）
    public static List<PlayerStatus> players = new List<PlayerStatus>();

    public static int perfect = 10;

    public static BossManager bossInfo;

    public static bool extra = false;
    public static bool ready = false;

    public static bool HUDshow = true;
	public static bool bossHPShow = false;
	public static bool ScoreShow = true;
	public static bool timeIncrease = true;

    public static int[] fruits = new int[8];
    public static int hudAnimNo = 0;

    void Start()
    {
        players = new List<PlayerStatus>();

        bossHPShow = false;
        timeIncrease = true;
        fruits = new int[8];
    }

    void Update()
    {
        //スコア（0点～9億9999万9999点まで）
        Score = 0;
        for (int i = 0; i < players.Count; i++){
            //プレイヤー全員分のスコアを総計
            Score += players[i].getStatus()[4];
        }
        Score = Mathf.Clamp(Score, 0, 999999999);
        //タイム（0分0秒00から60分0秒00まで）
        TotalTime = Mathf.Clamp(TotalTime, 0, 3600000);
        //コイン（0枚～999枚まで）
        Coins = Mathf.Clamp(Coins, 0, 999999);
        //ゴールド（0ドル～9億9999万9999ドルまで）
        Gold = Mathf.Clamp(Gold, 0, 999999999);

        if (Coins >= 100 && !coins1up){
            //1UP（1回のみ）
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < allPlayers.Length; i++){
                //プレイヤー全員分1UP
                if (allPlayers[i].GetComponent<PlayerInfo>() != null){
                    allPlayers[i].GetComponent<PlayerInfo>().OneUp();
                }
            }
            coins1up = true;
        }

        ready = (FadeManager.alpha <= 0) && (WipeManager.scale >= 1);

        if (checkPointCount > 0) {
            averageSpeed = totalSpeed / (float)checkPointCount;
        } else {
            averageSpeed = 0f;
        }

        //HUDアニメーション専用
        if (gameOver){
			//ゲームオーバー
			hudAnimNo = 2;
		} else {
			if (HUDshow && !bossHPShow && hudAnimNo != 1 && !(hudAnimNo >= 3 && hudAnimNo <= 5)){
				//表示
				hudAnimNo = 1;
			} else if (!HUDshow && !bossHPShow && hudAnimNo == 1){
				//非表示
				hudAnimNo = 2;
			} else if (HUDshow && bossHPShow && (hudAnimNo == 1 || hudAnimNo == 4)){
				//ボスゲージ表示（全表示状態）
				hudAnimNo = 3;
			} else if (HUDshow && !bossHPShow && (hudAnimNo == 3 || hudAnimNo == 5)){
				//ボスゲージ非表示（全表示状態）
				hudAnimNo = 4;
			} else if (HUDshow && bossHPShow && hudAnimNo != 3 && hudAnimNo != 5){
				//ボスゲージ表示
				hudAnimNo = 5;
			} else if (!HUDshow && !bossHPShow && hudAnimNo == 5){
				//ボスゲージ非表示
				hudAnimNo = 6;
			}
		}
    }

    public void Reset(int sc_tg, int tm_tg, bool sonicBeginner){
        death = false;
        falling = false;
        Score = 0;
        Coins = sonicBeginner ? 10 : 0;
        Gold = 0;
        targetScore = sc_tg;
        targetTime = tm_tg * 1000;
        maxCombo = 0;
        timeAction = true;

        checkPointCount = 0;
        totalSpeed = 0f;

        keyItems = new bool[7];
    }

    public static bool is3D() {
        return dimension == DimensionType.Normal3D;
    }
}
