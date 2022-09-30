using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{   
    [Header("データ")]
    public GameData data;
    [Header("HP")]
    public int HP = 3;
    private int latestHP = 3;
    public int HitsPerHP = 5;
    //private int hits;
    [HideInInspector] public int totalHp;
    [HideInInspector] public int maxHp;
    protected AudioSource sound;
    protected PlayerInfo player;
    protected int actionId = 0;

    [Header("アニメーション")]
    public Animator animator;
    [Header("段階ごとのHP")]
    public int[] PhasePerHP;
    [HideInInspector] public int phase = 0;
    [Header("ボスゲージ表示")]
    public bool showBossHP;
    [Header("ステージクリア")]
    public bool stageClear;

    float colliderRadius;
    protected float damageTime;
    private int damageTrigger = 0;
    protected bool invincible = false;
    [Header("効果音")]
    public AudioClip stompedSound;
    public AudioClip[] fireDamageSound;
    public AudioClip blowDamageSound;
    [Header("ボイス")]
    public AudioClip[] damageVoices;
    // Start is called before the first frame update
    void Awake()
    {
        //HPの設定
        if (GameManager.extra) {
            if (HP < 4) {
                HP++;
            } else {
                HP = (int)((float)HP * 1.25f);
            }
        }
        totalHp = HitsPerHP * 4 * HP;
        maxHp = totalHp;

        latestHP = HP;

        //効果音
        sound = GetComponent<AudioSource>();
        //攻撃開始
        StartCoroutine("Attack");
        //当たり判定
        colliderRadius = GetComponent<SphereCollider>().radius;

        //段階
        Array.Sort(PhasePerHP);
        phase = PhasePerHP.Length;

        if (showBossHP) {
            GameManager.bossInfo = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showBossHP) {
            if (!GameManager.bossHPShow && HP > 0) GameManager.bossHPShow = true;
        }

        if (damageTime > 0) {
            damageTime -= Time.deltaTime;
            if (damageTime <= 0) {
                damageTrigger = 0;
            }
        }

        if (animator != null) {
            animator.SetInteger("Actions", actionId);
            animator.SetBool("IsAlive", HP > 0);
            animator.SetInteger("DamageTrigger", damageTrigger);
        }
    }

    void OnTriggerStay(Collider col){
        if (HP > 0 && damageTime <= 0){
            int singleDamage = HitsPerHP * 4;
            
            if (col.gameObject.GetComponent<InstaShield>() != null) {
                //ダブル回転アタックの攻撃が当たった場合
                player = col.GetComponent<InstaShield>().player;
                player.BigStomp();
                Damage(player, singleDamage, false);
            }
        } 
    }
    void OnCollisionStay(Collision col){
        if (HP > 0 && damageTime <= 0){
            int singleDamage = HitsPerHP * 4;
            
            if (col.gameObject.tag == "Player") {
                player = col.gameObject.GetComponent<PlayerInfo>();
                if (player.invincible) {
                    if (player.playerType == 3) player.BigStomp();
                    Damage(player, singleDamage, false);
                } else if (player.rolling) {
                    if (player.playerType != 0) player.BigStomp();
                    Damage(player, singleDamage, Stomped(col));
                } else if (Stomped(col)) {
                    player.BigStomp();
                    Damage(player, singleDamage, true);
                } else if (player.attacking) {
                    Damage(player, singleDamage, Stomped(col));
                } else {
                    player.TakeDamage(6, this.transform.position);
                }
            }
        } 
    }
    public virtual void Damage(PlayerInfo playerObj, int damage, bool stomped){
        player = playerObj;

        if (!invincible) {
            if (HP > 0){
                if (stomped) {
                    sound.PlayOneShot(stompedSound);
                    damageTrigger = 1;
                    damageTime = 1f;

                    VoicePlay(damageVoices);
                } else {
                    SoundPlay(fireDamageSound);
                }
                
                totalHp -= damage;
                HP = (int)Math.Ceiling((float)totalHp / (HitsPerHP * 4));

                if (latestHP > HP) {
                    latestHP = HP;
                    if (!stomped) {
                        damageTime = 1f;
                        sound.PlayOneShot(blowDamageSound);
                        damageTrigger = 2;
                    }

                    StartCoroutine("DamageAnimation");
                }

                if (phase > 0) {
                    if (HP <= PhasePerHP[phase - 1]) {
                        phase--;
                        StartCoroutine("PhaseChange");
                    }
                }

                if (totalHp <= 0) {
                    if (stageClear) GameManager.timeIncrease = false;
                    
                    player.scorePopUp(5000, false, this.transform.position);
                    StopCoroutine("Attack");
                    StartCoroutine("DefeatedAnimation");
                }
            }
        }

        if (showBossHP) {
            GameManager.bossInfo = this;
        }
    }

    public virtual IEnumerator Attack()
    {
        yield return null;
    }
    public virtual IEnumerator PhaseChange() {
        yield return null;
    }
    public virtual IEnumerator DamageAnimation(){
        yield return null;
    }
    public virtual IEnumerator DefeatedAnimation(){
        yield return null;
    }
    public void VoicePlay(AudioClip[] voiceClips) {
        if (voiceClips.Length > 0) {
            int voiceIndex = UnityEngine.Random.Range(0, voiceClips.Length);

            //効果音の再生
            sound.clip = voiceClips[voiceIndex];
            sound.Play();
        }
    }

    protected void OnDestroy() {
        GameManager.bossHPShow = false;

        if (stageClear) {
            data.ResetCheckPoint();
            SceneManager.LoadScene("ResultScreen");
        }
    }

    bool Stomped(Collision col) {
        //float playerY = col.transform.position.y - 2f + (colliderRadius * this.transform.localScale.y);
        float playerY = col.transform.position.y;
        float thisY = this.transform.position.y;

        return player.Stompable && playerY >= thisY;
    }

    public void SoundPlay(AudioClip[] soundClips){
        if (soundClips != null) {
            int soundIndex = UnityEngine.Random.Range(0, soundClips.Length);

            //効果音の再生
            sound.PlayOneShot(soundClips[soundIndex]);
        }
    }
}
