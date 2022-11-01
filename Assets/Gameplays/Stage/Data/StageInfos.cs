using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageInfos", menuName = "ScriptableObjects/Stage Infos")]
public class StageInfos : ScriptableObject
{
    [Serializable]
    public class World {
        public Stage[] stages = new Stage[8];
    }
    [Serializable]
    public class Stage {
        [Header("目標スコア")]
        public int targetScore = 50000;
        [Header("目標タイム（秒単位）")]
        public int targetTime = 600;
        [Header("ステージの形式")]
        public GameType stageType;
        public bool openWorldAsDefault = false;
        public bool greenStarsFromScratch = false;
        [Header("サブタイトル")]
        public string subtitle = "基本的なステージ";

        [Space(1)]

        [Header("記録")]
        public bool cleared = false;
        public bool unlocked = true;
        public int highScore = 0;
        public int bestTime = 5999999;
        public bool[] greenStars = new bool[3];
        public int bestRank = 0;

        public void Initialize() {
            cleared = false;
            unlocked = false;
            highScore = 0;
            bestTime = 5999999;
            for (int i = 0; i < greenStars.Length; i++) {
                greenStars[i] = false;
            }
            bestRank = 0;
        }
    }

    public World[] worlds = new World[8];

    public int getAllGreenStars(int world) {
        //グリーンスター全体の数を取得
        int total = 0;
        foreach (Stage stage in worlds[world].stages) {
            total += stage.greenStars.Length;
        }
        return total;
    }
    public int getCurrentGreenStars(int world) {
        //現在獲得しているグリーンスターの数を取得
        int total = 0;
        foreach (Stage stage in worlds[world].stages) {
            foreach (bool single in stage.greenStars) {
                if (single) total++;
            }
        }
        return total;
    }
    public int getAllBestRankCount(int world) {
        //Ｓランク全体の数（全ステージ数）を取得
        return worlds[world].stages.Length;
    }
    public int getCurrentBestRankCount(int world) {
        //現在取っているＳランクの数を取得
        int total = 0;
        foreach (Stage stage in worlds[world].stages) {
            if (stage.bestRank == 6) total++;
        }
        return total;
    }
    public int getPercent(int world) {
        int max = 0;
        max += getAllGreenStars(world);
        max += getAllBestRankCount(world) * 2;

        int current = 0;
        foreach (Stage stage in worlds[world].stages) {
            if (stage.cleared) current++;
        }
        current += getCurrentGreenStars(world);
        current += getCurrentBestRankCount(world);

        return (int)Math.Floor(((float)current / (float)max) * 100f);
    }
}
