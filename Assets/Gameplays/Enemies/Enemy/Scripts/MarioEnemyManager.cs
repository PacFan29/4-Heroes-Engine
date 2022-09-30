using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioEnemyManager : EnemyManager
{
    public override IEnumerator DefeatedAnimation(bool stomped) {
        GetComponent<CapsuleCollider>().isTrigger = true;
        active = false;
        
        if (!stomped) {
            defeatedCase = 2;

            Vector3 distance;
            if (player != null) {
                distance = this.transform.position - player.transform.position;
            } else {
                distance = Vector3.zero;
            }
            distance.y = 0;
            
            skin.transform.forward = -distance.normalized;
            Vector3 hitVelocity = 10 * distance.normalized;
            hitVelocity.y = 45;
            velocity = hitVelocity;

            Grounded = false;
        } else {
            defeatedCase = 1;

            velocity = Vector3.zero;
        }

        float wait = stomped ? 0.4f : 0.6f;
        yield return new WaitForSeconds(wait);

        StartCoroutine("DestroyThisObject");
    }
}
