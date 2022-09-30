using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _13Tails : SonicActions
{
    private float jumpTime;
    private bool sliding = false;
    private float flyingTime = 8f;
    private bool flying = false;
    [Header("効果音")]
    public AudioClip spinSound;
    public AudioClip slidingSound;
    public LoopingSoundManager lManager;

    // Update is called once per frame
    void Update()
    {
        //共通アクションの実行
        actions();

        if (jumped) {
            info.rolling = false;
            jumpTime = 0.1f;
            jumped = false;
        }
        if (jumpTime > 0) {
            jumpTime -= Time.deltaTime;
            if (jumpTime <= 0 && info.Buttons["A"] && !info.Grounded) {
                info.SoundPlay(spinSound);
                info.rolling = true;
            }
        }
        if (!info.Grounded && info.finalVelocity.y <= -25 && info.rolling && actionId == 0) {
            info.rolling = false;
        }

        //スライディング
        if ((info.GetCrouchButton("RB") || info.GetCrouchButton("B")) && info.Grounded && !sliding && !info.rolling && actionId != 5) {
            if (info.XZmag <= 0 && info.input != Vector3.zero) {
                info.ForwardSetUp(Vector3.zero, 15f);
                info.Crouching = false;
            }
            if (info.XZmag >= 2.5f) {
                info.constantChange(true, "rollfrc", info.RollFrc * 3);
                info.rolling = true;
                sliding = true;

                lManager.SetUp(slidingSound, 1f, 2.595f);
            }
        } else if (sliding && ((!(info.GetCrouchButton("RB") || info.GetCrouchButton("B")) && transform.up == Vector3.up) || !info.Grounded || info.Crouching)) {
            info.constantChange(true, "rollfrc", info.RollFrc);
            if (info.Grounded || (!info.Grounded && info.finalVelocity.y <= 0)) info.rolling = false;
            sliding = false;

            lManager.Stop();
        }

        //飛行
        if (flying) {
            if (flyingTime > 0) {
                flyingTime -= Time.deltaTime;

                if (info.ButtonsDown["A"]) {
                    info.YvelSetUp(info.finalVelocity.y + 2f);
                }
            }

            if (info.finalVelocity.y < -15f) {
                info.YvelSetUp(-15f);
            }

            if (info.Grounded) {
                jumpAction = -1;
                flying = false;
                info.constantChange(false, "grv", info.Gravity);
            }
        } else {
            flyingTime = 8f;

            if (jumpAction > 0 && info.ButtonsDown["A"]) {
                info.rolling = false;
                info.constantChange(false, "grv", 0.15625f);
                jumpAction = 0;
                flying = true;
            }

            if (info.Grounded) {
                jumpAction = -1;
            }
        }
    }
}
