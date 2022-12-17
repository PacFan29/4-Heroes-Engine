using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("枚数")]
    public int amounts = 1;
    [Header("データ")]
    public GameData data;
    [Header("その他")]
    public GameObject[] shapes = new GameObject[5];
    public AudioClip[] sounds = new AudioClip[5];
    public GameObject coinEffect;
    public GameObject boltEffect;
    public GameObject ringEffect;
    public GameObject crystalEffect;

    private Character character;
    [HideInInspector] public bool magnetised = false;
    [HideInInspector] public bool split;
    private Rigidbody rb;
    private Vector3 velocity;
    [HideInInspector] public GameObject targetPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (split) {
            this.transform.Rotate(0f, -30f, 0f, Space.Self);
        } else {
            this.transform.Rotate(0f, -3f, 0f, Space.Self);
        }

        if (magnetised && !split) {
            int sx = Math.Sign(targetPlayer.transform.position.x - this.transform.position.x);
            int sy = Math.Sign(targetPlayer.transform.position.y - this.transform.position.y);
            int sz = Math.Sign(targetPlayer.transform.position.z - this.transform.position.z);
            
            //check relative movement
            bool tx = (Math.Sign(velocity.x) == sx);
            bool ty = (Math.Sign(velocity.y) == sy);
            bool tz = (Math.Sign(velocity.z) == sz);
            
            //add to speed
            velocity.x += (getCoinAcc(tx) * sx);
            velocity.y += (getCoinAcc(ty) * sy);
            velocity.z += (getCoinAcc(tz) * sz);

            if (targetPlayer.GetComponent<_16MSonic>() == null && targetPlayer.GetComponent<PlayerInfo>().shieldActive != 4) {
                magnetised = false;
            }
        } else {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players) {
                if (
                    Vector3.Distance(player.transform.position, transform.position) <= 16 && 
                    (
                        (player.GetComponent<PlayerInfo>().shieldActive == 4 || 
                        (player.GetComponent<_16MSonic>() != null && player.GetComponent<_16MSonic>().boost)) && 
                        !split && amounts < 2
                    )
                ) {
                    targetPlayer = player;
                    magnetised = true;
                }
            }

            if (split) velocity.y -= 3.125f;
        }

        rb.velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        character = data.character;

        for (int i = 0; i < shapes.Length; i++) {
            shapes[i].SetActive(false);
        }
        switch (character) {
            case Character.Mario:
            //コイン
            shapes[0].SetActive(true);
            break;
            
            case Character.PacMan:
            //クッキー
            shapes[1].SetActive(true);
            break;
            
            case Character.RockMan:
            //ネジ
            shapes[2].SetActive(true);
            break;
            
            case Character.Sonic:
            //リング
            shapes[3].SetActive(true);
            break;

            case Character.Other:
            //クリスタル
            shapes[4].SetActive(true);
            break;
        }
    }
    float getCoinAcc (bool signFit) {
        if (signFit) {
            return 0.9375f;
        } else {
            return 3.75f;
        }
    }
    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player" && !split){
            CoinSound(col.gameObject);
            DestroyCoin();
        }
    }

    void CoinSound(GameObject col) {
        int soundIndex = 0;
        float soundVolume = 1f;

        PlayerInfo player = col.GetComponent<PlayerInfo>();

        switch (data.character) {
            case Character.PacMan:
            //クッキー
            player.GotCookie(amounts);
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
            player.GotRing(amounts);
            soundIndex = 3;
            soundVolume = 0.7f;
            break;

            case Character.Other:
            //クリスタル
            soundIndex = 4;
            soundVolume = 0.7f;
            break;
        }
        GameManager.Coins += amounts;
        player.scoreIncrease(20 * amounts);

        if (col.GetComponent<_16MSonic>() != null) {
            col.GetComponent<_16MSonic>().BoostIncrease(amounts, false);
        }

        AudioSource playerGotit = col.transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        playerGotit.clip = sounds[soundIndex];
        playerGotit.volume = soundVolume;
        playerGotit.Play();
    }

    void DestroyCoin() {
        switch (character) {
            case Character.Mario:
            //コイン
            Instantiate(coinEffect, this.transform.position, Quaternion.identity);
            break;

            case Character.RockMan:
            //ネジ
            Instantiate(boltEffect, this.transform.position, Quaternion.identity);
            break;
                
            case Character.Sonic:
            //リング
            Instantiate(ringEffect, this.transform.position, Quaternion.identity);
            break;

            case Character.Other:
            //クリスタル
            Instantiate(crystalEffect, this.transform.position, Quaternion.identity);
            break;
        }
            
        Destroy(gameObject);
    }

    public IEnumerator SplitCoin() {
        split = true;
        velocity.y = 50f;
        CoinSound(targetPlayer);

        yield return new WaitForSeconds(0.5f);

        DestroyCoin();
    }
}
