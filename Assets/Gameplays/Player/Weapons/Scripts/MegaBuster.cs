using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBuster : ComboManager
{
    public int level = 0;
    public float power = 1f;
    public bool stronger;
    private Rigidbody rb;
    public GameObject hitEffect;
    [HideInInspector] public int powerUp = 0;

    [Header("スキン")]
    public GameObject[] normalBullets = new GameObject[3];
    public GameObject[] fireBullets = new GameObject[3];
    public GameObject[] iceBullets = new GameObject[3];
    public GameObject[] thunderBullets = new GameObject[3];
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine("LifeTime");
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = this.transform.forward * 50;

        //スキン、サイズ
        foreach (GameObject obj in normalBullets) {
            if (obj != null) {
                obj.SetActive(false);
            }
        }
        foreach (GameObject obj in fireBullets) {
            if (obj != null) {
                obj.SetActive(false);
            }
        }
        foreach (GameObject obj in iceBullets) {
            if (obj != null) {
                obj.SetActive(false);
            }
        }
        foreach (GameObject obj in thunderBullets) {
            if (obj != null) {
                obj.SetActive(false);
            }
        }
        
        switch (level) {
            case 2:
            //チャージショット(攻撃力 3)
            power = 3f;
            this.transform.localScale = Vector3.one * 2.5f;
            break;

            case 3:
            //ブルースストライク(攻撃力 5)
            power = 5f;
            this.transform.localScale = Vector3.one * 3.2f;
            break;

            default:
            this.transform.localScale = Vector3.one;
            break;
        }
        if (level == 3) {
            switch (powerUp) {
                case 0:
                break;

                case 1:
                fireBullets[2].SetActive(true);
                break;

                case 2:
                iceBullets[2].SetActive(true);
                break;

                case 3:
                thunderBullets[2].SetActive(true);
                break;
            }
        } else {
            switch (powerUp) {
                case 0:
                normalBullets[level].SetActive(true);
                break;
                
                case 1:
                fireBullets[level].SetActive(true);
                break;

                case 2:
                iceBullets[level].SetActive(true);
                break;

                case 3:
                thunderBullets[level].SetActive(true);
                break;
            }
        }

        //追従
        bool looking;
        GameObject hTargetObj = null;
        GameObject[] homingTargets = GameObject.FindGameObjectsWithTag("HomingTarget");
        float enDistance;
        float MaxHDistance = 20;
        bool homing = false;;
        
        float minDis = Mathf.Infinity;
        for (int i = 0; i < homingTargets.Length; i++){
            //ターゲットの対象が空でないかつ、正常に動いていれば認識できる。
            looking = Vector3.Angle(this.transform.forward, (homingTargets[i].transform.position - this.transform.position)) <= 15;

            if (
                homingTargets[i] != null && 
                (
                    (homingTargets[i].GetComponent<EnemyManager>() != null && homingTargets[i].GetComponent<EnemyManager>().isActive()) ||
                    (homingTargets[i].GetComponent<BossManager>() != null && homingTargets[i].GetComponent<BossManager>().HP > 0) || 
                    (homingTargets[i].GetComponent<EnemyManager>() == null && homingTargets[i].GetComponent<BossManager>() == null)
                )
                && looking
            ){
                //プレイヤーから敵までの距離
                enDistance = Vector3.Distance(homingTargets[i].transform.position, transform.position);
                if (enDistance <= MaxHDistance && enDistance < minDis){
                    //最短距離でターゲットを認識する
                    minDis = enDistance;
                    hTargetObj = homingTargets[i];
                    homing = true;
                }
            }
        }

        if (homing) {
            Vector3 distance = hTargetObj.transform.position - this.transform.position;
            distance.y = 0;

            this.transform.forward = Vector3.Lerp(this.transform.forward, distance.normalized, 0.25f);
        }
    }
    void OnTriggerEnter(Collider col){
        if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy"){
            if (col.GetComponent<EnemyManager>() != null) {
                EnemyManager enemy = col.GetComponent<EnemyManager>();
                enemy.TakeDamage(true, player, power, 0, false, this);
                if (power <= 1 || enemy.HP > 0 && level > 0) {
                    Destroy(gameObject);
                }
            } else if (col.GetComponent<ShellManager>() != null) {
                col.GetComponent<ShellManager>().Shot(player);
                if (power <= 1) {
                    Destroy(gameObject);
                }
            }
        } else if (LayerMask.LayerToName(col.gameObject.layer) == "Boss"){
            BossManager boss = col.GetComponent<BossManager>();
            boss.Damage(player, (int)(power * 2), false);
            if (power <= 1 || boss.HP > 0 && level > 0) {
                Destroy(gameObject);
            }
        } else if (col.gameObject.GetComponent<MonitorManager>() != null) {
            if (LayerMask.LayerToName(col.gameObject.layer) != "Ignore Raycast") {
                MonitorManager monitor = col.gameObject.GetComponent<MonitorManager>();
                monitor.player = player;
                monitor.destroyed = true;

                if (level <= 1) {
                    Destroy(gameObject);
                }
            }
        } else if (col.gameObject.GetComponent<QuestionBlockManager>() != null) {
            if (LayerMask.LayerToName(col.gameObject.layer) != "Ignore Raycast") {
                col.gameObject.GetComponent<QuestionBlockManager>().BlockHit(player, false);

                if (level <= 1) {
                    Destroy(gameObject);
                }
            }
        } else if (LayerMask.LayerToName(col.gameObject.layer) == "Default") {
            if (power < 1) {
                Destroy(gameObject);
            }
        }
    }
    IEnumerator LifeTime(){
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void OnDestroy() {
        Instantiate(hitEffect, this.transform.position, this.transform.rotation);
    }
}
