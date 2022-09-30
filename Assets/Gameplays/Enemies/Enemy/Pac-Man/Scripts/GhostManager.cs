using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    [Header("ステータス")]
    public float HP = 5;
    private int Score = 400;
    private bool attackedByPlayer;
    protected bool active = true;
    private AudioSource sound;
    protected PlayerInfo player;
    private int combo = 0;
    private int NormalAttack = 0;
    [Header("効果音")]
    public AudioClip damageSound;
    public AudioClip eaten;
    public AudioClip eyes;
    [Header("スキン")]
    public GameObject skin;
    [Header("エフェクト")]
    public GameObject effect;

    void Awake()
    {
        sound = GetComponent<AudioSource>();

        //colliderRadius = GetComponent<CapsuleCollider>().radius;

        //rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision col){
        if (col.gameObject.tag == "Player" && HP > 0 && active){
            player = col.gameObject.GetComponent<PlayerInfo>();
            if (player.invincible) {
                DamageManager(player);
            } else {
                player.TakeDamage(5, this.transform.position);
            }
        }
    }

    void DamageManager(PlayerInfo player){
        if (player.invincible) {
            TakeDamage(true, player, 99, 0, false, null);
        }
    }

    public void TakeDamage(bool atkByPl, PlayerInfo playerObj, float damage, int normalAtk, bool stomped, ComboManager comboM){
        if (active) {
            attackedByPlayer = atkByPl;
            HP -= damage;
            if (HP <= 0) {
                SoundPlay(eaten);
            } else {
                SoundPlay(damageSound);
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
                StartCoroutine(DefeatedAnimation());
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
                    if (combo >= 3) {
                        pts += 2500;
                    }
                    break;

                    default:
                    //通常攻撃、無敵
                    pts = Score * (int)Math.Pow(2, Math.Max(0, (combo-1)));
                    if (combo >= 5) {
                        pts += 1250;
                    }
                    break;
                }
                //弱点を突けたら得点2倍
                player.scorePopUp(pts, false, this.transform.position);

                if (player.rateTime > 0) player.rateTime += 0.5f;
            }
        }
    }

    public IEnumerator DefeatedAnimation() {
        active = false;
        Time.timeScale = 0f;
        this.GetComponent<CapsuleCollider>().isTrigger = true;

        skin.SetActive(false);
        var efMain = Instantiate(effect, this.transform.position, Quaternion.Euler(-90, 0, 0)).GetComponent<ParticleSystem>().main;
        efMain.useUnscaledTime = true;

        foreach (Transform other in effect.transform) {
            var otherEff = other.gameObject.GetComponent<ParticleSystem>().main;
            otherEff.useUnscaledTime = true;
        }

        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;
        SoundPlay(eyes);

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    public void SoundPlay(AudioClip soundClip){
        if (soundClip != null) {
            //効果音の再生
            sound.PlayOneShot(soundClip);
        }
    }
}
