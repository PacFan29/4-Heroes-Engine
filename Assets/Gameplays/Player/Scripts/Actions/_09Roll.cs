using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _09Roll : MegaManActions
{
    void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        ロールでしかできない技
        ・ダッシュ
        ・ほうきで攻撃
        */
        if (info != null){
            //プレイヤーIDを9に設定
            info.setPlayerId(9);
        }
    }
}
