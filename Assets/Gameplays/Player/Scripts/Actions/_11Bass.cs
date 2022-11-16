using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _11Bass : MegaManActions
{
    public GameObject buster;

    private bool sliding;
    private int doubleJumpTrigger = 0;

    [Header("効果音")]
    public AudioClip busterSound;
    public AudioClip dashSound;
    public AudioClip fireSound;
    public AudioClip iceSound;
    public AudioClip thunderSound;

    void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        フォルテでしかできない技
        ・バスター連射
        ・ダッシュ
        ・ダブルジャンプ
        */
        if (info != null){
            //プレイヤーIDを11に設定
            info.setPlayerId(11);
        }

        if (info.ButtonsDown["X"] && (weaponId == 0 || weaponId >= 9)){
            info.ComboReset();
            info.Crouching = true;
            StartCoroutine("ForteShoot");
        } else if (info.ButtonsUp["X"] && (weaponId == 0 || weaponId >= 9)) {
            info.Crouching = false;
            StopCoroutine("ForteShoot");
        }

        if (doubleJumpTrigger == 1) {
            if (info.ButtonsDown["A"] && canJump) {
                info.constantSetUp();
                info.Crouching = false;
                inputA = true;
                doubleJumpTrigger = 2;
                info.Jump();
            }
        } else if (doubleJumpTrigger == 0) {
            if (!info.Grounded && !inputA) {
                doubleJumpTrigger = 1;
            }
        }
        if (info.Grounded && doubleJumpTrigger != 0) {
            doubleJumpTrigger = 0;
        }

        canJump = !info.GetCrouchButton("RB");

        if (!canJump && info.ButtonsDown["A"] && !sliding && info.Grounded) {
            //info.axisInput = false;
            info.SoundPlay(dashSound);
            StartCoroutine("Dash");
        }
        if (sliding) {
            info.dustTrailEffect();

            Vector3 XZvel = new Vector3(info.finalVelocity.x, 0, info.finalVelocity.z);
            info.skin.transform.forward = XZvel.normalized;
            
            if (Vector3.Angle(XZvel, info.input) >= 130) {
                Debug.Log("Canceled");
                info.ForwardSetUp(-XZvel.normalized, 5f);
                sliding = false;
                info.constantSetUp();
                info.ForwardSetUp(Vector3.zero, 0);
                info.axisInput = true;

                StopCoroutine("Dash");
            }
            if (!info.Grounded) {
                Debug.Log("Jumped");
                info.axisInput = true;
                info.Crouching = false;
                sliding = false;
                StopCoroutine("Dash");
                info.constantSetUp();
                if (info.input != Vector3.zero) info.ForwardSetUp(Vector3.zero, info.TopSpeed);
            }
        }
    }

    IEnumerator ForteShoot() {
        while (info.Buttons["X"] && (weaponId == 0 || weaponId >= 9)) {
            info.SoundPlay(busterSound);

            GameObject solarbrit = Instantiate(buster, transform.position, info.skin.rotation);
            MegaBuster bust = solarbrit.GetComponent<MegaBuster>();
            bust.player = GetComponent<PlayerInfo>();
            bust.level = 0;
            bust.power = 0.5f;
            bust.powerUp = info.powerUpActive;

            yield return new WaitForSeconds(0.075f);
        }
        info.Crouching = false;
    }

    IEnumerator Dash() {
        sliding = true;
        info.ForwardSetUp(Vector3.zero, 40f);
        info.constantChange(true, "frc", 0);
        info.constantChange(true, "top", 40f);

        yield return new WaitForSeconds(0.4342f);

        sliding = false;
        info.constantSetUp();
        info.ForwardSetUp(Vector3.zero, 0);
        info.axisInput = true;
    }
}
