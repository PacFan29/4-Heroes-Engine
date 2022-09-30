using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSwitch : PacManSwitch
{
    [Header("時間制限")]
    public float timeLimit = 5f;
    
    public override IEnumerator Gimmick() {
        yield return new WaitForSeconds(timeLimit);

        pressed = false;
    }
}
