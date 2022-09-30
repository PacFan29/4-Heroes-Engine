using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PacManActions : MonoBehaviour
{
    protected PlayerInfo info;
    protected bool canJump;
    protected bool inputA;
    private bool underwaterPrevious = false;

    [HideInInspector] public int actionId = 0;

    public AudioSource invulSound;
    // Start is called before the first frame update
    void Start()
    {
        info = GetComponent<PlayerInfo>();
        info.Stompable = false;
        info.playerType = 1;
        info.desacelerar = true;
    }

    // Update is called once per frame
    public void actions()
    {
        if (info.underwater) {
            info.rolling = false;

            float currentY = info.finalVelocity.y;
            
            if (info.Buttons["A"] && !info.Buttons["B"]) {
                info.YvelSetUp(currentY + info.U_AirAcc);
            } else if (!info.Buttons["A"] && info.Buttons["B"]) {
                info.YvelSetUp(currentY - info.U_AirAcc);
            } else {
                info.YvelSetUp(currentY + (Math.Min(Math.Abs(currentY), info.U_AirFrc) * -Math.Sign(currentY)));
            }

            if (currentY > 20) {
                info.YvelSetUp(20);
            } else if (currentY < -20) {
                info.YvelSetUp(-20);
            }
        } else {
            if (info.ButtonsDown["A"] && info.Grounded && !info.Crouching) {
                info.rolling = false;
                info.Crouching = false;
                inputA = true;
                info.Jump();
            }
            if (inputA && info.finalVelocity.y > 0){
                //ジャンプの勢い
                float jumpReleaseStrength = 0f;
                if (info.underwater){
                    //水中
                    jumpReleaseStrength = 10f;
                } else {
                    //通常
                    jumpReleaseStrength = 20f;
                }
                //Aを押し続けるとより高く跳べる。
                if (info.finalVelocity.y > jumpReleaseStrength && info.ButtonsUp["A"]){
                    //途中でAを離したとき
                    info.YvelSetUp(jumpReleaseStrength);
                    inputA = false;
                } else if (info.finalVelocity.y <= jumpReleaseStrength){
                    //高く跳びきったとき
                    inputA = false;
                }
            }
        }
        if (info.Grounded && (actionId == 1 || actionId == 2)) {
            actionId = 0;
        }

        if (info.underwater != underwaterPrevious && info.Buttons["A"]) {
            /*ドルフィンジャンプ */
            info.YvelSetUp(50);
        }
        underwaterPrevious = info.underwater;

        if (info.Crouching || info.rolling) {
            this.GetComponent<CapsuleCollider>().height = 2.4f;
        } else {
            this.GetComponent<CapsuleCollider>().height = 3.5f;
        }

        if (info.invincible && !invulSound.isPlaying) {
            invulSound.Play();
        } else if (!info.invincible && invulSound.isPlaying) {
            invulSound.Stop();
        }

        this.GetComponent<PlayerAnimManager>().actionId = actionId;
    }

    public IEnumerator DamageAnimation() {
        info.tookDamage = false;
        yield return null;
    }
    public IEnumerator DeathAnimation() {
        actionId = -2;
        yield return null;
    }

    public void Reset() {
        actionId = 0;
    }
}
