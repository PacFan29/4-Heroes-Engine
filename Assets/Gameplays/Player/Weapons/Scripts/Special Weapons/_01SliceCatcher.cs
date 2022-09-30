using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _01SliceCatcher : WeaponMovements
{
    private int step = 0;
    private Vector3 startPos;
    private float startTime;
    // Update is called once per frame
    void Awake()
    {
        startPos = this.transform.position;
        StartCoroutine("Movement");
    }

    void Update() {
        if (step == 0) {
            this.transform.position = Vector3.Lerp(this.transform.position, startPos + this.transform.forward * 20f, 7 * Time.deltaTime);
        } else {
            velocity = (player.gameObject.transform.position - this.transform.position).normalized * (Time.time - startTime) * 100f;

            if (Vector3.Distance(this.transform.position, player.gameObject.transform.position) <= 2f && this.transform.childCount < 2) {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator Movement() {
        yield return new WaitForSeconds(0.6f);

        startTime = Time.time;
        step = 1;
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Collectables" || col.gameObject.tag == "HealItem") {
            col.gameObject.transform.parent = this.transform;
            col.gameObject.transform.localPosition = Vector3.zero;
        }
    }
}
