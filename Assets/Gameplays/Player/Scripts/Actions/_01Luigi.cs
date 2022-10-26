using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _01Luigi : MarioActions
{
    [Header("効果音(限定)")]
    public AudioClip wallJump;

    Vector3 contactNormal;

    public void Update()
    {
        /*
        ルイージでしかできない技
        ・ハイジャンプ

        声の出演：Charles Martinet
        */
        continueJumping = true;
        if (info != null){
            //プレイヤーIDを1に設定
            info.setPlayerId(1);
        }
        switch (actionId) {
            case 4:
            //壁キック
            gravityControl = true;
            if (info.Grounded) {
                actionId = 0;
            }
            break;
            
            case 5:
            //ヒップドロップ
            if (info.Grounded) {
                if (info.ButtonsDown["A"]) {
                    //ヒップドロップジャンプ
                    info.SoundPlay(jumpBroad);
                    StopCoroutine("GroundPound");
                    info.YvelSetUp(60f);

                    info.axisInput = true;
                    groundPound = false;
                    gravityControl = false;
                    actionId = 0;
                    canSlideOnWall = true;
                }
            } else {
                if (!info.activePhysics && info.ButtonsDown["X"]) {
                    //ボディアタック
                    info.SoundPlay(jumpBroad);
                    info.activePhysics = true;
                    StopCoroutine("GroundPound");
                    info.ForwardSetUp(info.skin.forward, 40f);
                    info.YvelSetUp(10f);

                    groundPound = false;
                    actionId = 6;
                }
            }
            break;

            case 6:
            canSlideOnWall = false;
            gravityControl = false;
            if (info.Grounded) {
                canSlideOnWall = true;
                info.axisInput = true;
                actionId = 0;
            }
            break;
        }

        if (info.canWallJump && info.finalVelocity.y < 0) {
            info.constantChange(false, "terminal", -30f);

            Vector3 pos = transform.position - contactNormal;
            Instantiate(info.dustTrail, pos, transform.rotation);

            if (info.ButtonsDown["A"]) {
                //壁キック
                info.SoundPlay(wallJump);
                this.transform.position += contactNormal;
                info.ForwardSetUp(contactNormal, 15f);
                info.YvelSetUp(info.JumpForce);
                info.canWallJump = false;

                voices.Jump();
            }
        } else {
            info.constantChange(false, "terminal", info.TerminalVelocity);
        }

        if (!info.Grounded && (info.GetCrouchButtonDown("LB") || info.GetCrouchButtonDown("RB")) && !groundPound && (actionId == 0 || (actionId == 4 && !info.canWallJump))) {
            groundPound = true;
            StartCoroutine("GroundPound");
        }

        //共通アクションの実行
        actions();
    }

    /* 壁キック前の判定 */
    void OnCollisionStay (Collision hit)
    {
        if (info.isGroundLayerC(hit)) {
            ContactPoint contact = hit.contacts[0];
            if (!info.Grounded && contact.normal.y < 0.1f && !info.canWallJump && !info.underwater && canSlideOnWall && info.finalVelocity.y < 0) {
                Debug.Log(Vector3.Angle(info.skin.forward, contact.normal));
                if (Vector3.Angle(info.skin.forward, contact.normal) >= 135f) {
                    if (!info.axisInput) info.axisInput = true;
                    if (actionId != 4) info.ForwardSetUp(Vector3.zero, 0f);
                    actionId = 4;
                    Debug.DrawRay(contact.point, contact.normal, Color.red, 1.25f);
                    contactNormal = contact.normal;
                    info.canWallJump = true;
                }
            }
        }
    }
    void OnCollisionExit(Collision hit) {
        if (info.isGroundLayerC(hit) && info.canWallJump && info.finalVelocity.y < 0) {
            canSlideOnWall = true;
            actionId = 0;
            info.canWallJump = false;
        }
    }
}
