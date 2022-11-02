using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCapsule : MonoBehaviour
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
    public MeshRenderer[] screens = new MeshRenderer[2];
    public Material[] screenMaterials;

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

            this.tag = "Untagged";
            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            StartCoroutine("MonitorEffect");
        }

        Material[] mat = new Material[1];
        mat[0] = screenMaterials[(int)monitorType];

        foreach (MeshRenderer screen in screens) {
            screen.materials = mat;
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player" && !destroyed){
            player = col.gameObject.GetComponent<PlayerInfo>();
            playerCol = col.gameObject;

            if (!player.Grounded) player.Stomp();
            destroyed = true;
        } else if (col.gameObject.name == "InstaShield" && !destroyed) {
            player = col.GetComponent<InstaShield>().player;
            player.Stomp();
            destroyed = true;
        }
    }

    IEnumerator MonitorEffect() {
        int soundIndex = 0;
        float soundVolume = 1f;
        AudioSource playerGotit;

        yield return new WaitForSeconds(0.5f);

        switch (monitorType) {
            case MonitorType.TenRings:
            GameManager.Coins += 10;

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

            playerGotit = player.gameObject.transform.GetChild(0).gameObject.GetComponent<AudioSource>();
            playerGotit.clip = coinSounds[soundIndex];
            playerGotit.volume = soundVolume;
            playerGotit.Play();
            break;

            case MonitorType.RandomRings:
            int[] randomAmounts = {1, 5, 10, 20, 30, 40, 50};
            int am = randomAmounts[UnityEngine.Random.Range(0, randomAmounts.Length)];
            GameManager.Coins += am;

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
            player.scoreIncrease(am * 20);

            playerGotit = player.gameObject.transform.GetChild(0).gameObject.GetComponent<AudioSource>();
            playerGotit.clip = coinSounds[soundIndex];
            playerGotit.volume = soundVolume;
            playerGotit.Play();
            break;

            case MonitorType.SpeedUp:
            player.SpeedUp();
            break;

            case MonitorType.Invincible:
            player.Invincible(20, 2);
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

            case MonitorType.BlueRing:
            if (player.gameObject.GetComponent<_16MSonic>() != null) {
                player.gameObject.GetComponent<_16MSonic>().ActiveBlueRing();
            }
            break;
            
            case MonitorType.Eggman:
            player.TakeDamage(8, this.transform.position);
            break;
            
            case MonitorType.Extend:
            player.OneUp();
            break;
        }

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
