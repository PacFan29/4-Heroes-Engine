using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSwitch : PacManSwitch
{
    [Header("対象オブジェクト")]
    public GimmickManager move;

    public override IEnumerator Gimmick() {
        if (move != null) move.gimmick = true;
        yield return null;
    }
}
