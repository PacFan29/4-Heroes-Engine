using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SonicActions : MonoBehaviour
{
    [Header("速度定数（スーパー）")]
    public float S_Acceleration = 0.9375f;
    public float S_Deceleration = 5f;
    public float S_TopSpeed = 50f;
    
    [Header("空中速度定数（スーパー）")]
    public float S_AirAcc = 1.875f;
    public float S_JumpForce = 40f;

    [Header("速度定数（スーパー、水中）")]
    public float S_U_Acceleration = 0.46875f;
    public float S_U_Deceleration = 2.5f;
    public float S_U_TopSpeed = 25f;
    
    [Header("空中速度定数（スーパー、水中）")]
    public float S_U_AirAcc = 0.9375f;
    public float S_U_JumpForce = 17.5f;

    [Header("スキン")]
    public GameObject normalSkin;
    public GameObject spinSkin;
    protected bool enableSpin = false;
    [Header("スーパー状態かどうか")]
    public bool isSuper;
    private bool superPrevious;
    private float superTime;

    protected PlayerInfo info;

    //操作等
    protected bool inputA;
    protected bool canJump = true;
    protected bool jumped = false;
    protected int jumpAction = -1;
    protected int actionId;
    private bool underwaterPrevious = false;

    protected PlayerVoiceManager voices;
    [Header("摩擦")]
    public GameObject frictionWeapon;
    private float fricInterval = 0f;
    
    void Start()
    {
        info = GetComponent<PlayerInfo>();
        info.Stompable = false;
        info.playerType = 3;
        info.desacelerar = false;

        voices = GetComponent<PlayerVoiceManager>();
    }

    void FixedUpdate() {
        if (this.GetComponent<PlayerAnimManager>() != null) {
            this.GetComponent<PlayerAnimManager>().skin.speed = 1 + (info.Grounded ? (info.XZmag / 30f) : 0);
        }

        normalSkin.SetActive(!enableSpin);
        spinSkin.SetActive(enableSpin);
        // if (info.XZmag >= 60) {
        //     info.scorePopUp((int)info.XZmag - 59, this.transform.position);
        // }
    }

    public void actions()
    {
        if (info.XZmag <= 1f && (info.GetCrouchButton("RB") || info.GetCrouchButton("B")) && info.Grounded && !info.Crouching && !info.rolling) {
            info.Crouching = true;
        } else if ((!info.GetCrouchButton("RB") && !info.GetCrouchButton("B")) && info.Grounded && info.Crouching) {
            info.Crouching = false;
        }

        if (info.ButtonsDown["A"] && info.Grounded && !info.Crouching && canJump) {
            info.Crouching = false;
            inputA = true;
            jumped = true;
            info.Jump();

            voices.Jump();
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
        if (!info.Grounded && !info.Buttons["A"] && jumpAction == -1) {
            jumpAction = 1;
        } else if ((info.Grounded || !info.rolling) && jumpAction == 0) {
            jumpAction = -1;
        }

        if (info.Crouching || info.rolling) {
            this.GetComponent<CapsuleCollider>().height = 2.4f;
        } else {
            this.GetComponent<CapsuleCollider>().height = 3.5f;
        }

        this.GetComponent<PlayerAnimManager>().actionId = actionId;

        if (info.underwater != underwaterPrevious) {
            if (info.underwater) {
                Vector3 setvel = info.finalVelocity / 2f;
                setvel.y /= 2f;
                info.VelocitySetUp(setvel);
            } else {
                info.YvelSetUp(info.finalVelocity.y * 2);
            }
        }
        underwaterPrevious = info.underwater;

        //摩擦
        if (info.rolling && info.Grounded && info.XZmag > 0f && info.powerUpActive > 0) {
            fricInterval += Time.deltaTime;
            if (fricInterval >= 0.1f) {
                fricInterval = 0f;
                FrictionWeapons fr = Instantiate(frictionWeapon, transform.position + (transform.up * -0.3f), this.transform.rotation).GetComponent<FrictionWeapons>();
                fr.index = info.powerUpActive - 1;
                fr.player = info;
            }
        }

        //スーパー
        if (!info.Grounded && info.ButtonsDown["Y"] && !isSuper && false && GameManager.Coins >= 50) {
            isSuper = true;
        }

        if (isSuper) {
            if (!superPrevious) {
                SuperConstantChange();
            }

            superTime += Time.deltaTime;
            if (superTime >= 1f) {
                GameManager.Coins--;
                if (info.playerId == 16) info.scoreIncrease(100);
                superTime -= 1f;
            }

            if (GameManager.Coins <= 0) {
                isSuper = false;
                info.constantSetUp();
            }

            info.speedUp = false;
            info.invincible = true;
        } else {
            if (superPrevious) {
                info.constantSetUp();
            }
            superTime = 0f;
        }
        superPrevious = isSuper;
    }

    public void SuperConstantChange() {
        if (info.underwater) {
            info.constantChange(true, "acc", S_U_Acceleration);
            info.constantChange(true, "dec", S_U_Deceleration);
            info.constantChange(true, "top", S_U_TopSpeed);
            
            info.constantChange(false, "airacc", S_U_AirAcc);
            info.constantChange(false, "jmp", S_U_JumpForce);
        } else {
            info.constantChange(true, "acc", S_Acceleration);
            info.constantChange(true, "dec", S_Deceleration);
            info.constantChange(true, "top", S_TopSpeed);
            
            info.constantChange(false, "airacc", S_AirAcc);
            info.constantChange(false, "jmp", S_JumpForce);
        }
    }

    public IEnumerator DamageAnimation(Vector3 direction) {
        info.canInput = false;
        info.rolling = false;
        actionId = -1;

        Vector3 damageVelocity = direction * 10f;
        damageVelocity.y = 20f;
        info.VelocitySetUp(damageVelocity);
        info.constantChange(false, "grv", 0.9375f);

        Vector3 XZvel = new Vector3(damageVelocity.x, 0, damageVelocity.z) * -1;

        while (!info.Grounded) {
            info.skin.transform.forward = XZvel.normalized;
            yield return new WaitForSeconds(0);
        }
        
        actionId = 0;
        info.VelocitySetUp(Vector3.zero);
        info.constantChange(false, "grv", info.Gravity);
        info.tookDamage = false;
        info.canInput = true;
        canJump = true;
    }
    public IEnumerator DeathAnimation() {
        info.LookFront();
        GetChildren(info.skin.gameObject, "Top Priority");

        voices.Death();
        
        actionId = -2;
        info.activeCollision = false;
        info.ForwardSetUp(-Vector3.forward, 0);
        info.YvelSetUp(35);
        info.constantChange(false, "grv", info.Gravity);
        GetComponent<CapsuleCollider>().isTrigger = true;

        this.gameObject.layer = LayerMask.NameToLayer("Top Priority");

        yield return null;
    }
    public IEnumerator DrownAnimation() {
        info.LookFront();
        GetChildren(info.skin.gameObject, "Top Priority");
        
        info.activeCollision = false;
        info.ForwardSetUp(-Vector3.forward, 0);
        info.YvelSetUp(0);
        info.constantChange(false, "grv", info.Gravity);
        GetComponent<CapsuleCollider>().isTrigger = true;

        this.gameObject.layer = LayerMask.NameToLayer("Top Priority");

        yield return null;
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
        info.activeCollision = true;
        actionId = 0;
        GetComponent<CapsuleCollider>().isTrigger = false;
        GetChildren(info.skin.gameObject, "Ignore Raycast");
    }
}
