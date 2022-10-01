using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MarioActions : MonoBehaviour
{
    protected PlayerInfo info;
    [Header("共通エフェクト")]
    public GameObject sprintEffect;
    private float sprintTime = 1f;
    private bool sprint;
    private bool underwaterPrevious = false;
    protected bool gravityControl;

    //操作等
    [HideInInspector] public int actionId = 0;

    protected PlayerVoiceManager voices;

    [Header("効果音")]
    public AudioClip jumpTurn;
    public AudioClip jumpBroad;
    public AudioClip throwFireball;
    public AudioClip throwIceball;
    [Header("ボイス")]
    public AudioClip[] jumpBroadVoices;

    void Start()
    {
        info = this.GetComponent<PlayerInfo>();
        info.Stompable = true;
        info.playerType = 0;
        info.desacelerar = true;

        voices = GetComponent<PlayerVoiceManager>();
    }
    // Update is called once per frame
    public void actions()
    {
        /*
        共通する技
        ・360°歩行（物理）
        ・ダッシュ
        ・しゃがみジャンプ
        ・幅跳び
        ・横宙返り
        */
        bool crouchButton = info.GetCrouchButton("LB") || info.GetCrouchButton("RB");
        
        if (crouchButton && info.Grounded && !info.Crouching && !info.rolling) {
            if (info.GroundNormal != Vector3.up) {
                if (info.XZmag <= info.Friction) {
                    info.VelocitySetUp(info.skin.forward * info.Friction * 2);
                }
                info.rolling = true;
            } else {
                info.Crouching = true;
            }
        } else if (!crouchButton && info.Grounded && info.Crouching && !info.rolling && !info.isNarrow()) {
            info.Crouching = false;
        }
        if (info.Crouching) {
            this.GetComponent<CapsuleCollider>().height = 2.4f;
        } else {
            this.GetComponent<CapsuleCollider>().height = 3.5f;
        }

        bool jumpButton = info.Buttons["A"] || info.Buttons["B"];

        if ((info.ButtonsDown["A"] || info.ButtonsDown["B"]) && info.Grounded && !info.underwater) {
            info.rolling = false;

            if (info.input.magnitude > 0 && crouchButton && info.XZmag > 0 && !info.isNarrow()) {
                //幅跳び
                voices.VoicePlay(jumpBroadVoices);
                info.SoundPlay(jumpBroad);
                actionId = 2;
                info.Crouching = false;

                info.constantChange(false, "grv", info.Gravity * 0.64f);
                info.ForwardSetUp(Vector3.zero, 45f);
                info.YvelSetUp(25f);
            } else {
                if (info.skidding) {
                    //横宙返り
                    info.SoundPlay(jumpTurn);
                    actionId = 3;

                    info.ForwardSetUp(Vector3.zero, -5f);
                    info.YvelSetUp(40f);
                } else {
                    info.Jump();
                    
                    voices.Jump();
                }
            }
        } else if ((info.ButtonsDown["A"] || info.ButtonsDown["B"]) && info.underwater) {
            info.Swim();
        }

        switch (actionId) {
            case 2:
            //幅跳び
            gravityControl = false;
            info.axisInput = false;

            if (info.Grounded) {
                actionId = 0;
                info.axisInput = true;
            }
            break;

            case 3:
            //横宙返り
            gravityControl = !(info.finalVelocity.y > 0);
            if (!gravityControl) info.constantChange(false, "grv", info.Gravity / 2f);

            if (info.Grounded || info.canWallJump) {
                if (!info.canWallJump) actionId = 0;
                info.axisInput = true;
            }
            break;
        }
        if (info.Grounded && !gravityControl && actionId == 0) {
            gravityControl = true;
        }

        if (info.underwater) {
            if (info.Grounded) {
                info.constantChange(true, "top", info.U_TopSpeed);
            } else {
                info.constantChange(true, "top", 30f);
            }
            if (info.finalVelocity.y > 0){
                info.constantChange(false, "grv", 0.625f);
            } else {
                info.constantChange(false, "grv", info.U_Gravity);
            }
        } else {
            if (gravityControl && !info.gravityLock) {
                if (info.finalVelocity.y >= 20 && jumpButton){
                    info.constantChange(false, "grv", info.Gravity / 3f); //例）0.625f
                } else {
                    info.constantChange(false, "grv", info.Gravity);
                }
            }

            if (info.XZmag >= 30) {
                info.constantChange(false, "jmp", info.JumpForce + 5.00f); //例）39.375f
            } else if (info.XZmag >= 20) {
                info.constantChange(false, "jmp", info.JumpForce + 2.5f); //例）36.875f
            } else if (info.XZmag >= 10) {
                info.constantChange(false, "jmp", info.JumpForce + 1.25f); //例）35.625f
            } else {
                info.constantChange(false, "jmp", info.JumpForce); //例）34.375f
            }

            if (info.Grounded) {
                if ((info.Buttons["X"] || info.Buttons["Y"])) {
                    if (sprintTime > 0 && info.XZmag >= info.TopSpeed - 2.5f) {
                        // if (sprintTime >= 1f) Instantiate(sprintEffect, transform.position, info.skin.rotation);
                        sprintTime -= Time.deltaTime;
                    }
                    if (sprintTime <= 0 && !sprint) {
                        if (!info.rolling) Instantiate(sprintEffect, transform.position, info.skin.rotation);
                        sprint = true;
                        info.ForwardSetUp(Vector3.zero, info.TopSpeed * 1.5f);
                        info.constantChange(true, "top", Math.Min(30f, info.TopSpeed * 1.5f)); //例）30f
                    }
                } else {
                    sprint = false;
                    sprintTime = 1f;
                    info.constantChange(true, "top", info.TopSpeed);
                }
            } else {
                if (info.XZmag > info.TopSpeed * 1.25f) {
                    sprintTime = 0f;
                } else {
                    sprintTime = 1f;
                }
                sprint = false;
            }
        }
        if (sprint) {
            info.dustTrailEffect();
        }

        if (info.underwater != underwaterPrevious) {
            if (!info.underwater && jumpButton) {
                info.YvelSetUp(31.875f);
            } else if (info.underwater) {
                info.YvelSetUp(info.finalVelocity.y / 4f);
            }
        }
        underwaterPrevious = info.underwater;

        this.GetComponent<PlayerAnimManager>().actionId = actionId;


        if (info.ButtonsDown["X"] && info.powerUpActive == 1) {
            info.SoundPlay(throwFireball);
            
            FireBall fire = Instantiate(info.fireBall, transform.position + info.skin.forward, info.skin.rotation).GetComponent<FireBall>();
            fire.player = info;
        }
        if (info.ButtonsDown["X"] && info.powerUpActive == 2) {
            info.SoundPlay(throwIceball);
            
            IceBall ice = Instantiate(info.iceBall, transform.position + info.skin.forward, info.skin.rotation).GetComponent<IceBall>();
            ice.player = info;
        }
    }
    void OnCollisionEnter (Collision hit)
    {
        if (info.isGroundLayerC(hit)) {
            ContactPoint contact = hit.contacts[0];
            if (!info.Grounded && contact.normal.y < 0.1f && !info.canWallJump && !info.underwater && actionId == 2) {
                voices.VoicePlay(voices.HurtVoices);
                info.SoundPlay(info.damageSound);
                StartCoroutine("HitWall", -info.skin.forward);
            }
        }
    }
    IEnumerator HitWall(Vector3 direction) {
        info.constantChange(false, "grv", info.Gravity);
        info.canInput = false;
        actionId = -1;

        Vector3 damageVelocity = direction.normalized * 15f;
        damageVelocity.y = 22f;
        info.VelocitySetUp(damageVelocity);

        Vector3 XZvel = new Vector3(damageVelocity.x, 0, damageVelocity.z) * -1;

        while (!info.Grounded) {
            info.skin.transform.forward = XZvel.normalized;
            yield return new WaitForSeconds(0);
        }

        actionId = 0;
        info.canInput = true;
        info.axisInput = true;
        info.tookDamage = false;
    }

    public IEnumerator DamageAnimation(Vector3 direction) {
        StartCoroutine("HitWall", direction);
        yield return null;
    }
    public IEnumerator DeathAnimation() {
        info.LookFront();

        info.VelocitySetUp(Vector3.zero);
        info.activePhysics = false;
        GetChildren(info.skin.gameObject, "Top Priority");
        actionId = -2;

        this.GetComponent<CapsuleCollider>().isTrigger = true;

        yield return new WaitForSeconds(0.8f);

        voices.Death();
    }
    void GetChildren(GameObject obj, string layer) {
        Transform children = obj.GetComponentInChildren<Transform>();
        //子要素がいなければ終了
        if (children.childCount == 0) {
            return;
        }
        foreach(Transform ob in children) {
            //ここに何かしらの処理
            ob.gameObject.layer = LayerMask.NameToLayer(layer);
            GetChildren(ob.gameObject, layer);
        }
    }

    public void Reset() {
        info.activePhysics = true;
        info.activeCollision = true;
        actionId = 0;
        GetComponent<CapsuleCollider>().isTrigger = false;
        GetChildren(info.skin.gameObject, "Ignore Raycast");
    }
}
