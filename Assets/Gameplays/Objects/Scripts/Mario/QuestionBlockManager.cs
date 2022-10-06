using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType {
    None,
    Coin,
    TenCoins,
    RestoreHP,
    RestoreWP,
    Invincible,
    FireFlower,
    IceFlower,
    ThunderShroom,
    PoisonShroom,
    Extend,
    Shield,
    SpeedUp,
    FireShield,
    BubbleShield,
    ThunderShield
}

public class QuestionBlockManager : MonoBehaviour
{
    [Header("ブロックの状態")]
    public bool isEmpty;
    public bool isHidden;
    public bool isLong;
    private bool breaked = false;
    public Transform skinGroup;
    public GameObject fullBlock;
    public GameObject emptyBlock;
    public GameObject brickBreak;
    [Header("ブロックのタイプ")]
    public BlockType blockType;

    private int leftCoins = 10;
    [Header("コイン・アイテム")]
    public GameObject coin;
    public GameObject item;
    [Header("効果音")]
    public AudioClip upperPunched;
    public AudioClip headHit;
    public AudioClip breakedSound;
    private AudioSource source;

    private float whackedTime = 0f;
    private float time = 1f;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }
    void Awake() {
        if (isHidden) {
            this.GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        fullBlock.SetActive(!isEmpty && !isHidden && !breaked);
        emptyBlock.SetActive(isEmpty && !breaked);

        brickBreak.SetActive(breaked);

        if (whackedTime > 0f) {
            whackedTime -= Time.deltaTime;
            if (whackedTime < 0f) whackedTime = 0f;
        }
        float movement = (float)Math.Sin(((double)whackedTime / 0.2) * 180 * Math.PI / 180.0);
        skinGroup.localPosition = Vector3.up * 0.4f * movement;
        skinGroup.localScale = Vector3.one * (1f + 0.2f * movement);

        if (leftCoins < 10 && leftCoins > 1 && blockType == BlockType.TenCoins) {
            time -= Time.deltaTime;
            if (time <= 0) {
                leftCoins--;
                time = 1f;
            }
        }
    }

    public void BlockHit(PlayerInfo player, bool downWard) {
        if (!isEmpty) {
            if (blockType != BlockType.None && this.GetComponent<BrickManager>() != null || this.GetComponent<BrickManager>() == null) {
                source.PlayOneShot(upperPunched);
            }
            time = 1f;
            whackedTime = 0.2f;

            this.GetComponent<BoxCollider>().isTrigger = false;
            isHidden = false;
            switch (blockType) {
                case BlockType.Coin:
                SplitCoin(player, Vector3.zero);
                break;

                case BlockType.TenCoins:
                SplitCoin(player, Vector3.zero);
                leftCoins--;
                break;

                case BlockType.None:
                DestroyBrick(player);
                break;

                default:
                int index = (int)blockType - 3;
                if (index >= Enum.GetNames(typeof(ItemType)).Length) {
                    ;
                } else {
                    GameObject itemObj = Instantiate(item, this.transform.position + (Vector3.up * 1.375f), this.transform.rotation);
                    itemObj.GetComponent<ItemManager>().itemType = (ItemType)index;
                    itemObj.transform.Rotate(0, 180, 0);

                    itemObj.GetComponent<ItemManager>().Split();
                }
                break;
            }
                    
            if (blockType != BlockType.TenCoins || (blockType == BlockType.TenCoins && leftCoins <= 0)) {
                isEmpty = true;
            }

            if (isLong) {
                for (int i = 0; i <= 1; i++) {
                    Vector3 offset = this.transform.right * 2.75f * (i * 2 - 1);
                    SplitCoin(player, offset);
                }
            }
        }
    }

    void SplitCoin(PlayerInfo player, Vector3 offset) {
        CoinManager coinObj = Instantiate(coin, this.transform.position + offset, Quaternion.identity).GetComponent<CoinManager>();
        coinObj.targetPlayer = player.gameObject;

        coinObj.StartCoroutine(coinObj.SplitCoin());
    }

    public virtual void DestroyBrick(PlayerInfo player) {
        ;
    }

    public IEnumerator DestroyBlock(PlayerInfo player) {
        source.PlayOneShot(breakedSound);
        player.scoreIncrease(10);

        this.GetComponent<BoxCollider>().isTrigger = true;
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        this.gameObject.tag = "Other";
        
        breaked = true;

        yield return new WaitForSeconds(1.1f);

        Destroy(gameObject);
    }
}
