using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    public DamageType type;
    public int power = 4;

    private void OnParticleCollision(GameObject other) {
        if (power > 0) {
            if (other.tag == "Player") {
                PlayerInfo player = other.GetComponent<PlayerInfo>();
                player.TakeDamage(power, this.transform.position, (int)type);
            } else if (LayerMask.LayerToName(other.layer) == "Enemy") {
                EnemyManager enemy = other.GetComponent<EnemyManager>();
                enemy.TakeDamage(false, null, 999, 1, false, null);
            }
        }
	}
}
