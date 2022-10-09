using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSwitch : MarioSwitch
{
    [Header("青コイン出現")]
    public int gimmickWaves = 1;
    
    public override IEnumerator Gimmick() {
        yield return null;
    }
}
