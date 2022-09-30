using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComboManager : MonoBehaviour
{
    public int combo = 0;
    public PlayerInfo player;

    public int ComboIncrease(){
        //コンボ上昇（ただし5まで）
        if (combo < 99) combo++;
        if (GameManager.maxCombo < combo) GameManager.maxCombo = combo;

        if (combo >= 8) {
            player.OneUp();
        }

        return Math.Min(5, combo);
    }
    public int GetCombo(){
        //コンボの取得
        return Math.Min(5, combo);
    }
    public void ComboReset(){
        //コンボの初期化
        combo = 0;
    }
}
