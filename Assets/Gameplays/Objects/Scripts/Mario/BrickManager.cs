using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : QuestionBlockManager
{
    public override void DestroyBrick(PlayerInfo player) {
        StartCoroutine("DestroyBlock", player);
    }
}
