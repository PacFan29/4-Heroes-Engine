using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall : ComboManager
{
    public Transform skin;
    private Rigidbody rb;
    private Vector3 velocity;
    private Vector3 objNormalVector = Vector3.zero;
    private Vector3 afterReflectVelo = Vector3.zero;

    private int boundCount = 2;
    // Start is called before the first frame update
    void Start()
    {
        velocity = transform.forward * 25f;
        velocity.y = -12.5f;

        afterReflectVelo = velocity;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity.y -= 1.25f;
        if (velocity.y < -20f) {
            velocity.y = -20f;
        }
        rb.velocity = velocity;

        skin.forward = velocity.normalized;
    }

    void OnTriggerStay(Collider col) {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
            EnemyManager enemy = col.GetComponent<EnemyManager>();
            enemy.TakeDamage(true, player, 6, 1, false, this);
            Destroy(gameObject);
        } else if (LayerMask.LayerToName(col.gameObject.layer) == "Boss"){
            BossManager boss = col.GetComponent<BossManager>();
            boss.Damage(player, 4, false);
            Destroy(gameObject);
        } else if (LayerMask.LayerToName(col.gameObject.layer) == "Default") {
            //跳ね返る
            RaycastHit hit;
            Vector3 XZvel = new Vector3(velocity.x, 0f, velocity.z);

            bool groundHit = Physics.Raycast(transform.position + Vector3.up * 12f, -Vector3.up, out hit, Math.Abs(velocity.y) * 24f);
            bool wallHit = Physics.Raycast(transform.position - XZvel.normalized * 3f, XZvel.normalized, out hit, 6f);

            if (groundHit && velocity.y <= 0) {
                velocity.y = 20f;

                boundCount--;
                if (boundCount <= 0) {
                    Destroy(gameObject);
                }
            }
            if (wallHit) {
                objNormalVector = hit.normal;
                Vector3 reflectVec = Vector3.Reflect (afterReflectVelo, objNormalVector);
                velocity.x = reflectVec.x;
                velocity.z = reflectVec.z;
                // 計算した反射ベクトルを保存
                afterReflectVelo = velocity;
                afterReflectVelo.y = 0;
            }
        }
    }
}
