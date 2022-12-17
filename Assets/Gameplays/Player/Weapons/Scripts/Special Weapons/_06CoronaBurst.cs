using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _06CoronaBurst : WeaponMovements
{
    private Vector3 goalPos;
    public GameObject explosion;

    private void Awake() {
        StartCoroutine("LifeTime");
        goalPos = this.transform.position + this.transform.forward * 10f;

        weaponType = WeaponTypes.Fire;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, goalPos, 0.2f);
    }

    IEnumerator LifeTime() {
        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }

    public void OnDestroy() {
        EffectDamage expl = Instantiate(explosion, transform.position, Quaternion.identity).GetComponent<EffectDamage>();
        expl.player = player;
        expl.weaponType = weaponType;
    }
}
