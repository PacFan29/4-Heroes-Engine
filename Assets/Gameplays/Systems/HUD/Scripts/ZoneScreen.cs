using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ZoneScreen : MonoBehaviour
{
    public GameData data;
    public StageInfos infos;
    public RectTransform[] objects = new RectTransform[4];
    public Text[] recordDisps = new Text[2];
    public Image rankImage;
    public Sprite[] rankSprites = new Sprite[7];

    private float[] times = new float[4];
    private int[] records = new int[2];

    private bool goingDestroy;
    private Dictionary<string, string> stageInfo;
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < objects.Length; i++){
            Vector3 pos = objects[i].anchoredPosition;
            if (i == 0) {
                objects[i].anchoredPosition = new Vector3(1200, pos.y, pos.z);
            } else {
                objects[i].anchoredPosition = new Vector3(600, pos.y, pos.z);
            }
            times[i] = 20f * i;
        }

        StartCoroutine("LoadData");

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        stageInfo = data.stageInfo;

        objects[1].gameObject.GetComponent<Text>().text = "WORLD " + (Int32.Parse(stageInfo["worldNo"]) + 1).ToString() + "-" + (Int32.Parse(stageInfo["stageNo"]) + 1).ToString();
        objects[2].gameObject.GetComponent<Text>().text = stageInfo["subtitle"];

        StageInfos.Stage info = infos.worlds[Int32.Parse(stageInfo["worldNo"])].stages[Int32.Parse(stageInfo["stageNo"])];

        if (goingDestroy) {
            for (int i = 1; i < objects.Length; i++){
                Vector3 pos = objects[i].anchoredPosition;
                pos.x -= (600 + pos.x) * 4f * Time.deltaTime;
                objects[i].anchoredPosition = new Vector3(pos.x, pos.y, pos.z);
            }

            if (times[0] < 20){
                times[0] += 100 * Time.deltaTime;
            } else {
                Vector3 pos = objects[0].anchoredPosition;
                pos.x -= (1200 + pos.x) * 1.5f * Time.deltaTime;
                objects[0].anchoredPosition = new Vector3(pos.x, pos.y, pos.z);
            }
        } else {
            for (int i = 0; i < objects.Length; i++){
                if (times[i] > 0){
                    times[i] -= 100 * Time.deltaTime;
                } else {
                    Vector3 pos = objects[i].anchoredPosition;
                    objects[i].anchoredPosition = new Vector3(pos.x * (45f*Time.deltaTime), pos.y, pos.z);
                }
            }
        }
        
        if (info.cleared) {
            objects[3].gameObject.SetActive(true);

            recordDisps[0].text = info.highScore.ToString("###,###,##0");
            recordDisps[1].text = getTimeFormat(info.bestTime);

            rankImage.sprite = rankSprites[info.bestRank];
        } else {
            //記録なし
            objects[3].gameObject.SetActive(false);
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

    IEnumerator LoadData() {
        yield return new WaitForSeconds(1.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Int32.Parse(stageInfo["stageNo"]) + 3);

        while (!asyncLoad.isDone) {
            yield return null;
        }

        goingDestroy = true;
        StartCoroutine("DestroyScreen");
	}

    IEnumerator DestroyScreen() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
