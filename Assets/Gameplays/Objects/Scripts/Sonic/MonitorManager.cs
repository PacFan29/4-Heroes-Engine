using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonitorType {
    TenRings,
    SpeedUp,
    Invincible,
    Shield,
    FireShield,
    BubbleShield,
    ThunderShield,
    Eggman,
    Extend
}

public class MonitorManager : MonoBehaviour
{
    [Header("データ")]
    public GameData data;
    [Header("モニターのタイプ")]
    public MonitorType monitorType;
    [HideInInspector] public PlayerInfo player;

    [HideInInspector] public bool destroyed;
    private bool isDestroyed;
    private GameObject playerCol;

    [Header("エフェクト等")]
    public GameObject monitorSkin;
    public GameObject explosion;
    [Header("効果音")]
    public AudioClip[] coinSounds = new AudioClip[5];
    [Header("画面")]
    public MeshRenderer screen;
    public Material[] screenMaterials;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyed && !isDestroyed) {
            this.GetComponent<AudioSource>().Play();

            if (player != null) {
                player.scoreIncrease(50);
            }
            isDestroyed = true;

            monitorSkin.SetActive(false);
            explosion.SetActive(true);

            this.GetComponent<BoxCollider>().isTrigger = true;
            this.tag = "Untagged";
            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            StartCoroutine("MonitorEffect");
        }

        Material[] mat = new Material[1];
        mat[0] = screenMaterials[(int)monitorType];

        screen.materials = mat;
    }

    IEnumerator MonitorEffect() {
        yield return new WaitForSeconds(0.5f);

        switch (monitorType) {
            case MonitorType.TenRings:
            GameManager.Coins += 10;

            int soundIndex = 0;
            float soundVolume = 1f;
            switch (data.character) {
                case Character.PacMan:
                //クッキー
                player.GotCookie(10);
                soundIndex = 1;
                soundVolume = 0.35f;
                break;
                    
                case Character.RockMan:
                //ネジ
                soundIndex = 2;
                soundVolume = 1f;
                break;
                    
                case Character.Sonic:
                //リング
                player.GotRing(10);
                soundIndex = 3;
                soundVolume = 0.7f;
                break;

                case Character.Other:
                //クリスタル
                soundIndex = 4;
                soundVolume = 0.7f;
                break;
            }
            player.scoreIncrease(200);

            AudioSource playerGotit = player.gameObject.transform.GetChild(0).gameObject.GetComponent<AudioSource>();
            playerGotit.clip = coinSounds[soundIndex];
            playerGotit.volume = soundVolume;
            playerGotit.Play();
            break;

            case MonitorType.SpeedUp:
            player.SpeedUp();
            break;

            case MonitorType.Invincible:
            player.Invincible(20);
            break;

            case MonitorType.Shield:
            if (player.shieldActive == 1) {
                StartCoroutine(player.ItemStock(3, 1));
            } else if (player.shieldActive > 0) {
                StartCoroutine(player.ItemStock(3, player.shieldActive));
            }
            player.shieldActive = 1;
            break;
            
            case MonitorType.FireShield:
            if (player.shieldActive == 2) {
                StartCoroutine(player.ItemStock(3, 2));
            } else if (player.shieldActive > 0) {
                StartCoroutine(player.ItemStock(3, player.shieldActive));
            }
            player.shieldActive = 2;
            break;
            
            case MonitorType.BubbleShield:
            if (player.shieldActive == 3) {
                StartCoroutine(player.ItemStock(3, 3));
            } else if (player.shieldActive > 0) {
                StartCoroutine(player.ItemStock(3, player.shieldActive));
            }
            player.shieldActive = 3;
            break;
            
            case MonitorType.ThunderShield:
            if (player.shieldActive == 4) {
                StartCoroutine(player.ItemStock(3, 4));
            } else if (player.shieldActive > 0) {
                StartCoroutine(player.ItemStock(3, player.shieldActive));
            }
            player.shieldActive = 4;
            break;
            
            case MonitorType.Eggman:
            player.TakeDamage(8, this.transform.position);
            break;
            
            case MonitorType.Extend:
            player.OneUp();
            break;
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Player" && !destroyed){
            player = col.gameObject.GetComponent<PlayerInfo>();
            playerCol = col.gameObject;

            if (
                (player.rolling && player.Grounded) ||
                player.attacking
            ) {
                if (player.attacking) player.Stomp();
                destroyed = true;
            }
        }
    }
    void OnTriggerEnter(Collider col) {
        if (col.gameObject.name == "InstaShield" && !destroyed) {
            player = col.GetComponent<InstaShield>().player;
            player.Stomp();
            destroyed = true;
        }
    }
}
