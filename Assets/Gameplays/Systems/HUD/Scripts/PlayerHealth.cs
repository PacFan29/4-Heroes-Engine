using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
～色の分類～

マリオ：赤
ルイージ：緑
ピーチ姫：桃
ヨッシー：緑
ワリオ：黄

パックマン：黄
ミズ・パックマン：黄
パック・ジュニア：黄

ロックマン：水
ロール：桃
ブルース：赤
フォルテ：紫

クラシックソニック：水
テイルス：黄
エミー：桃
ナックルズ：赤
モダンソニック：青
*/
public class PlayerHealth : MonoBehaviour {

	//変数の宣言
	[Header("オプション")]
	public Options options;
	[Header("アイコン")]
	public Sprite[] icons = new Sprite[17];
	public Image icon;
	public GameObject boostHeader;
	[Header("前面枠")]
	public Image HealthFront;
	public Sprite[] HealthFrontSprites = new Sprite[4];
	private int characterNo = 0;
	private int colors;
	private bool boost;
	private bool pinch;
	private int max_health;
	private int memory;
	private int health;
    private float health_percent;
	private float damage_amount;
	private int lives;
	private float animTime;
	private float yPosMemory;

	//オブジェクトの宣言
	[Header("メイン")]
	public Image main;
	[Header("バー")]
	public Image healthAmount;
	public Image damage;
	[Header("スキルバー")]
	public Image skillAmount;
	[Header("ブースト")]
	public Image extraBoost;
	public Color[] barColors = new Color[4];
	[Header("残機")]
	public GameObject LivesGroup;
	public Text LivesCounter;
	[Header("P1～P4")]
	public int playerNo = 0;
	public GameObject PlayerDisplay;
	private RectTransform rect;

	private bool ready = false;
	private Animator livesAnim;
	private int currentLives = 99;
	private bool livesUp = false;
	// Use this for initialization
	void Start () {
		lives = 0;
		health_percent = 1.00f;
		damage_amount = health_percent;
		memory = 50;

		rect = GetComponent<RectTransform>();
		yPosMemory = rect.anchoredPosition.y;

		livesAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		ready = GameManager.players.Count > playerNo;

		if (ready) {
			characterNo = GameManager.players[playerNo].getStatus()[0];
		} else {
			//マリオをデフォルトとして
			characterNo = 0;
		}
		colors = colorSetUp(characterNo);
		boost = (characterNo == 16);

		//色の調節
		switch (colors){
			case 0: //赤
			main.color = new Color(1f, 0f, 0f, 1f);
			break;
			case 1: //黄
			main.color = new Color(1f, 0.87f, 0f, 1f);
			break;
			case 2: //緑
			main.color = new Color(0f, 0.85f, 0.25f, 1f);
			break;
			case 3: //水
			main.color = new Color(0f, 0.75f, 1f, 1f);
			break;
			case 4: //青
			main.color = new Color(0f, 0.34f, 1f, 1f);
			break;
			case 5: //紫
			main.color = new Color(0.7f, 0f, 1f, 1f);
			break;
			case 6: //桃
			main.color = new Color(1f, 0.4f, 0.9f, 1f);
			break;
		}
		HealthFront.color = main.color;
		if (boost){
			healthAmount.color = new Color(0f, 1f, 1f, 1f);
			pinch = (GameManager.Coins <= 0);
		} else {
			int colorIndex = 0;
			if (GameManager.extra) {
				colorIndex = Math.Min(2, (int)Math.Ceiling((health_percent * 100) / 20f) - 1);
			} else {
				colorIndex = (int)Math.Ceiling((health_percent * 100) / 25f) - 1;
			}
			if (colorIndex > 0){
				pinch = false;
				if (GameManager.extra){
					HealthFront.sprite = HealthFrontSprites[1];
				} else {
					HealthFront.sprite = HealthFrontSprites[0];
				}
				healthAmount.color = barColors[colorIndex];
			} else {
				pinch = true;
				healthAmount.color = new Color(1f, HUDManager.alert[0]*0.8f, HUDManager.alert[0]*0.8f, 1f);
			}
		}

		//ライフ（100%バー）
		if (ready){
			max_health = GameManager.players[playerNo].getStatus()[1];
			health = GameManager.players[playerNo].getStatus()[2];
			
			if (!boost && memory > max_health){
				memory = max_health;
			} else if (boost && memory > 999){
				memory = 999;
			}
			//ダメージ・回復時のアニメーション
			if (!boost) {
				if (health > memory){
					//回復
					memory = health;
					animTime = 0.3f;
				} else if (health < memory) {
					//ダメージ
					memory = health;
					if (health > 0) animTime = -0.3f;
				}
			}
			float yPos = yPosMemory;
			if (animTime > 0){
				//回復
				animTime -= Time.deltaTime;
				if (animTime <= 0) animTime = 0f;
			} else if (animTime < 0) {
				//ダメージ
				yPos += UnityEngine.Random.Range(-0.2f, 0.2f) * (animTime * -150); //揺れ
				animTime += Time.deltaTime;
				if (animTime >= 0) animTime = 0f;
			} else {
				//増加・減少
				if (health_percent < damage_amount){
					if (health <= 0) damage_amount = 0;
					damage_amount -= 0.5f * Time.deltaTime;
					if (health_percent >= damage_amount) damage_amount = health_percent;
				} else if (health_percent > damage_amount){
					damage_amount += 0.5f * Time.deltaTime;
					if (health_percent <= damage_amount) damage_amount = health_percent;
				}
			}
			rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, yPos);
		} else {
			max_health = 50;
			health = 50;
		}
		health_percent = (float)health / max_health;
		
		//バー
		if (boost) {
			healthAmount.fillAmount = health_percent;
			damage.fillAmount = health_percent;
			if (extraBoost != null) extraBoost.fillAmount = (float)Math.Max(0f, (health - 100f)) / 400f;
		} else {
			if (health_percent > damage_amount){
				healthAmount.fillAmount = damage_amount;
				damage.fillAmount = health_percent;
			} else {
				healthAmount.fillAmount = health_percent;
				damage.fillAmount = damage_amount;
			}
			if (extraBoost != null) extraBoost.fillAmount = 0f;
		}

		//スキルバー表示
		Vector3 barScale = Vector3.one;
		bool skillShow = false;
		if (skillShow) {
			barScale = new Vector3(1f, 0.55f, 1f);
			if (GameManager.extra){
				HealthFront.sprite = HealthFrontSprites[3];
			} else {
				HealthFront.sprite = HealthFrontSprites[2];
			}
		}
		healthAmount.gameObject.GetComponent<RectTransform>().localScale = barScale;
		damage.gameObject.GetComponent<RectTransform>().localScale = barScale;
		skillAmount.gameObject.SetActive(skillShow);
		
		//残機（出力のみ、２桁まで）
		LivesGroup.SetActive(HUDManager.HUDTypeGetter != GameType.BossRush && options.lives); //ボスラッシュ時のみ非表示
		if (ready) lives = GameManager.players[playerNo].getStatus()[3]; //残機の取得
        LivesCounter.text = lives.ToString();

		if (currentLives != lives) {
			if (currentLives < lives && FadeManager.alpha <= 0) {
				livesAnim.Play("LivesUp", 0, 0);
				livesUp = true;
			}
			currentLives = lives;
		}
		livesAnim.SetBool("Lives", livesUp);

		//アイコン
		icon.sprite = icons[characterNo];
		//Mario icons ripped by Joshuat1306 (DeviantArt)

		if (boostHeader != null) {
			boostHeader.SetActive(boost);
		}

		//P1～P4
		playerNo = Mathf.Clamp(playerNo, 0, 3);
		PlayerDisplay.SetActive(GameManager.players.Count > 1);
		PlayerDisplay.GetComponent<Text>().text = "P" + (playerNo+1);
	}

	int colorSetUp (int characterNo){
		int color = 0;

		switch(characterNo){
			/* 赤 */
			case 0: //マリオ
			case 10: //ブルース
			case 15: //ナックルズ
			color = 0;
			break;

			/* 黄 */
			case 4: //ワリオ
			case 5: //パックマン
			case 6: //ミズ・パックマン
			case 7: //パック・ジュニア
			case 13: //テイルス
			color = 1;
			break;

			/* 緑 */
			case 1: //ルイージ
			case 3: //ヨッシー
			color = 2;
			break;

			/* 水 */
			case 8: //ロックマン
			case 12: //クラシックソニック
			color = 3;
			break;

			/* 青 */
			case 16: //モダンソニック
			color = 4;
			break;
			
			/* 紫 */
			case 11: //フォルテ
			color = 5;
			break;

			/* 桃 */
			case 2: //ピーチ姫
			case 9: //ロール
			case 14: //エミー
			color = 6;
			break;
		}

		return color;
	}
}
