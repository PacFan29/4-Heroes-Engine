using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDCommon : MonoBehaviour
{
    [Header("データ")]
	public GameData data;
	public Options options;
    [Header("ポーズ")]
	public GameObject pauseMenu;
	private bool isPaused = false;
	[Header("ゲームオーバー")]
	public Animator GameOverManager;
	private int GameOverAnimNo = 0;

    protected int[] timeValues = new int[3];

	private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        GameManager gm = this.GetComponent<GameManager>();
		Dictionary<string, string> stageInfo = data.stageInfo;
        gm.Reset(Int32.Parse(stageInfo["targetScore"]), Int32.Parse(stageInfo["targetTime"]), options.beginnerMode && data.HUDType == GameType.Sonic);

		anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
		anim.speed = (GameManager.ready) ? 1f : 0f;

        bool[] displays = new bool[5];
        switch (data.HUDType){
			case GameType.Normal:
			//通常
			displays[0] = true;
			displays[1] = true;
			displays[2] = true;
			displays[3] = false;
			displays[4] = true;
			break;

			case GameType.Boss:
			case GameType.BossRush:
			//vs. ボス
			displays[0] = false;
			displays[1] = true;
			displays[2] = false;
			displays[3] = false;
			displays[4] = false;
			break;

			case GameType.Sonic:
			//モダンソニックステージ
			displays[0] = true;
			displays[1] = true;
			displays[2] = true;
			displays[3] = false;
			displays[4] = true;
			break;

			case GameType.SonicBoss:
			//モダンソニック vs. ボス
			displays[0] = false;
			displays[1] = true;
			displays[2] = false;
			displays[3] = false;
			displays[4] = false;
			break;

			case GameType.Wario:
			//ワリオステージ
			displays[0] = false;
			displays[1] = false;
			displays[2] = false;
			displays[3] = true;
			displays[4] = true;
			break;

			case GameType.Trail:
			//おためし部屋
			displays[0] = true;
			displays[1] = false;
			displays[2] = false;
			displays[3] = false;
			displays[4] = false;
			break;
		}
		GameManager.ScoreShow = displays[0];

        if (GameManager.ready && GameManager.timeIncrease) {
			if (GameManager.TotalTime < 5999999){
				GameManager.TotalTime += 1000 * Time.deltaTime;
			}
		}
        timeValues[0] = (int)Math.Floor(GameManager.TotalTime / 60000);
        timeValues[1] = (int)Math.Floor((GameManager.TotalTime / 1000) % 60);
		timeValues[2] = (int)Math.Floor((GameManager.TotalTime % 1000) / 10);

        ScoreDisp();
        TimeDisp();
        CoinsDisp();
		if (GameManager.bossHPShow) BossHealth();

        //アニメーターの処理
		if (GameManager.gameOver){
			//ゲームオーバー
			GameOver();
		}
		anim.SetInteger("animNo", GameManager.hudAnimNo);
		anim.updateMode = AnimatorUpdateMode.UnscaledTime;

        //ポーズ
		if (Input.GetButtonDown("Start")) {
			isPaused = !isPaused;

			Time.timeScale = isPaused ? 0f : 1f;
		}
		pauseMenu.SetActive(isPaused);

		GameOverManager.SetInteger("RouteID", GameOverAnimNo);
    }
    public virtual void ScoreDisp() {
        ;
    }
    public virtual void TimeDisp() {
        ;
    }
    public virtual void CoinsDisp() {
        ;
    }

    public virtual void BossHealth() {
        ;
    }

    public void GameOver(){
		GameOverAnimNo = 1;
		return;
	}
}
