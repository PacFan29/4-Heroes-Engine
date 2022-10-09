using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitch : MarioSwitch
{
    [Header("キャラクター")]
    public int charIndex = 0;

    public override IEnumerator Gimmick() {
        yield return null;
    }
}
