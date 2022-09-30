using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePin : MonoBehaviour
{
    [Header("データ")]
    public GameData data;
    public StageInfos stages;
    [Header("ステージ番号")]
    public int stageNo;

    private bool placed = false;
    private int world = 0;
    private StageInfos.Stage stg;
    [Header("ステージピン")]
    public MeshRenderer lightPart;
    public Material[] materials = new Material[3];
    [Header("ランク")]
    public GameObject rank;
    public Sprite[] rankSprites = new Sprite[7];
    // Start is called before the first frame update
    void Start()
    {
        world = 0;
        stg = stages.worlds[world].stages[stageNo];

        if (stg.stageType == GameType.Boss || stg.stageType == GameType.SonicBoss) {
            stg.targetScore = 50000;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Start") && placed && stg.unlocked) {
            if (WorldMapMainUI.step > 0) {
                WorldMapMainUI.step = 0;
                StageEnter();
            } else {
                WorldMapMainUI.step = stageNo + 1;
            }
        }

        if (Input.GetButtonDown("B") && WorldMapMainUI.step > 0) {
            WorldMapMainUI.step = 0;
        }

        Material[] mat = new Material[1];
        mat[0] = materials[0];
        if (stg.cleared) {
            mat[0] = materials[2];
        } else if (stg.unlocked) {
            mat[0] = materials[1];
        }
        lightPart.materials = mat;

        rank.GetComponent<SpriteRenderer>().sprite = rankSprites[stg.bestRank];
        rank.SetActive(stg.cleared);
    }

    void OnCollisionStay(Collision col) {
        if (col.gameObject.tag == "Player" && !placed) {
            placed = true;
        }
    }
    void OnCollisionExit(Collision col) {
        if (col.gameObject.tag == "Player" && placed) {
            placed = false;
        }
    }

    void StageEnter() {
        data.stageInfo["worldNo"] = world.ToString();
        data.stageInfo["stageNo"] = stageNo.ToString();
        data.stageInfo["targetScore"] = stg.targetScore.ToString();
        data.stageInfo["targetTime"] = stg.targetTime.ToString();
        data.stageInfo["subtitle"] = stg.subtitle;

        data.HUDType = stg.stageType;

        if (stg.stageType == GameType.Sonic || stg.stageType == GameType.SonicBoss) {
            data.character = Character.Sonic;
        } else {
            data.character = Character.Mario;
        }

        data.openWorldAsDefault = stg.openWorldAsDefault;

        data.greenStars = stg.greenStars;

        SceneManager.LoadScene("ZoneScreen");
    }
}
