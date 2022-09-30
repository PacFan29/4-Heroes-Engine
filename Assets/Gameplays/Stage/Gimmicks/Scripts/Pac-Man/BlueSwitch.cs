using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSwitch : PacManSwitch
{
    [Header("順番")]
    public int index;

    public override IEnumerator Gimmick() {
        yield return null;
    }
}
