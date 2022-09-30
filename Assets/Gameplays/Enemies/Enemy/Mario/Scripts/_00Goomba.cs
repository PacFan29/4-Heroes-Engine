using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _00Goomba : EnemyMovements
{
    // Update is called once per frame
    public override void Movements()
    {
        Vector3 runVel = Vector3.zero;

        time -= Time.deltaTime;

        switch (action) {
            case 0:
            runVel = Vector3.zero;
            if (time <= 0f) {
                action = 1;
                time = 1f;
                direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                direction = direction.normalized;
            }
            break;

            case 1:
            runVel = 3 * direction;
                
            if (time <= 0f) {
                action = 0;
                time = 0.2f;
            }
            break;

            case 2:
            direction = targetPlayer.transform.position - this.transform.position;
            direction.y = 0;

            runVel = 10 * direction.normalized;
            break;

            default:
            runVel = Vector3.zero;
            break;
        }

        if (action < 2) {
            SetPlayer();
        }

        if (PlayerFound()) {
            if (action < 2) {
                action = 3;
                StartCoroutine("Attack");
            }
            time = 0.2f;
        } else {
            if (action > 1) action = 0;
        }

        manager.velocity.x = runVel.x;
        manager.velocity.z = runVel.z;
    }

    IEnumerator Attack() {
        manager.actionId = 1;
        manager.skin.transform.forward = (targetPlayer.transform.position - this.transform.position).normalized;
        yield return new WaitForSeconds(0.56f);

        if (manager.isActive() && manager.isNear()) {
            manager.actionId = 0;
            action = 2;
        }
    }
}
