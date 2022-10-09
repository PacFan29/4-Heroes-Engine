using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType {
    Normal,
    Fire,
    Ice,
    Electricity,
    Spike,
}

public class TriggerDamage : MonoBehaviour
{
    public DamageType type;
    public int power = 4;

    void OnCollisionStay(Collision col)
    {
        if (power > 0) {
            if (col.gameObject.tag == "Player") {
                PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();
                player.TakeDamage(power, this.transform.position, (int)type);
            } else if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
                EnemyManager enemy = col.gameObject.GetComponent<EnemyManager>();
                enemy.TakeDamage(false, null, 999, 1, false, null);
            }
        }
    }
    void OnTriggerEnter(Collider col){
        if (power > 0) {
            if (col.gameObject.tag == "Player") {
                PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();
                player.TakeDamage(power, this.transform.position, (int)type);
            } else if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
                EnemyManager enemy = col.gameObject.GetComponent<EnemyManager>();
                enemy.TakeDamage(false, null, 999, 1, false, null);
            }
        }
    }
}
