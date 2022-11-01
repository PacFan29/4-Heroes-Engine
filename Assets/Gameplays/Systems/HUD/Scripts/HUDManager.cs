using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour {

	[Header("データ")]
	public GameData data;
	public Options options;
	[Header("オブジェクト")]
	public Text ScoreHeader;
	public Text[] Counters = new Text[4];
	public GameObject[] Groups = new GameObject[5];
	public GameObject[] TimeCoin = new GameObject[2];
	public GameObject[] KeyItemDisplays = new GameObject[2];
	private List<KeyItemDisplay> gotKeyItems = new List<KeyItemDisplay>();
	public Image CollectableIcon;
	[Header("プレイヤー")]
	public Transform playerParent;
	public RectTransform playerHealth;
	private int currentPlayerCount = 0;

	public static GameType HUDTypeGetter;

	//変数の宣言
	private double minutes;
	private double seconds;
	private double milli;

	public bool TimeType; //真＝減少、偽＝増加
	public int SecSetUp;

	private int currentScore;
	private int currentCoins;
	private float positionMemory;

	private int KeyItemAmounts;

	//画像のコントロール
	private RectTransform sec;
	private RectTransform min;
	public static float[] alert = new float [2] { 1f, 1f };

	[Header("画像")]
	public Sprite[] CollectableSprites = new Sprite[4];
	public Sprite[] KeyItemSprites = new Sprite[23];

	//グループのコントロール
	private bool[] displays = new bool[5];

	[Header("ポーズ")]
	public GameObject pauseMenu;
	private bool isPaused = false;
	[Header("ゲームオーバー")]
	public Animator GameOverManager;
	private int GameOverAnimNo = 0;

	private Animator anim;
	private Animator coinsAnim;
	private bool timeShow = false;

	// Use this for initialization
	void Start () {
		if (TimeType){
			GameManager.TotalTime = SecSetUp * 1000;
		} else {
			if (!data.isCpPassed) {
				GameManager.TotalTime = 0f;
			}
		}
		GameManager.Score = 0;
		
		GameManager gm = gameObject.AddComponent<GameManager>();
		Dictionary<string, string> stageInfo = data.stageInfo;
        gm.Reset(Int32.Parse(stageInfo["targetScore"]), Int32.Parse(stageInfo["targetTime"]), options.beginnerMode && data.HUDType == GameType.Sonic);

		min = GameObject.Find("MinutesArrow").GetComponent<RectTransform>();
		sec = GameObject.Find("SecondsArrow").GetComponent<RectTransform>();

		positionMemory = TimeCoin[1].GetComponent<RectTransform>().anchoredPosition.y;

		anim = GetComponent<Animator>();

		coinsAnim = TimeCoin[1].GetComponent<Animator>();

		//キーアイテム
		//値が0の場合は、隠す。
		KeyItemAmounts = data.greenStars.Length;
		if (KeyItemAmounts <= 0){
			KeyItemDisplays[1].GetComponent<Image>().color = new Color(1, 1, 1, 0);
		} else {
			KeyItemDisplays[1].GetComponent<Image>().color = new Color(1, 1, 1, 1);

			RectTransform keyRect = (RectTransform)KeyItemDisplays[1].transform;
			gotKeyItems.Add(keyRect.gameObject.GetComponent<KeyItemDisplay>());
			KeyItemDisplays[1].transform.localPosition = new Vector2(
				keyRect.localPosition.x - (keyRect.sizeDelta.x * (KeyItemAmounts-1) / 2f),
				keyRect.localPosition.y
			);
			
			GameManager.keyItems[0] = data.greenStars[0];
			switch (data.character){
				case Character.Mario:
				KeyItemDisplays[1].GetComponent<Image>().sprite = KeyItemSprites[0];
				break;
				case Character.PacMan:
				if (KeyItemAmounts == 7) {
					KeyItemDisplays[1].GetComponent<Image>().sprite = KeyItemSprites[7];
				} else {
					KeyItemDisplays[1].GetComponent<Image>().sprite = KeyItemSprites[1];
				}
				break;
				case Character.RockMan:
				KeyItemDisplays[1].GetComponent<Image>().sprite = KeyItemSprites[14];
				break;
				case Character.Sonic:
				KeyItemDisplays[1].GetComponent<Image>().sprite = KeyItemSprites[22];
				break;
			}
			
			for ( int i = 1 ; i < KeyItemAmounts; i++ ) {
				keyRect = (RectTransform)Instantiate(KeyItemDisplays[1]).transform;
				gotKeyItems.Add(keyRect.gameObject.GetComponent<KeyItemDisplay>());
				keyRect.SetParent(KeyItemDisplays[0].transform, false);
				keyRect.localPosition = KeyItemDisplays[1].transform.localPosition;

				keyRect.localPosition = new Vector2(
					keyRect.localPosition.x + (keyRect.sizeDelta.x * i),
					keyRect.localPosition.y
				);

				GameManager.keyItems[i] = data.greenStars[i];

				switch (data.character){
					case Character.Mario:
					keyRect.GetComponent<Image>().sprite = KeyItemSprites[0];
					break;
					case Character.PacMan:
					if (KeyItemAmounts == 7) {
						keyRect.GetComponent<Image>().sprite = KeyItemSprites[i+7];
					} else {
						keyRect.GetComponent<Image>().sprite = KeyItemSprites[i+1];
					}
					break;
					case Character.RockMan:
					keyRect.GetComponent<Image>().sprite = KeyItemSprites[14];
					break;
					case Character.Sonic:
					keyRect.GetComponent<Image>().sprite = KeyItemSprites[22];
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		anim.speed = (GameManager.ready) ? 1f : 0f;
		
		bool sonicStage = (data.HUDType == GameType.Sonic);

		//グループの表示・非表示（0=スコア  1=タイム  2=コイン  3=ゴールド  4＝グリーンスター）
		switch (data.HUDType){
			case GameType.Normal:
			//通常
			displays[0] = true;
			displays[1] = timeShow;
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

		if (SceneManager.GetActiveScene().name == "WorldMap") {
			//ワールドマップのみ、プレイヤーの情報以外を非表示にする。
			displays[0] = false;
			displays[1] = false;
			displays[2] = false;
			displays[3] = false;
			displays[4] = false;
		}

		HUDTypeGetter = data.HUDType;

        //サイズのセットアップ：ピクセルの2/9
        
		//点滅
		alert[0] += 7f * alert[1] * Time.deltaTime;
		if(alert[1] > 0f && alert[0] >= 1f || alert[1] < 0f && alert[0] <= 0f){
            alert[1] *= -1f;
		}

		ScoreDisp(sonicStage);
		TimeDisp(sonicStage);
		CoinsDisp();
		KeyItemsDisp();

		if (currentPlayerCount != GameManager.players.Count) {
			PlayerShow();
			currentPlayerCount = GameManager.players.Count;
		}


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
			timeShow = isPaused; //ポーズ時にタイムを表示

			Time.timeScale = isPaused ? 0f : 1f;
		}
		pauseMenu.SetActive(isPaused);

		GameOverManager.SetInteger("RouteID", GameOverAnimNo);
	}
	void ScoreDisp(bool sonicStage) {
		//スコアの表示・非表示
		if (displays[0] || displays[3]){
			Groups[1].GetComponent<CanvasGroup>().alpha = 1f;
		} else {
			Groups[1].GetComponent<CanvasGroup>().alpha = 0f;
		}

        //スコア（出力のみ,９桁まで）
        if(displays[0]){
			if (currentScore < GameManager.Score){
				currentScore += Math.Max(1, (int)((float)(GameManager.Score - currentScore) * 5f * Time.deltaTime));
			} else if (currentScore > GameManager.Score){
				currentScore -= Math.Max(1, (int)((float)(currentScore - GameManager.Score) * 5f * Time.deltaTime));
			}
			currentScore = Mathf.Clamp(currentScore, 0, GameManager.Score);

			if (GameManager.players.Count >= 2) {
				//２プレイヤー以降
				ScoreHeader.text = "TOTAL SCORE";
			} else {
				//１プレイヤーのみ
				ScoreHeader.text = "SCORE";
			}
			Counters[2].text = currentScore.ToString("###,###,##0");
		} else {
			GameManager.Score = (int)currentScore;
		}
		//ゴールド（出力のみ、９桁まで）
        if(displays[3]){
			ScoreHeader.text = "GOLD";
			Counters[2].text = "$ " + GameManager.Gold.ToString("###,###,##0");
		}
	}
	void TimeDisp(bool sonicStage) {
		//タイム
		//60分（1時間）でタイムアップ
		if (GameManager.ready && GameManager.timeIncrease) {
			if (TimeType){
				if (GameManager.TotalTime > 0){
					GameManager.TotalTime -= 1000 * Time.deltaTime;
				}
			} else {
				if (GameManager.TotalTime < 5999999){
					GameManager.TotalTime += 1000 * Time.deltaTime;
				}
			}
		}
		//値のセットアップ（1分＝60,000ミリ秒、1秒＝1,000ミリ秒）
		minutes = Math.Floor(GameManager.TotalTime / 60000);
        seconds = Math.Floor((GameManager.TotalTime / 1000) % 60);
		milli = Math.Floor((GameManager.TotalTime % 1000) / 10);
		//出力
        Counters[1].text = minutes.ToString("00") 
		+ "\'" + seconds.ToString("00") 
		+ "\"" + milli.ToString("00");
		//時計の針の傾き
		sec.transform.rotation = Quaternion.Euler (0, 0, ((GameManager.TotalTime / -1000f) * 6f) % 360f);
		min.transform.rotation = Quaternion.Euler (0, 0, ((GameManager.TotalTime / -60000f) * 6f) % 360f);
		
		if(TimeType && GameManager.TotalTime <= 60000){ 
			//タイムアップの１時間前（増加式）
			//タイムアップの１分前（減少式）
			Counters[1].color = new Color(1.0f, alert[0]*0.8f, alert[0]*0.8f, 1.0f);
		} else {
			Counters[1].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}
	void CoinsDisp() {
		//コインなど（出力のみ、３桁まで）
		if(displays[2]){
			switch (data.character){
				case Character.Mario:
				CollectableIcon.sprite = CollectableSprites[0];
				break;
				case Character.PacMan:
				CollectableIcon.sprite = CollectableSprites[1];
				break;
				case Character.RockMan:
				CollectableIcon.sprite = CollectableSprites[2];
				break;
				case Character.Sonic:
				CollectableIcon.sprite = CollectableSprites[3];
				break;
			}
		    Counters[0].text = GameManager.Coins.ToString("###,###,##0");
		    
			if (data.HUDType == GameType.Sonic) {
				if(GameManager.Coins <= 0){
					//モダンソニックのみ
					Counters[3].text = Counters[0].text;
					Counters[3].color = new Color(1.0f, alert[0], alert[0], 1.0f);
					Counters[3].gameObject.SetActive(true);
				} else {
					Counters[3].gameObject.SetActive(false);
				}
			}
		}
		
		if (currentCoins != GameManager.Coins) {
			if (currentCoins < GameManager.Coins) coinsAnim.Play("CoinsAdd", 0, 0);
			currentCoins = GameManager.Coins;
		}
		coinsAnim.SetBool("GotCoins", (GameManager.Coins > 0));

		//タイム・コインの表示・非表示
		TimeCoin[0].GetComponent<CanvasGroup>().alpha = alphaSet(displays[1]);
		TimeCoin[1].GetComponent<CanvasGroup>().alpha = alphaSet(displays[2]);
		RectTransform coinPos = TimeCoin[1].GetComponent<RectTransform>();
		if (!displays[1]){
			coinPos.anchoredPosition = TimeCoin[0].GetComponent<RectTransform>().anchoredPosition;
		} else {
			coinPos.anchoredPosition = new Vector2(coinPos.anchoredPosition.x, positionMemory);
		}
	}
	void KeyItemsDisp() {
		//キーアイテム
		for (int i = 0; i < gotKeyItems.Count; i++) {
			gotKeyItems[i].gotIt = GameManager.keyItems[i];
		}
		if (displays[4]) {
			Groups[4].GetComponent<CanvasGroup>().alpha = 1f;
		} else {
			Groups[4].GetComponent<CanvasGroup>().alpha = 0f;
		}
	}
	void PlayerShow() {
		foreach (Transform obj in playerParent) {
            if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                Destroy(obj.gameObject);
            }
        }
		for ( int i = 1 ; i < GameManager.players.Count ; i++ ) {
            //1の位の複製
            RectTransform ply = (RectTransform)Instantiate(playerHealth).transform;
            ply.SetParent(playerParent , false);
            ply.localPosition = new Vector2(
                ply.localPosition.x + ply.sizeDelta.x * i * 3.4f ,
                ply.localPosition.y);
			
			ply.gameObject.GetComponent<PlayerHealth>().playerNo = i;
        }
	}

	float alphaSet(bool show){
		if (show){
			return 1f;
		} else {
			return 0f;
		}
	}

	public void GameOver(){
		GameOverAnimNo = 1;
		return;
	}
}
