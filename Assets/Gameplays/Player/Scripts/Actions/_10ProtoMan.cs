using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _10ProtoMan : MegaManActions
{
    void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        ブルースでしかできない技
        ・攻撃力5のブルースストライク（チャージショット）
        ・ダッシュ
        */
        if (info != null){
            //プレイヤーIDを10に設定
            info.setPlayerId(10);
        }
    }
}
