using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _15Knuckles : SonicActions
{
    private float jumpTime;
    private bool sliding = false;
    private int glidingTrigger = 0; //0=通常、1=滑空、2=壁にくっつく
    [Header("効果音")]
    public AudioClip spinSound;
    public AudioClip slidingSound;
    public LoopingSoundManager lManager;

    Vector3 contactNormal;

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

        //滑空
        switch (glidingTrigger) {
            case 1:
            info.ForwardSetUp(Vector3.zero, info.XZmag + 0.078125f);

            if (!info.Buttons["A"]) {
                info.constantChange(false, "grv", info.Gravity);
                jumpAction = -1;
                glidingTrigger = 0;
                info.MaxSpeed = 80f;
                info.axisInput = true;
            }

            if (info.finalVelocity.y < -2.5f) {
                info.YvelSetUp(-2.5f);
            }

            if (info.Grounded) {
                info.constantChange(false, "grv", info.Gravity);
                jumpAction = -1;
                glidingTrigger = 0;
                info.MaxSpeed = 80f;
                info.axisInput = true;
            }
            break;

            case 2:
            if (info.ButtonsDown["A"]) {
                info.constantChange(false, "grv", info.Gravity);
                jumpAction = -1;
                info.MaxSpeed = 80f;
                info.axisInput = true;

                glidingTrigger = 0;
                info.activePhysics = true;
                info.ForwardSetUp(contactNormal, 20f);

                inputA = true;
                jumped = true;
                info.Jump();

                voices.Jump();
            }
            break;

            default:
            if (jumpAction > 0 && info.ButtonsDown["A"]) {
                info.rolling = false;
                info.MaxSpeed = 120f;
                jumpAction = 0;
                glidingTrigger = 1;

                info.constantChange(false, "grv", 0.625f);

                info.ForwardSetUp(Vector3.zero, 20f);
                info.axisInput = false;
                info.YvelSetUp(0f);
            }

            if (info.Grounded) {
                jumpAction = -1;
            }
            break;
        }
    }
    void OnCollisionStay (Collision hit)
    {
        if (info.isGroundLayerC(hit)) {
            ContactPoint contact = hit.contacts[0];
            if (!info.Grounded && contact.normal.y < 0.1f && glidingTrigger == 1) {
                glidingTrigger = 2;
                contactNormal = contact.normal;

                info.skin.forward = -contactNormal;

                info.VelocitySetUp(Vector3.zero);
                info.finalVelocity = Vector3.zero;
                
                info.activePhysics = false;
            }
        }
    }
}
