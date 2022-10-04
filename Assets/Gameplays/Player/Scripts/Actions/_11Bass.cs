using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _11Bass : MegaManActions
{
    void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        ブルースでしかできない技
        ・バスター連射
        ・ダッシュ
        ・ダブルジャンプ
        */
        if (info != null){
            //プレイヤーIDを11に設定
            info.setPlayerId(11);
        }
    }
}
