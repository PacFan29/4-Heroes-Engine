using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyManager : DimensionManager
{
    [Header("ステータス")]
    public float HP = 1;
    public int Score = 300;
    private bool attackedByPlayer;
    private int combo = 0;
    private int NormalAttack = 0;
    protected bool active = true;
    public GameObject skin;
    public LayerMask GroundLayer;
    public bool defended = false;
    public bool spiked = false;

    [Header("エフェクト")]
    public GameObject vanishEffect;
    protected PlayerInfo player;

    private AudioSource sound;
    float colliderRadius;
    //private Rigidbody rb;
    private RaycastHit hit;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public bool Grounded = true;
    public bool Flying = false;

    [Header("効果音")]
    public AudioClip[] stompedSound;
    public AudioClip[] damageSound;
    public AudioClip[] defeatedSound;

    [Header("アニメーション")]
    public Animator anim;
    [HideInInspector] public int defeatedCase = 0;
    [HideInInspector] public int actionId = 0;
    // Start is called before the first frame update
    void Awake()
    {
        if (HP <= 0) HP = 1;
        Score = Mathf.Clamp(Score, 1, 2500);
        sound = GetComponent<AudioSource>();

        colliderRadius = GetComponent<CapsuleCollider>().radius;

        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.GetComponent<InstaShield>() != null && active){
            player = col.gameObject.GetComponent<InstaShield>().player;
            DamageManager(player);
        }
    }
    void OnCollisionEnter(Collision col){
        if (col.gameObject.tag == "Player" && HP > 0 && active){
            player = col.gameObject.GetComponent<PlayerInfo>();
            if (PlayerAttacking(col.gameObject)) {
                DamageManager(player);
            }
        }
    }
    void OnCollisionStay(Collision col){
        if (col.gameObject.tag == "Player" && HP > 0 && active){
            player = col.gameObject.GetComponent<PlayerInfo>();
            if (!PlayerAttacking(col.gameObject)) {
                player.TakeDamage(5, this.transform.position);
            }
        }
    }

    void DamageManager(PlayerInfo player){
        if (player.invincible) {
            if (player.playerType == 3) player.Stomp();
            TakeDamage(true, player, 99, 0, false, null);
        } else if (player.rolling) {
            if (player.playerType != 0) player.Stomp();
            TakeDamage(true, player, 6, 0, Stomped(player.gameObject, false), null);
        } else if (Stomped(player.gameObject, true)) {
            player.Stomp();
            TakeDamage(true, player, 6, 0, true, null);
        } else if (player.attacking) {
            TakeDamage(true, player, 6, 0, Stomped(player.gameObject, true), null);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FreezePos(dimension);

        if (HP <= 0){
            active = false;
        }

        if (!Flying || !active) {
            Physics.BoxCast(transform.position, Vector3.one * 0.75f, -transform.up, out hit, Quaternion.identity, 2f, GroundLayer);
            if (Grounded) {
                if (hit.distance <= 0f) {
                    Grounded = false;
                } else {
                    transform.position += Vector3.up * (0.6f - hit.distance);
                }
            } else {
                velocity.y -= 1.875f;

                if (hit.distance > 0 && velocity.y < 0) {
                    Grounded = true;
                    velocity.y = 0f;
                }
            }
        }
        if (!isNear()) {
            velocity.x = 0f;
            velocity.z = 0f;
        }
        rb.velocity = velocity;

        if (this.transform.position.y <= -100f && active) {
            HP = 0;
            StartCoroutine("DestroyThisObject");
        }

        if (anim != null) AnimatorManager();

        // PlayerInfo[] players = FindObjectsOfType<PlayerInfo>();
        // PlayerInfo nearistPlayer = null;
        // float minDistance = Mathf.Infinity;

        // foreach (PlayerInfo player in players) {
        //     float distance = Vector3.Distance(player.gameObject.transform.position, transform.position);
        //     if (minDistance > distance) {
        //         nearistPlayer = player;
        //         minDistance = distance;
        //     }
        // }
        // if (nearistPlayer != null) {
        //     if (!PlayerAttacking(nearistPlayer.gameObject) && nearistPlayer.XZmag >= 45 && minDistance < 30) {
        //         Time.timeScale = (minDistance / 30f);
        //     } else {
        //         if (Time.timeScale < 1f) {
        //             Time.timeScale = 1f;
        //         }
        //     }
        // }
    }
    public void AnimatorManager() {
        Vector3 speed = new Vector3(velocity.x, 0, velocity.z);

        anim.SetInteger("actionId", actionId);
        anim.SetFloat("Speed", speed.magnitude);
        anim.SetBool("Grounded", true);
        anim.SetInteger("DefeatedCase", defeatedCase);
    }

    public bool isActive(){
        return active;
    }
    public bool isNear() {
        return Vector3.Distance(Camera.main.transform.position, this.transform.position) <= 128;
    }

    public void TakeDamage(bool atkByPl, PlayerInfo playerObj, float damage, int normalAtk, bool stomped, ComboManager comboM){
        if (active) {
            if (defended) {
                this.GetComponent<EnemyMovements>().DamageDefended();
                return;
            }
            attackedByPlayer = atkByPl;
            HP -= damage;
            if (stomped) {
                SoundPlay(stompedSound);
            } else {
                if (HP <= 0) {
                    SoundPlay(defeatedSound);
                } else {
                    SoundPlay(damageSound);
                }
            }
            
            if (attackedByPlayer) {
                player = playerObj;
                NormalAttack = normalAtk;

                if (HP <= 0) {
                    if (comboM != null) {
                        combo = comboM.ComboIncrease();
                    } else {
                        combo = player.ComboIncrease();
                    }
                } else {
                    if (comboM != null) {
                        combo = comboM.GetCombo();
                    } else {
                        combo = player.GetCombo();
                    }
                }
            }
            if (HP <= 0) {
                StartCoroutine(DefeatedAnimation(stomped));
            }

            if (attackedByPlayer && combo > 0){
                int pts;
                switch (NormalAttack) {
                    case 1:
                    //属性攻撃
                    pts = Score * Math.Max(1, combo);
                    break;

                    case 2:
                    //スペシャル攻撃
                    pts = Score * 2 * (int)Math.Pow(4, Math.Min(2, Math.Max(0, (combo-1))));
                    break;

                    default:
                    //通常攻撃、無敵
                    pts = Score * (int)Math.Pow(2, Math.Max(0, (combo-1)));
                    break;
                }
                //弱点を突けたら得点2倍
                player.scorePopUp(pts, false, this.transform.position);

                if (player.rateTime > 0) player.rateTime += 0.5f;
            }
        }
    }

    bool Stomped(GameObject col, bool stompable) {
        float playerY = col.transform.position.y - 2f + (colliderRadius * this.transform.localScale.y);
        float thisY = this.transform.position.y;

        if (stompable) {
            return player.Stompable && playerY >= thisY;
        }
        return playerY >= thisY;
    }

    public void SoundPlay(AudioClip[] soundClips){
        if (soundClips != null) {
            int soundIndex = UnityEngine.Random.Range(0, soundClips.Length);

            //効果音の再生
            sound.PlayOneShot(soundClips[soundIndex]);
        }
    }
    public virtual IEnumerator DefeatedAnimation(bool stomped) {
        yield return null;
    }

    public IEnumerator DestroyThisObject() {
        active = false;
        GetComponent<CapsuleCollider>().isTrigger = true;

        if (player != null && player.GetComponent<_16MSonic>() != null) {
            player.GetComponent<_16MSonic>().BoostIncrease(10, false);
        }

        vanishEffect.SetActive(true);
        vanishEffect.transform.SetParent(null);

        skin.SetActive(false);
        while (vanishEffect != null) {
            yield return null;
        }
        Destroy(gameObject);
    }

    bool PlayerAttacking(GameObject playerObj) {
        player = playerObj.GetComponent<PlayerInfo>();
        return (player.invincible || player.rolling || player.attacking || Stomped(playerObj, true) && !player.tookDamage);
    }
}
