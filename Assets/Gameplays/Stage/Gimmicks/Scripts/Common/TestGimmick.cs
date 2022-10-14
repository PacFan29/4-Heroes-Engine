using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGimmick : GimmickManager
{
    public override void gimmickMove() {
        this.transform.Rotate(0f, 0f, 1f);
    }
}
