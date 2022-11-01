using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Results : MonoBehaviour
{
    [Header("データ")]
    public GameData data;
    public StageInfos stageInfos;
    public Options options;
    [Header("ワールドA-B")]
    public Text worldDisp;
    [Header("グループ")]
    public GameObject scoreAttackOnly;
    public GameObject bossStageOnly;

    [Header("ヘッダー")]
    public Text CoinsHeader;
    public Text MaxComboHeader;

    [Header("[スコアアタック]")]
    public Text ScoreText;
    [Header("タイム")]
    public Text TimeText;
    public Text TimeBonus;
    [Header("コイン・クッキー・ネジ・リング")]
    public Text CoinsText;
    public Text CoinBonus;
    [Header("最大コンボ")]
    public Text MaxComboText;
    public Text MaxComboBonus;
    [Header("合計スコア")]
    public Text TotalScore;
    public Image scoreBar;
    [Header("[ボスステージ]")]
    public Text PerfectHeader;
    public Text PerfectBonus;

    [Header("ランク")]
    public Sprite[] RankSprites = new Sprite[7];
    public GameObject RankObj;
    public Text NextRank;
    private int[] bonuses = new int[3];
    private int[] uppers = new int[3];
    private int[] currents = new int[3];

    private int animNo;
    private float time;
    private GameObject[] PopUps = new GameObject[6];
    private float[] totalDistances = new float[6];

    public bool bossStage;
    private bool sonicStage;

    [Header("ＢＧＭ・効果音")]
    public AudioClip[] musics = new AudioClip[3];
    public AudioClip[] sounds = new AudioClip[6];
    private AudioSource musicPlayer;

    private Animator rankAnim;
    private int rankIndex = 0;
    // Start is called before the first frame update
    void Awake()
    {
        sonicStage = (data.HUDType == GameType.Sonic);
        bossStage = (data.HUDType == GameType.Boss) || (data.HUDType == GameType.BossRush);

        musicPlayer = GetComponent<AudioSource>();
        rankAnim = GetComponent<Animator>();
        for (int i = 0; i < bonuses.Length; i++){
            bonuses[i] = 0;
        }
        uppers[0] = (int)GameManager.TotalTime;
        if (bossStage) {
            if (GameManager.noMiss) {
                if (GameManager.damageTaken > 0) {
                    uppers[1] = 3000;
                } else {
                    uppers[1] = 10000;
                }
            } else {
                uppers[1] = 0;
            }
        } else {
            if (sonicStage){
                uppers[1] = GameManager.Coins;
                uppers[2] = (int)Math.Ceiling(GameManager.averageSpeed * 10);
            } else {
                uppers[1] = GameManager.damageTaken;
                uppers[2] = GameManager.maxCombo;
            }
        }

        currents[0] = 5999999;
        currents[1] = (sonicStage || bossStage) ? 0 : 99;
        currents[2] = 0;

        animNo = 0;

        //ボスステージ判定
        scoreAttackOnly.SetActive(!bossStage);
        bossStageOnly.SetActive(bossStage);

        if (sonicStage){
            musicPlayer.clip = musics[1];
        } else {
            musicPlayer.clip = musics[0];
        }
        //musicPlayer.clip = musics[2];
        musicPlayer.Play();
        musicPlayer.volume = options.musicVolume * 0.7f;

        data.LevelComplete();
    }

    // Update is called once per frame
    void Update()
    {
        int world = Int32.Parse(data.stageInfo["worldNo"]);
        int stage = Int32.Parse(data.stageInfo["stageNo"]);

        //ワールドA-B
        worldDisp.text = "WORLD " + (world + 1).ToString() + "-" + (stage + 1).ToString();

        string timeFormat = getTimeFormat(currents[0]);
        int left = 0;
        
        //タイムボーナス(1 - (現在のタイム ÷ 目標タイム) × 30,000点)
        int maxTimeBonus = sonicStage ? 60000 : 30000;
        int timeRate = bossStage ? 5 : 1;
        if (bossStage) maxTimeBonus = 50000;
        bonuses[0] = Math.Max(0, (int)Math.Ceiling((1f - ((float)currents[0] / (GameManager.targetTime * timeRate))) * maxTimeBonus));
        /* 出力 */
        TimeText.text = timeFormat;
        TimeBonus.text = bonuses[0].ToString("###,###,##0");

        if (!bossStage) {
            if (sonicStage){
                //リングボーナス(獲得リング枚数 × 100点)
                bonuses[1] = Math.Min(currents[1] * 100, 99999);

                //平均スピードボーナス(平均スピード × 6.25点)
                bonuses[2] = (int)Math.Ceiling((float)currents[2] * 0.625f);
            } else {
                //被ダメージボーナス(7000点 - ダメージ数 × 700点)
                bonuses[1] = Math.Max(7000 - currents[1] * 700, 0);

                //最大コンボボーナス（(最大コンボ ^ 2) * 100点）
                bonuses[2] = (int)Math.Min(Math.Pow(currents[2], 2) * 100, 10000);
            }

            /* 出力 */
            //スコア
            ScoreText.text = GameManager.Score.ToString("###,###,##0");
            //コインボーナス
            if (sonicStage){
                CoinsHeader.text = "リング";
            } else {
                CoinsHeader.text = "被ダメージ";
            }
            CoinsText.text = currents[1].ToString("###,###,##0");
            CoinBonus.text = bonuses[1].ToString("###,###,##0");
            if (sonicStage) {
                //平均スピードボーナス
                MaxComboHeader.text = "平均スピード";
                MaxComboText.text = ((float)currents[2] / 10f).ToString("###,###,##0.0 km/h");
            } else {
                //最大コンボボーナス
                MaxComboHeader.text = "最大コンボ";
                MaxComboText.text = currents[2].ToString("###,###,##0");
            }
            MaxComboBonus.text = bonuses[2].ToString("###,###,##0");
        } else {
            //ノーダメージ・ノーミスボーナス
            bonuses[1] = currents[1];

            if (GameManager.noMiss) {
                if (GameManager.damageTaken <= 0) {
                    PerfectHeader.text = "ノーダメージ";
                } else {
                    PerfectHeader.text = "ノーミス";
                }
                PerfectBonus.text = bonuses[1].ToString("###,###,##0");
            } else {
                PerfectHeader.gameObject.SetActive(false);
            }
        }
        //合計スコア
        int total = GameManager.Score;
        foreach(int bonus in bonuses){
            total += bonus;
        }
        TotalScore.text = total.ToString("###,###,##0");
        float scorePercent = ((float)total / GameManager.targetScore) * 100;
        scoreBar.fillAmount = scorePercent / 100f;
        //ランク
        if (scorePercent >= 100){
            //ランクＳ
            rankIndex = 6;
        } else if (scorePercent >= 90){
            //ランクＡ
            rankIndex = 5;
            left = GameManager.targetScore - total;
        } else if (scorePercent >= 70){
            //ランクＢ
            rankIndex = 4;
            left = (int)Math.Ceiling(GameManager.targetScore * 0.9f) - total;
        } else if (scorePercent >= 50){
            //ランクＣ
            rankIndex = 3;
            left = (int)Math.Ceiling(GameManager.targetScore * 0.7f) - total;
        } else if (scorePercent >= 30){
            //ランクＤ
            rankIndex = 2;
            left = (int)Math.Ceiling(GameManager.targetScore * 0.5f) - total;
        } else if (scorePercent >= 10){
            //ランクＥ
            rankIndex = 1;
            left = (int)Math.Ceiling(GameManager.targetScore * 0.3f) - total;
        } else {
            //ランクＦ
            rankIndex = 0;
            left = (int)Math.Ceiling(GameManager.targetScore * 0.1f) - total;
        }
        RankObj.GetComponent<Image>().sprite = RankSprites[rankIndex];
        NextRank.text = left.ToString("###,###,##0");

        /* アニメーション */
        time += Time.deltaTime;
        switch (animNo){
            case 0:
            case 2:
            case 4:
            //0.5秒待つ
            if (animNo == 0 && time >= 2f || animNo != 0 && time >= 0.5f) {
                if (bossStage && animNo == 2){
                    animNo = 7;
                } else {
                    animNo += 1;
                }
            }
            break;

            case 1:
            //タイムボーナス
            currents[0] = valueAnim(currents[0], uppers[0]);
            if (currents[0] == uppers[0]){
                animNo = 2;
                time = 0f;
            }
            break;

            case 3:
            //コインボーナス
            currents[1] = valueAnim(currents[1], uppers[1]);
            if (currents[1] == uppers[1]){
                animNo = 4;
                time = 0f;
            }
            break;

            case 5:
            //最大コンボボーナス
            currents[2] = valueAnim(currents[2], uppers[2]);
            if (currents[2] == uppers[2]){
                animNo = 6;
                time = 0f;
            }
            break;

            case 6:
            if (Input.GetButtonDown("A")) {
                data.coins += GameManager.Coins;
                data.coins += (int)Math.Round(scorePercent) * (bossStage ? 10 : 5);
                if (data.coins > 999999) data.coins = 999999;

                GameManager.noMiss = true;
                GameManager.damageTaken = 0;
                data.HUDType = GameType.Normal;
                SceneManager.LoadScene("WorldMap");
            }
            break;

            case 7:
            //ノーダメージ・ノーミスボーナス
            currents[1] = valueAnim(currents[1], uppers[1]);
            if (currents[1] == uppers[1]){
                animNo = 6;
                time = 0f;
            }
            break;
        }
        if (Input.GetButtonDown("A") && animNo != 6){
            currents[0] = uppers[0];
            currents[1] = uppers[1];
            currents[2] = uppers[2];
        }
        rankAnim.SetBool("rankShow", (animNo == 6));


        /* ハイスコア更新等 */
        StageInfos.Stage info = stageInfos.worlds[world].stages[stage];

        info.cleared = true;

        if (total > info.highScore) {
            //ハイスコア
            info.highScore = total;
        }
        if (rankIndex > info.bestRank) {
            //ランク
            info.bestRank = rankIndex;
        }
        if (uppers[0] < info.bestTime) {
            //ベストタイム
            info.bestTime = uppers[0];
        }

        //グリーンスターの記録
        for (int i = 0; i < info.greenStars.Length; i++) {
            if (GameManager.keyItems[i]) {
                info.greenStars[i] = GameManager.keyItems[i];
            }
        }
    }

    public int valueAnim(int current, int goal){
        int up = (int)Math.Ceiling(((float)(goal - current) * 5) * Time.deltaTime);
        if (Math.Abs(up) <= 0 && goal != current){
            if ((goal - current) > 0){
                up = 1;
            } else {
                up = -1;
            }
        }
        current += up;
        return current;
    }
    public string getTimeFormat(int totalTime){
        double minutes = totalTime / 60000;
        double seconds = (totalTime / 1000) % 60;
        double milli = (totalTime % 1000) / 10;
        return minutes.ToString("00") 
            + "\'" + seconds.ToString("00") 
            + "\"" + milli.ToString("00");
    }
    public void rankSound(){
        musicPlayer.PlayOneShot(sounds[2], options.soundVolume);
        switch (rankIndex){
            case 0:
            //ランクＦ
            musicPlayer.PlayOneShot(sounds[3], options.soundVolume);
            break;
            case 5:
            //ランクＡ
            musicPlayer.PlayOneShot(sounds[4], options.soundVolume);
            break;
            case 6:
            //ランクＳ
            musicPlayer.PlayOneShot(sounds[5], options.soundVolume);
            break;
        }
    }
}
