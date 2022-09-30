using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
	public Image BossHealthAmount;
	public Image BossDamageAmount;
	public Text BossName;
    public Transform borderLine;
	private int b_max_health = 60;
	private float b_yPosMemory;
	private float b_yPos;
	private int b_fillMemory;
	private int b_health = 60;
    private float b_health_percent;
	private float b_damage_amount;
	private float b_animTime;

    public Color[] barColors = new Color[4];

    // Start is called before the first frame update
    void Start()
    {
        b_yPosMemory = this.GetComponent<RectTransform>().anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        //ボスゲージ
		if (GameManager.bossInfo != null) {
			b_health = GameManager.bossInfo.totalHp;
			b_max_health = GameManager.bossInfo.maxHp;
			BossName.text = GameManager.bossInfo.gameObject.name;
		}

        RectTransform BossRect = this.GetComponent<RectTransform>();

		if (GameManager.bossHPShow) {
			b_health = Mathf.Clamp(b_health, 0, b_max_health);
			b_health_percent = (float)b_health / b_max_health;

			//ダメージ・回復時のアニメーション
			if (b_health > b_fillMemory){
				//回復
				b_fillMemory = b_health;
			} else if (b_health < b_fillMemory) {
				//ダメージ
				b_fillMemory = b_health;
				b_animTime = 0.3f;
			}
			float b_yPos = b_yPosMemory;
			if (b_animTime > 0){
				//ダメージ
				b_yPos += UnityEngine.Random.Range(-0.2f, 0.2f) * (b_animTime * -150); //揺れ
				b_animTime -= Time.deltaTime;
				BossRect.anchoredPosition = new Vector2(BossRect.anchoredPosition.x, b_yPos);
				if (b_animTime <= 0) b_animTime = 0f;
			} else {
				//増加・減少
				if (b_health_percent < b_damage_amount){
					b_damage_amount -= 0.5f * Time.deltaTime;
					if (b_health_percent >= b_damage_amount) b_damage_amount = b_health_percent;
				} else if (b_health_percent > b_damage_amount){
					b_damage_amount += 0.5f * Time.deltaTime;
					if (b_health_percent <= b_damage_amount) b_damage_amount = b_health_percent;
				}
			}

            bool isPinch = false;
			//バー
			if (b_health_percent > b_damage_amount){
				BossHealthAmount.fillAmount = b_damage_amount;
				BossDamageAmount.fillAmount = b_damage_amount;
			} else {
				BossHealthAmount.fillAmount = b_health_percent;
				BossDamageAmount.fillAmount = b_damage_amount;
			}
            BossHealthAmount.color = barColors[GameManager.bossInfo.phase];

            if (GameManager.bossInfo.PhasePerHP.Length > 0) {
                //段階の境界線
                borderLine.gameObject.SetActive(true);

                float barSize = BossHealthAmount.GetComponent<RectTransform>().sizeDelta.x;
                float emptyXpos = barSize / 2f;
                float rate = (float)GameManager.bossInfo.PhasePerHP[0] * GameManager.bossInfo.HitsPerHP * 4 / b_max_health;

                borderLine.localPosition = new Vector3(emptyXpos - (barSize * rate), 0, 0);

				//複数
				Transform parent = BossHealthAmount.gameObject.transform;
				foreach (Transform obj in parent) {
					if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
						Destroy(obj.gameObject);
					}
				}
				for (int i = 1; i < GameManager.bossInfo.PhasePerHP.Length; i++) {
					rate = (float)GameManager.bossInfo.PhasePerHP[i] * GameManager.bossInfo.HitsPerHP * 4 / b_max_health;

					RectTransform bd = (RectTransform)Instantiate(borderLine).transform;
					bd.SetParent(parent , false);
					bd.localPosition = new Vector3(emptyXpos - (barSize * rate), 0, 0);
				}

                isPinch = GameManager.bossInfo.phase == 0;
            } else {
                borderLine.gameObject.SetActive(false);

                isPinch = b_health_percent <= 0.5;
            }

            if (isPinch) {
                BossHealthAmount.color = new Color(1f, HUDManager.alert[0]*0.8f, HUDManager.alert[0]*0.8f, 1f);
            }
		}
    }
}
