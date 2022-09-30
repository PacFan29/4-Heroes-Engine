using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType {
	Normal,
	Boss,
	Sonic,
	SonicBoss,
	Wario,
	BossRush,
	Trail
}
public enum Character {
	Mario,
	PacMan,
	RockMan,
	Sonic,
    Other
}
public enum HUDStyle {
    Default,
    Mario3DWorld,
	PacManWorld,
	RockMan11,
	SonicMania,
	SonicWorldAdventure,
	SonicColors,
	SonicGenerations,
	SonicForces
}

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/Game Data")]
public class GameData : ScriptableObject
{
    [Serializable]
    public class Item {
        public int amounts;
        public bool megaManOnly;
        public bool special;
        public int limit;
        [Header("アイコン")]
        public Sprite[] sprites;
    }

    [Header("コイン総計")]
    public int coins = 0;
    [Header("ステージの形式")]
    public GameType HUDType = GameType.Normal;
    public Character character = Character.Mario;
    public HUDStyle HUDStyle = HUDStyle.Default;
    public bool openWorldAsDefault = false;
    [Header("プレイヤー")]
    public int[] characterIds = {12, 5, 8, 12};
    public int[] HPs = {30, 30, 30, 30};
    public int[] lives = {2, 2, 2, 2};
    public int[] powerUps = {0, 0, 0, 0};
    public int[] shields = {0, 0, 0, 0};
    public Dictionary<string, string> stageInfo = new Dictionary<string, string>() {
        {"worldNo", "0"},
        {"stageNo", "0"},
        {"targetScore", "50000"},
        {"targetTime", "600"},
        {"subtitle", "基本的なステージ"}
    };
    [Header("アイテムストック")]
    public Item[] stockItems = new Item[7];
    [Header("グリーンスター")]
    public bool[] greenStars = new bool[3];
    [Header("チェックポイント")]
    public float totalTime = 0f;

    public bool isCpPassed = false;
    public Vector3 CpPosition;
    
    public void ResetAll() {
        ResetCheckPoint();

        if (HUDType == GameType.Sonic) character = Character.Sonic;

        for (int i = 0; i < lives.Length; i++) {
            lives[i] = 2;
        }
    }
    public void ResetExceptTime() {
        Lives();
        GameManager.TotalTime = totalTime;
    }
    public void LevelComplete() {
        Lives();
        totalTime = 0f;
    }
    public void Lives() {
        int index = 0;
        foreach (PlayerStatus player in GameManager.players) {
            lives[index] = player.getStatus()[3];
            index++;
        }
    }

    public void LostLife(int playerNumber) {
        Lives();
        lives[playerNumber]--;
    }

    public void ResetCheckPoint() {
        totalTime = 0f;
        isCpPassed = false;
        CpPosition = Vector3.zero;
    }
}
