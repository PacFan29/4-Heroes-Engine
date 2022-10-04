using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _02Peach : MarioActions
{
    void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        ピーチ姫でしかできない技
        ・空中で少しの間浮く

        声の出演：?
        */
        if (info != null){
            //プレイヤーIDを2に設定
            info.setPlayerId(2);
        }
    }
}
