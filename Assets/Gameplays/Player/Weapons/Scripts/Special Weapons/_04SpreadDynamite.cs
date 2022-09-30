using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _04SpreadDynamite : WeaponMovements
{
    [Header("エフェクト")]
    public GameObject explosion;
    // Update is called once per frame
    void Awake()
    {
        velocity = this.transform.forward * 25f;
        afterReflectVelo = velocity;
    }

    void OnDestroy() {
        EffectDamage expl = Instantiate(explosion, this.transform.position, Quaternion.identity).GetComponent<EffectDamage>();
        expl.player = player;
    }
}
