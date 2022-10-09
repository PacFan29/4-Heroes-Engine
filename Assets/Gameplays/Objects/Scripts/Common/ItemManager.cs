using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    RestoreHP,
    RestoreWP,
    Invincible,
    FireFlower,
    IceFlower,
    ThunderShroom,
    PoisonShroom,
    Extend
}

public class ItemManager : MonoBehaviour
{
    public GameData data;
    private Rigidbody rb;
    private Vector3 velocity;
    private bool Grounded = false;
    private Vector3 objNormalVector = Vector3.zero;
    private Vector3 afterReflectVelo = Vector3.zero;

    public ItemType itemType;
    public GameObject[] restoreHP = new GameObject[4];
    public GameObject restoreWP;
    public GameObject[] invincible = new GameObject[2];
    public GameObject[] powerUps = new GameObject[3];
    public GameObject poison;
    public GameObject[] extend = new GameObject[4];
    private int skinIndex;
    private bool floating = false;

    [HideInInspector] public bool stock = false;

    [Header("エフェクト")]
    public GameObject healEffect;
    public GameObject effect;
    public GameObject starEffect;

    void Start() {
        rb = this.GetComponent<Rigidbody>();

        afterReflectVelo = velocity;
    }

    public void Split() {
        this.GetComponent<AudioSource>().Play();

        velocity = this.transform.forward * 3;
        velocity.y = 8f;
        afterReflectVelo = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> meshes = new List<GameObject>();
        meshes = addItem(meshes, restoreHP);
        meshes = addItem(meshes, new GameObject[1]{restoreWP});
        meshes = addItem(meshes, invincible);
        meshes = addItem(meshes, powerUps);
        meshes = addItem(meshes, new GameObject[1]{poison});
        meshes = addItem(meshes, extend);

        foreach (var mesh in meshes) {
            if (mesh != null) {
                mesh.SetActive(false);
            }
        }

        skinIndex = 0;
        switch (itemType){
            case ItemType.RestoreHP:
            //スーパーキノコ（HP 8回復）
            restoreHP[0].SetActive(true);
            break;

            case ItemType.RestoreWP:
            //アミーボ 武器（武器エネルギー 8回復）
            restoreWP.SetActive(true);
            break;
                
            case ItemType.Invincible:
            //スーパースター（無敵）
            if ((int)data.character < 2) {
                invincible[(int)data.character].SetActive(true);
            }
            skinIndex = (int)data.character;
            break;
                
            case ItemType.FireFlower:
            //ファイアフラワー
            powerUps[0].SetActive(true);
            break;
                
            case ItemType.IceFlower:
            //アイスフラワー
            powerUps[1].SetActive(true);
            break;
                
            case ItemType.ThunderShroom:
            //サンダーキノコ
            powerUps[2].SetActive(true);
            break;
                
            case ItemType.PoisonShroom:
            //毒キノコ（8ダメージ）
            poison.SetActive(true);
            break;
                
            case ItemType.Extend:
            //1UPキノコ
            extend[0].SetActive(true);
            break;
        }

        if (skinIndex == 1) {
            floating = true;
        }
    }
    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player"){
            PlayerInfo player = col.GetComponent<PlayerInfo>();

            switch (itemType){
                case ItemType.RestoreHP:
                //スーパーキノコ（HP 8回復）
                MakeEffect((player.HP == player.maxHP) ? effect : healEffect);
                if (col.GetComponent<_16MSonic>() != null) {
                    col.GetComponent<_16MSonic>().BoostIncrease(50, false);
                } else {
                    player.GotShroom();
                    player.HPRestore(8);
                }
                break;

                case ItemType.RestoreWP:
                //アミーボ 武器（武器エネルギー 8回復）
                player.GotRestoreWP();
                break;
                
                case ItemType.Invincible:
                //スーパースター（無敵）
                if (skinIndex == 1) {
                    player.Invincible(7.5f);
                } else {
                    player.Invincible(10);
                }
                MakeEffect(effect);
                break;
                
                case ItemType.FireFlower:
                //ファイアフラワー
                player.PowerUpAnim(1);
                MakeEffect(effect);
                break;
                
                case ItemType.IceFlower:
                //アイスフラワー
                player.PowerUpAnim(2);
                MakeEffect(effect);
                break;
                
                case ItemType.ThunderShroom:
                //サンダーキノコ
                player.PowerUpAnim(3);
                MakeEffect(effect);
                break;
                
                case ItemType.PoisonShroom:
                //毒キノコ（8ダメージ）
                player.TakeDamage(8, this.transform.position);
                break;
                
                case ItemType.Extend:
                //1UPキノコ
                player.OneUp();
                MakeEffect(effect);
                break;
            }
            
            if (itemType != ItemType.PoisonShroom && !stock) {
                player.scoreIncrease(50);
            }
            
            Destroy(gameObject);
        }
    }

    void MakeEffect(GameObject ef) {
        Instantiate(ef, this.transform.position, Quaternion.identity);
    }

    void FixedUpdate() {
        if (!floating) {
            if (Grounded) {
                if (velocity.y > 0) {
                    Grounded = false;
                }
            } else {
                velocity.y -= 0.25f;
                if (velocity.y < -20f) {
                    velocity.y = -40f;
                }
            }
            rb.velocity = velocity;
        } else {
            rb.velocity = Vector3.zero;
        }
    }
    public void OnTriggerStay (Collider col) {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Default" && !floating) {
            //跳ね返る
            RaycastHit Ghit;
            RaycastHit Whit;
            Vector3 XZvel = new Vector3(velocity.x, 0f, velocity.z);

            bool groundHit = Physics.Raycast(transform.position + Vector3.up * 2.7f, -Vector3.up, out Ghit, Math.Abs(velocity.y) * 5.4f);
            bool wallHit = Physics.Raycast(transform.position - XZvel.normalized * 5f, XZvel.normalized, out Whit, 10f);

            if (groundHit && velocity.y <= 0) {
                transform.position += Vector3.up * ((5.4f - Ghit.distance) - 1.35f);
                Grounded = true;
                velocity = Vector3.zero;
            }
            if (wallHit) {
                objNormalVector = Whit.normal;
                Vector3 reflectVec = Vector3.Reflect (afterReflectVelo, objNormalVector);
                velocity.x = reflectVec.x;
                velocity.z = reflectVec.z;
                // 計算した反射ベクトルを保存
                afterReflectVelo = velocity;
                afterReflectVelo.y = 0;
            }
        }
    }

    List<GameObject> addItem(List<GameObject> list, GameObject[] meshes) {
        foreach (var item in meshes) {
            if (item != null) list.Add(item);
        }
        return list;
    }
}
