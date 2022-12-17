using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponMovements : ComboManager
{
    [Header("重力")]
    public float gravity = 0f;
    [Header("各状態")]
    public bool isAttacking; //攻撃状態か
    public bool canPenetrate = false; //敵にあたっても貫通するか
    public bool penetrateWall = false; //壁を貫通するか
    public bool reflectable = false; //跳ね返るか
    protected Rigidbody rb;
    protected Vector3 velocity;
    private Vector3 objNormalVector = Vector3.zero;
    protected Vector3 afterReflectVelo = Vector3.zero;

    protected WeaponTypes weaponType = WeaponTypes.None;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        afterReflectVelo = velocity;
    }

    void FixedUpdate() {
        velocity.y -= gravity;

        rb.velocity = velocity;
    }

    void OnTriggerStay(Collider col) {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
            if (isAttacking) {
                EnemyManager enemy = col.GetComponent<EnemyManager>();
                enemy.TakeDamage(true, player, 1, 1, false, this, weaponType);
            }
            if (!canPenetrate) Destroy(gameObject);
        } else if (LayerMask.LayerToName(col.gameObject.layer) == "Boss"){
            if (isAttacking) {
                BossManager boss = col.GetComponent<BossManager>();
                boss.Damage(player, 1, false);
            }
            if (!canPenetrate) Destroy(gameObject);
        } else if (LayerMask.LayerToName(col.gameObject.layer) == "Default") {
            //跳ね返る
            if (reflectable) {
                RaycastHit hit;
                Physics.Raycast(transform.position - velocity.normalized * 3f, velocity.normalized, out hit, 6f);

                objNormalVector = hit.normal;
                Vector3 reflectVec = Vector3.Reflect (afterReflectVelo, objNormalVector);
                velocity = reflectVec;
                // 計算した反射ベクトルを保存
                afterReflectVelo = velocity;
            } else if (!penetrateWall) {
                Destroy(gameObject);
            }
        }
    }
    public void VelocityChange(int index, float speed) {
        switch (index) {
            case 0:
            velocity.x = speed;
            break;
            
            case 1:
            velocity.y = speed;
            break;

            case 2:
            velocity.z = speed;
            break;
        }
    }
}
