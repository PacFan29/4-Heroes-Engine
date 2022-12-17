using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDamage : ComboManager
{
    public float lifeTime = 1f;
    private bool isTrigger = true;

    private float damageTime = 0f;

    [HideInInspector] public WeaponTypes weaponType = WeaponTypes.None;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine("LifeTime");
    }

    // Update is called once per frame
    void Update()
    {
        if (damageTime > 0) {
            damageTime -= Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider col) {
        if (damageTime <= 0 && isTrigger) {
            if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
                EnemyManager enemy = col.GetComponent<EnemyManager>();
                enemy.TakeDamage(true, player, 6, 1, false, this, weaponType);

                //damageTime = 0.25f;
            } else if (LayerMask.LayerToName(col.gameObject.layer) == "Boss"){
                BossManager boss = col.GetComponent<BossManager>();
                boss.Damage(player, 1, false);
                
                //damageTime = 0.5f;
            }
        }
    }

    IEnumerator LifeTime() {
        yield return new WaitForSeconds(lifeTime);

        isTrigger = false;
    }
}
