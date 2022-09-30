using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _05PacMan : PacManActions
{
    private Dictionary<string, bool> pacActions = new Dictionary<string, bool>() {
        {"bounce", false},
        {"dashReady", false}
    };
    private bool canKick = false;
    private int bounceCount;
    private float dashTime;
    private AudioSource source;
    [Header("エフェクト")]
    public GameObject rollingEffect;
    public GameObject bounceEffect;
    public GameObject flipKickEffect;
    [Header("効果音")]
    public AudioClip bounceSound;
    public AudioClip flipkickSound;
    public AudioClip dashReadySound;
    public AudioClip dashReleaseSound;
    [Header("効果音(メタル状態)")]
    public AudioClip bounceSoundMetal;
    public AudioClip flipkickSoundMetal;
    public void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        パックマンでしかできない技
        ・パックダッシュ
        */
        if (info != null){
            //プレイヤーIDを5に設定
            info.setPlayerId(5);
        }
        if (info.Grounded) {
            canKick = true;
        } else {
            if (info.ButtonsDown["X"] && !info.rolling && canKick) {
                canKick = false;
                if (info.metal) {
                    info.SoundPlay(flipkickSoundMetal);
                } else {
                    info.SoundPlay(flipkickSound);
                }
                StartCoroutine("FlipKick");
            }
        }
        
        if (info.underwater) {
            dashTime = 0f;
            bounceCount = 0;
            pacActions["bounce"] = false;
            pacActions["dashReady"] = false;

            info.axisInput = true;
        } else {
            /* ヒップアタック */
            if (info.axisInput && info.ButtonsDown["A"] && !inputA && !info.groundEvent && !info.rolling && dashTime <= 0) {
                //急降下
                actionId = 0;
                info.attacked = false;
                info.groundAttack = true;
                
                if (info.finalVelocity.y > 0) info.YvelSetUp(0f);
                info.rolling = true;
                info.constantChange(false, "grv", info.Gravity * 5f);

                pacActions["bounce"] = true;
            } else if (pacActions["bounce"] && (info.Grounded || info.attacked)) {
                //バウンド
                if (!info.attacked) info.ComboReset();
                info.attacked = false;
                info.groundAttack = false;

                if (bounceCount < 3) bounceCount++;
                info.constantSetUp();
                if (info.metal) {
                    info.SoundPlay(bounceSoundMetal);
                } else {
                    info.SoundPlay(bounceSound);
                }
                info.rolling = false;
                info.YvelSetUp(info.JumpForce * (1.2f - ((float)(bounceCount - 1) / 40)));

                pacActions["bounce"] = false;
                actionId = bounceCount == 3 ? 2 : 1;

                RaycastHit hit;
                Physics.BoxCast(transform.position, Vector3.one * 0.5f, -Vector3.up, out hit, Quaternion.identity, 1.85f, info.GroundLayer);
                Vector3 pos = transform.position - (transform.up * 1.35f);
                Instantiate(bounceEffect, pos, Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(-90, 0, 0));
            } else if (!pacActions["bounce"] && info.Grounded) {
                bounceCount = 0;
            }

            /* パックダッシュ */
            if (info.ButtonsDown["X"] && dashTime <= 0 && info.Grounded) {
                //パックダッシュ開始
                info.Crouching = true;
                actionId = 3;

                canJump = false;
                info.StopAllSounds();

                source = GetComponent<AudioSource>();
                source.clip = dashReadySound;
                source.pitch = 0f;
                source.loop = true;
                source.Play();

                dashTime += Time.deltaTime;
                pacActions["dashReady"] = true;
            } else if (pacActions["dashReady"] && dashTime < 1f && info.Buttons["X"]) {
                //チャージ（効果音のピッチ：0～1.9）
                dashTime += Time.deltaTime;
                if (dashTime > 1f) dashTime = 1f;
                source.pitch = Math.Min(1.9f, 3.8f * dashTime);
            } else if (!info.Buttons["X"] && pacActions["dashReady"]) {
                info.Crouching = false;
                source.clip = null;
                source.pitch = 1f;
                source.loop = false;
                source.Stop();

                pacActions["dashReady"] = false;
                
                if (dashTime >= 0.25f) {
                    //突進
                    actionId = 0;
                    info.axisInput = false;
                    info.rolling = true;
                    info.constantChange(false, "airfrc", 0f);
                    info.ForwardSetUp(Vector3.zero, 55f);
                    info.SoundPlay(dashReleaseSound);
                    dashTime -= Time.deltaTime;

                    info.constantChange(true, "slprollup", 0f);
                } else {
                    //キャンセル
                    actionId = 0;
                    dashTime = 0f;
                }
            } else if (!pacActions["dashReady"] && dashTime > 0f) {
                //ダッシュ中の時間減少
                if (info.Grounded) dashTime -= Time.deltaTime;

                if (dashTime < 0) {
                    dashTime = 0f;
                    info.constantChange(true, "slprollup", info.SlopeRollUp);
                }
            }
            if (inputA && (dashTime > 0 || info.rolling) && !info.isNarrow()) {
                //パックダッシュ中にジャンプ（キャンセル）
                info.axisInput = true;
                info.rolling = false;
                dashTime = 0f;
            }

            if (dashTime <= 0f && Vector3.Angle(Vector3.up, info.GroundNormal) <= 15 && info.Grounded && !info.isNarrow()) {
                if (info.rolling) info.constantSetUp();
                info.axisInput = true;
                info.rolling = false;
            } else if (Vector3.Angle(Vector3.up, info.GroundNormal) >= 40 && info.Grounded) {
                if (!info.rolling) info.constantChange(false, "airfrc", 0f);
                info.axisInput = false;
                info.rolling = true;
            }

            if (!info.axisInput && !info.Grounded && !info.rolling) {
                info.axisInput = true;
            }

            if (!pacActions["bounce"] && info.rolling && info.attacked) {
                info.VelocitySetUp(info.finalVelocity * -1);
                info.attacked = false;
            }

            info.groundEvent = (/*info.rolling && info.XZmag > 0 || */ pacActions["bounce"]);

            if (info.rolling) {
                Instantiate(rollingEffect, transform.position, Quaternion.identity);
            }
        }
    }

    IEnumerator FlipKick() {
        actionId = 4;
        flipKickEffect.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        flipKickEffect.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        actionId = 0;
    }
}
