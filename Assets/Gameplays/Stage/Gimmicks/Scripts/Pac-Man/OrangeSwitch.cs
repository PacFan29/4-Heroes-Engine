using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSwitch : PacManSwitch
{
    [Header("時間制限")]
    public float timeLimit = 5f;
    [Header("対象オブジェクト")]
    public GimmickManager move;
    
    public override IEnumerator Gimmick() {
        if (move != null) move.gimmick = true;
        yield return new WaitForSeconds(timeLimit);

        pressed = false;
        if (move != null) move.gimmick = false;
    }
}
