using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GlobalData
{
    public bool[] clearedStages;
    public int[] playerLives;
    public int[] playerHPs;
    public int[] coins;

    public GlobalData(bool[] clearedStages, int[] playerLives, int[] playerHPs, int[] coins){
        this.clearedStages = clearedStages;
        this.playerLives = playerLives;
        this.playerHPs = playerHPs;
        this.coins = coins;
    }
}
public class CommonStageData
{
    public int time; //0～5,999,999まで（float型なので、小数点以下を切り捨てにする）
    public string rank; //"F"～"S"まで
}
public class StageData : CommonStageData
{
    public int score; //0～999,999,999まで
    public bool[] greenStars; //例）[false, false, true, false, true]

    public StageData(int score, int time, bool[] greenStars, string rank){
        this.score = score;
        this.time = time;
        this.greenStars = greenStars;
        this.rank = rank;
    }
}
public class WarioStageData : CommonStageData
{
    public int gold; //0～999,999,999まで
    public bool[] diamonds; //例）[false, false, true, false, true]
}
public class BossStageData : CommonStageData
{
    public BossStageData(int time, string rank){
        this.time = time;
        this.rank = rank;
    }
}
public class SaveManager : MonoBehaviour
{
    string filePath;
    StageData save;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Save()
    {
        filePath = pathSet(0, 0);

        save = new StageData(
            /* ステージのデータ */
            999999999, 
            5999999, 
            new bool[5]{true, true, true, true, true},
            "S"
        );
        string json = JsonUtility.ToJson(save);
 
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log(json);
    }
    void Load()
    {
        filePath = pathSet(0, 0);

        if (File.Exists(filePath))
        {
            StreamReader streamReader;
            streamReader = new StreamReader(filePath);
            string data = streamReader.ReadToEnd();
            streamReader.Close();
 
            save = JsonUtility.FromJson<StageData>(data);

            // save.score;
            // save.time;
            // save.greenStars;
            // save.rank;
        }
    }
    public string pathSet(int fileNo, int stageNo){
        return Application.persistentDataPath + "/file" + fileNo + "/stage" + stageNo + ".json";
    }
}
