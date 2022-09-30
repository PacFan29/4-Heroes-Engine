using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicEnemyManager : EnemyManager
{
    //[Header("その他")]
    // Update is called once per frame

    public override IEnumerator DefeatedAnimation(bool stomped) {
        GetComponent<CapsuleCollider>().isTrigger = true;
        active = false;
        
        if (!Flying) {
            if (!stomped) {
                defeatedCase = 2;

                Vector3 distance = this.transform.position - player.transform.position;
                Vector3 hitVelocity = 30 * distance.normalized;
                velocity = hitVelocity;

                Grounded = false;
            } else {
                defeatedCase = 1;
            }

            yield return new WaitForSeconds(0.6f);
        }

        StartCoroutine("DestroyThisObject");
    }
}
