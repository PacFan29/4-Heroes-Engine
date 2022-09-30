using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaManBossManager : BossManager
{
    [Header("追加効果音")]
    public AudioClip defeatedSound;
    public AudioClip explosionSound;

    public override void Damage(PlayerInfo playerObj, int damage, bool stomped){
        player = playerObj;

        if (!invincible) {
            if (HP > 0 && damageTime <= 0f){
                damageTime = 1f;
                SoundPlay(fireDamageSound);
                VoicePlay(damageVoices);
                
                totalHp -= damage;
                HP = (int)Math.Ceiling((float)totalHp / (HitsPerHP * 4));

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
    
    public override IEnumerator DefeatedAnimation(){
        sound.PlayOneShot(defeatedSound);
        Time.timeScale = 0.5f;

        yield return new WaitForSeconds(0.875f);

        sound.PlayOneShot(explosionSound);
        Time.timeScale = 1f;

        yield return new WaitForSeconds(2.5f);
        
        OnDestroy();
        
        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }
}
