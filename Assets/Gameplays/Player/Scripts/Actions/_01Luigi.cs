using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _01Luigi : MarioActions
{
    public void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        ルイージでしかできない技
        ・ハイジャンプ

        声の出演：Charles Martinet
        */
        if (info != null){
            //プレイヤーIDを1に設定
            info.setPlayerId(1);
        }
    }
}
