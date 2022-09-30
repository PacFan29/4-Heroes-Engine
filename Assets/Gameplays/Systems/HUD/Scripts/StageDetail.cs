using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageDetail : MonoBehaviour
{
    [Header("ワールドA-B")]
    public int world;
    public int stage;
    [Header("データ")]
    public StageInfos data;
    [Header("その他出力")]
    public Text worldText;
    public Transform greenStar;
    public Text timeText;
    public Text scoreText;
    public Image rankImage;
    public Sprite[] rankSprites = new Sprite[7];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stage = Math.Max(0, WorldMapMainUI.step - 1);

        worldText.text = "WORLD " + (world + 1).ToString() + "-" + (stage + 1).ToString();
        
        StageInfos.Stage info = data.worlds[world].stages[stage];

        if (info.cleared) {
            timeText.text = getTimeFormat(info.bestTime);
            scoreText.text = info.highScore.ToString("###,###,##0");
            rankImage.gameObject.SetActive(true);
            rankImage.sprite = rankSprites[info.bestRank];
        } else {
            timeText.text = "--\'--\"--";
            scoreText.text = "-";
            rankImage.gameObject.SetActive(false);
        }

        //キーアイテム
        int KeyItemAmounts = info.greenStars.Length;

        RectTransform keyRect = (RectTransform)greenStar;
        if (KeyItemAmounts > 0) {
            if (info.greenStars[0]) {
                greenStar.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            } else {
                greenStar.gameObject.GetComponent<Image>().color = new Color(0.16f, 0.16f, 0.16f, 1);
            }
        } else {
            greenStar.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        foreach (Transform obj in greenStar.parent) {
            if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                Destroy(obj.gameObject);
            }
        }
			
		for ( int i = 1 ; i < KeyItemAmounts; i++ ) {
			keyRect = (RectTransform)Instantiate(greenStar).transform;
			keyRect.SetParent(greenStar.parent, false);
			keyRect.localPosition = greenStar.localPosition;

			keyRect.localPosition = new Vector2(
				keyRect.localPosition.x + (keyRect.sizeDelta.x * i),
				keyRect.localPosition.y
			);

            if (info.greenStars[i]) {
                keyRect.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            } else {
                keyRect.gameObject.GetComponent<Image>().color = new Color(0.16f, 0.16f, 0.16f, 1);
            }
		}
    }

     public string getTimeFormat(int totalTime){
        double minutes = totalTime / 60000;
        double seconds = (totalTime / 1000) % 60;
        double milli = (totalTime % 1000) / 10;
        return minutes.ToString("00") 
            + "\'" + seconds.ToString("00") 
            + "\"" + milli.ToString("00");
    }
}
