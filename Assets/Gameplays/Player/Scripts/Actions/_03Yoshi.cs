using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _03Yoshi : MarioActions
{
    void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        ヨッシーでしかできない技
        ・敵を食べる
        ・卵を産む
        ・卵を投げる
        ・ヒップドロップ

        声の出演：?
        */
        if (info != null){
            //プレイヤーIDを3に設定
            info.setPlayerId(2);
        }
    }
}
