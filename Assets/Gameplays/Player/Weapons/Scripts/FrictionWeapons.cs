using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionWeapons : ComboManager
{
    [HideInInspector] public int index;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LifeTime());
    }
    void Update() {
        for (int i = 0; i < this.transform.childCount; i++) {
            this.transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }


    void OnTriggerStay(Collider col) {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
            EnemyManager enemy = col.GetComponent<EnemyManager>();
            enemy.TakeDamage(true, player, 6, 1, false, this);
        } else if (LayerMask.LayerToName(col.gameObject.layer) == "Boss"){
            BossManager boss = col.GetComponent<BossManager>();
            boss.Damage(player, 2, false);
        }
    }

    IEnumerator LifeTime() {
        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }
}
