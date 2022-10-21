using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : ComboManager
{
    public Transform skin;
    private Rigidbody rb;
    private Vector3 velocity;
    private Vector3 objNormalVector = Vector3.zero;
    private Vector3 afterReflectVelo = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        velocity = transform.forward * 40f;
        velocity.y = -25f;

        afterReflectVelo = velocity;

        rb = GetComponent<Rigidbody>();
        StartCoroutine("LifeTime");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity.y -= 2.5f;
        if (velocity.y < -40f) {
            velocity.y = -40f;
        }
        rb.velocity = velocity;

        skin.forward = velocity.normalized;
    }

    IEnumerator LifeTime() {
        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }

    void OnTriggerStay(Collider col) {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
            if (col.GetComponent<EnemyManager>() != null) {
                EnemyManager enemy = col.GetComponent<EnemyManager>();
                enemy.TakeDamage(true, player, 6, 1, false, this);
                Destroy(gameObject);
            } else if (col.GetComponent<ShellManager>() != null) {
                col.GetComponent<ShellManager>().Shot(player);
                Destroy(gameObject);
            }
        } else if (LayerMask.LayerToName(col.gameObject.layer) == "Boss"){
            BossManager boss = col.GetComponent<BossManager>();
            boss.Damage(player, 4, false);
            Destroy(gameObject);
        } else if (col.gameObject.GetComponent<Box>() != null) {
            if (LayerMask.LayerToName(col.gameObject.layer) != "Ignore Raycast") {
                StartCoroutine(col.gameObject.GetComponent<Box>().DestroyBox(player));

                Destroy(gameObject);
            }
        } else if (LayerMask.LayerToName(col.gameObject.layer) == "Default") {
            //跳ね返る
            RaycastHit hit;
            Vector3 XZvel = new Vector3(velocity.x, 0f, velocity.z);

            bool groundHit = Physics.Raycast(transform.position + Vector3.up * 12f, -Vector3.up, out hit, Math.Abs(velocity.y) * 24f);
            bool wallHit = Physics.Raycast(transform.position - XZvel.normalized * 3f, XZvel.normalized, out hit, 6f);

            if (groundHit && velocity.y <= 0) {
                velocity.y = 25f;
                return;
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

    // void OnTriggerEnter(Collider other) {
    //     if (LayerMask.LayerToName(other.gameObject.layer) == "Default") {
    //         //跳ね返る
    //         Vector3 contact = other.ClosestPoint(this.transform.position);
    //         if (Vector3.Distance(contact, this.transform.position) < 0.1f) {
    //             float keepY = velocity.y;

    //             objNormalVector = contact;
    //             Vector3 reflectVec = Vector3.Reflect (afterReflectVelo, objNormalVector);
    //             velocity = reflectVec;
    //             velocity.y = keepY;
    //             afterReflectVelo = velocity;
    //         } else {
    //             if (velocity.y <= 0) {
    //                 velocity.y = 20f;
    //             }
    //         }
    //     }
    // }

    public void OnCollisionStay (Collision collision) {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemy") {
            EnemyManager enemy = collision.gameObject.GetComponent<EnemyManager>();
            enemy.TakeDamage(true, player, 6, 1, false, this);
            Destroy(gameObject);
        } else if (LayerMask.LayerToName(collision.gameObject.layer) == "Boss"){
            BossManager boss = collision.gameObject.GetComponent<BossManager>();
            boss.Damage(player, 2, false);
            Destroy(gameObject);
        }
    }
    // public void OnCollisionEnter (Collision collision) {
    //     if (LayerMask.LayerToName(collision.gameObject.layer) == "Default") {
    //         //跳ね返る
    //         if (collision.contacts[0].normal.y < 0.1f) {
    //             float keepY = velocity.y;

    //             objNormalVector = collision.contacts[0].normal;
    //             Vector3 reflectVec = Vector3.Reflect (afterReflectVelo, objNormalVector);
    //             velocity = reflectVec;
    //             velocity.y = keepY;
    //             afterReflectVelo = velocity;
    //         } else {
    //             if (velocity.y <= 0) {
    //                 velocity.y = 20f;
    //             }
    //         }
    //     }
    // }
}
