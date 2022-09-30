using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _00BoomBoom : BossManager
{
    public override IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.75f);
        
        while (true) {
            actionId = 1;
            yield return new WaitForSeconds(4f);
            actionId = 2;
            yield return new WaitForSeconds(5f);
            actionId = 0;
            yield return new WaitForSeconds(2f);
        }
    }
    public override IEnumerator DamageAnimation(){
        invincible = true;
        StopCoroutine("Attack");
        yield return new WaitForSeconds(1f);
        actionId = 3;
        yield return new WaitForSeconds(5f);
        actionId = 0;
        invincible = false;
        yield return new WaitForSeconds(0.25f);
        StartCoroutine("Attack");
    }
    public override IEnumerator DefeatedAnimation(){
        StopCoroutine("DamageAnimation");
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
