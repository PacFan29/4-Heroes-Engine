using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPhysics : DimensionManager
{
    //速度定数
    [Header("速度定数（通常）")]
    public float Acceleration = 0.234375f;
    public float Deceleration = 2.5f;
    public float Friction = 0.234375f;
    public float RollFrc = 0.1171875f;
    public float RollDec = 0.625f;
    public float TopSpeed = 30f;
    public float Slope = 0.625f;
    public float SlopeRollUp = 0.390625f;
    public float SlopeRollDown = 1.5625f;
    public float MaxSpeed = 80f;
    //空中速度定数
    [Header("空中速度定数（通常）")]
    public float AirAcc = 0.46875f;
    public float AirFrc = 0f;
    public float JumpForce = 32.5f;
    public float Gravity = 1.09375f;
    public float TerminalVelocity = -80f;
    //氷上
    [Header("速度定数（氷上）")]
    public float I_Acceleration = 0.234375f;
    public float I_Deceleration = 0.9375f;
    public float I_Friction = 0.1171875f;
    //水中
    [Header("速度定数（水中）")]
    public float U_Acceleration = 0.1171875f;
    public float U_Deceleration = 1.25f;
    public float U_Friction = 0.1171875f;
    public float U_RollFrc = 0.05859375f;
    public float U_RollDec = 0.625f;
    public float U_TopSpeed = 15f;
    [Header("空中速度定数（水中）")]
    public float U_AirAcc = 0.234375f;
    public float U_AirFrc = 0f;
    public float U_JumpForce = 17.5f;
    public float U_Gravity = 0.3125f;

    //Rigidbodyの設定
    protected float ColliderRadius;
    protected float ColliderHeight;
    //Rigidbodyの速度
    protected Vector3 XZvel;
    [Header("速度")]
    public float XZmag;
    protected Vector3 velocity;
    public Vector3 finalVelocity;
    public Vector3 conveyorVelocity;
    public Vector3 extraVelocity;
    private float currentMagnitude = 0f;
    //回転
    protected float hitAngle;
    [HideInInspector] public Quaternion saveRotation;
    private float eulerX;
    private float eulerZ;
    //Raycast
    private RaycastHit hit;
    private Vector3 up;
    [HideInInspector] public Vector3 GroundNormal;
    //状態
    [HideInInspector] public bool Stompable;
    [Header("状態")]
    public bool Grounded;
    public bool Crouching;
    public bool LookingUp;
    public bool rolling;
    public bool attacking;
    public bool skidding;
    [HideInInspector] public bool groundAttack = false;
    //氷上
    protected bool onIce = false;
    private bool onIcePrevious = false;
    //水中
    public bool underwaterTrigger = false;
    public bool underwater = false;
    private bool underwaterPrevious = false;
    //スピードアップ
    public bool speedUp = false;
    private bool speedUpPrevious = false;
    protected float speedUpTime;
    //無敵
    public bool invincible = false;
    protected float invincibleTime;
    //メタル
    public bool metal = false;
    protected float metalTime;

    public bool canWallJump = false;
    public bool activeCollision = true;
    [HideInInspector] public bool groundEvent = false;
    [HideInInspector] public bool desacelerar = false;

    [Header("エフェクト")]
    public GameObject dustTrail;
    public GameObject groundEffect;
    public ParticleSystem invincibleEffect;

    [Header("その他")]
    public GameObject skinGroup;
    public Transform skin;
    public Transform metalSkin;
    public LayerMask GroundLayer;
    public Transform pivot;
    [HideInInspector] public Vector3 input;
    public bool activePhysics = true;
    protected int combo = 0;
    [HideInInspector] public bool attacked;
    [HideInInspector] public bool tookDamage;
    [HideInInspector] public float controlLockTimer;
    [HideInInspector] public bool gravityLock = false;

    [Header("効果音")]
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip skiddingSound;
    public AudioClip healSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip drownSound;
    public AudioClip oneUpSound;
    public AudioClip powerUpSound;
    public AudioClip powerDownSound;
    public AudioClip pipeSound;
    public AudioClip stockSound;
    [Header("効果音(メタル状態)")]
    public AudioClip jumpSoundMetal;
    public AudioClip landSoundMetal;

    Vector3 YcenterUp;
    protected Vector3 previousLandedPos;
    
    protected Dictionary<string, float> SpeedC = new Dictionary<string, float>() {
        {"acc", 0.234375f},
        {"dec", 2.5f},
        {"frc", 0.234375f},
        {"rollfrc", 0.1171875f},
        {"rolldec", 0.625f},
        {"top", 30f},
        {"slp", 0.625f},
        {"slprollup", 0.390625f},
        {"slprolldown", 1.5625f},
        {"fall", 12.5f}
    };
    protected Dictionary<string, float> AirborneC = new Dictionary<string, float>() {
        {"airacc", 0.46875f},
        {"airfrc", 0f},
        {"jmp", 32.5f},
        {"grv", 1.09375f},
        {"terminal", -80f}
    };

    [HideInInspector] public int playerType = 0;

    protected bool speedPassed;

    //初期設定
    void Awake()
    {
        GameManager.dimension = DimensionType.Normal3D;
        rb = GetComponent<Rigidbody>();
        Grounded = true;
        rolling = false;
        saveRotation = skin.transform.rotation;

        constantSetUp();
    }
    void FixedUpdate() {
        Quaternion cameraRotation = Camera.main.transform.rotation;
        float CameraY = Camera.main.transform.eulerAngles.y;

        pivot.rotation = Quaternion.Euler(cameraRotation.eulerAngles.x, 0, cameraRotation.eulerAngles.z);
        pivot.Rotate(0, CameraY, 0f, Space.Self);
        
        rb.velocity = Vector3.zero;
        ColliderRadius = GetComponent<CapsuleCollider>().radius;
        ColliderHeight = GetComponent<CapsuleCollider>().height;
        
        FreezePos(dimension);
        GameManager.dimension = dimension;

        Vector3 pos = this.transform.position;
        switch (dimension){
            case DimensionType.XWay2D:
            //2D(X方向、Z固定)
            pos.z = layerPos - (2.75f * layerRate);
            this.transform.position = pos;
            break;
                
            case DimensionType.ZWay2D:
            //2D(Z方向、X固定)
            pos.x = layerPos + (2.75f * layerRate);
            this.transform.position = pos;
            break;
        }

        Vector3 Ycenter;
        if (rolling) {
            Ycenter = Vector3.zero;
        } else {
            Ycenter = new Vector3(0f, (3.5f - ColliderHeight) / -2f, 0f);
        }
        GetComponent<CapsuleCollider>().center = Ycenter;

        YcenterUp = Vector3.zero;
        YcenterUp += Ycenter.x * transform.right;
        YcenterUp += Ycenter.y * transform.up;
        YcenterUp += Ycenter.z * transform.forward;

        if (activePhysics) {
            BasicPhysics(); //基本物理の実行
        } else {
            // switch (actions){
            //     case 0:
            //     break;
            // }
        }
        
        if (metalSkin != null) {
            metalSkin.transform.rotation = skin.transform.rotation;
        }
    }
    void BasicPhysics(){
        //基本物理
        eulerX = transform.rotation.eulerAngles.x;
        eulerZ = transform.rotation.eulerAngles.z;

        if (Grounded){
            //地上

            //特定のスピードを超えていれば、ソニックのように壁を走れる。
            speedPassed = (playerType == 3) || rolling;
            up = (playerType == 3) ? transform.up : Vector3.up;
            //up = (playerType == 3 || rolling) ? transform.up : Vector3.up;

            GroundMovement();
            canWallJump = false;
        } else {
            //空中
            up = Vector3.up;
            AirMovement();
            if (onIce) constantSetUp();
            onIce = false;
        }

        rb.velocity = finalVelocity + conveyorVelocity + extraVelocity;

        if (metal) {
            underwater = false;
            if (underwater != underwaterPrevious) {
                constantSetUp();
            }
            underwaterPrevious = false;
        } else {
            underwater = underwaterTrigger;
            if (underwater != underwaterPrevious) {
                constantSetUp();
            }
            underwaterPrevious = underwater;
        }
        
        if (speedUp != speedUpPrevious) {
            constantSetUp();
        }
        speedUpPrevious = speedUp;
    }

    void GroundMovement()
    {
        if (!invincible && !rolling) {
            combo = 0;
        }
        //地面判定用の光線
        Physics.Raycast(transform.position+YcenterUp, -transform.up, out hit, ColliderHeight, GroundLayer);
        if (hit.distance > 0f) {
            if (Vector3.Angle(transform.up, hit.normal) <= 60) {
                GroundNormal = hit.normal;
            } else {
                Grounded = false;
                return;
            }
            //地面離れ防止のため
            if (speedPassed) {
                transform.position += GroundNormal * ((ColliderHeight / 2f) - hit.distance);
            } else {
                float diff = (1 - GroundNormal.y) + ((ColliderHeight / 2f) - hit.distance);
                transform.position += Vector3.up * diff;
            }
        }
        //氷上判定
        if (hit.collider != null) {
            onIce = hit.collider.CompareTag("IceFloor");
            if (onIce == !onIcePrevious) constantSetUp();
        }
        onIcePrevious = onIce;

        //地面の向きに応じて回転
        if (speedPassed) {
            Quaternion transformRotation = Quaternion.FromToRotation(Vector3.up, GroundNormal);
            transform.rotation = (Vector3.Angle(Vector3.up, transform.up) >= 170) ?
                Quaternion.FromToRotation(Vector3.up, GroundNormal) : Quaternion.Lerp(transform.rotation, transformRotation, 0.5f);
        } else {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, GroundNormal);
        }

        if (rolling) {
            //転がっている場合
            CommonMoving(SpeedC["acc"], SpeedC["dec"], SpeedC["rollfrc"], true);

            if (XZmag > 0.1f) dustTrailEffect();
        } else {
            //通常
            CommonMoving(SpeedC["acc"], SpeedC["dec"], SpeedC["frc"], false);
        }
        
        if (transform.up == -Vector3.up) {
            //向きが180°の場合
            finalVelocity += velocity.x * -transform.right;
            finalVelocity += velocity.z * -transform.forward;
        } else {
            //通常
            finalVelocity += velocity.x * transform.right;
            finalVelocity += velocity.z * transform.forward;
        }
        if (!speedPassed) transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.up);
        velocity.y = 0;

        if (hit.distance <= 0f) {
            //地面判定がない場合、落ちる。（坂の場合は走っている勢いで跳ぶ。）
            Physics.BoxCast(transform.position+YcenterUp, Vector3.one * 0.5f, -up, out hit, Quaternion.identity, ColliderHeight, GroundLayer);
            
            if (hit.distance <= 0f) {
                velocity = finalVelocity;
                Grounded = false;
            }
        } else if (XZmag <= SpeedC["fall"] || playerType != 3){
            if ((eulerX >= 90 && eulerX <= 270) || (eulerZ >= 90 && eulerZ <= 270)) {
                //90°以上270°以下の状態で止まりそうな時、落ちる。
                velocity = finalVelocity;
                Grounded = false;
            }
        }

        //斜面物理
        hitAngle = getHitAngle();
        float SlopeFactor = 0f;
        if (rolling) {
            //坂を下るときは速く、上るときは遅く。
            SlopeFactor = (Vector3.Angle(Vector3.up, skin.forward) > 90) ?
                SpeedC["slprolldown"] : SpeedC["slprollup"];
        } else {
            //走っている時は、坂を上るときも下るときも同じ。
            SlopeFactor = SpeedC["slp"];
        }
        Vector3 addSpeed = GroundNormal * SlopeFactor;
        addSpeed.y = 0;
        velocity += addSpeed;

        if (desacelerar && XZmag > SpeedC["top"] && !rolling && !skidding) {
            Vector3 sForward = skin.transform.forward;
            velocity.x -= SpeedC["frc"] * Math.Abs(sForward.x) * Math.Sign(velocity.x);
            velocity.z -= SpeedC["frc"] * Math.Abs(sForward.z) * Math.Sign(velocity.z);
        }

        previousLandedPos = this.transform.position;
    }

    void AirMovement()
    {
        //空中回転（X）
        if (eulerX >= 180) this.transform.Rotate(Math.Min(eulerX, 2.8125f), 0f, 0f, Space.Self);
        else this.transform.Rotate(Math.Max(-eulerX, -2.8125f), 0f, 0f, Space.Self);
        //空中回転（Z）
        if (eulerZ >= 180) this.transform.Rotate(0f, 0f, Math.Min(eulerZ, 2.8125f), Space.Self);
        else this.transform.Rotate(0f, 0f, Math.Max(-eulerZ, -2.8125f), Space.Self);

        //天井判定用の光線（先端が丸）
        RaycastHit hitCeiling;
        Physics.SphereCast(transform.position-YcenterUp, 0.4f, up, out hitCeiling, ColliderHeight/2f, GroundLayer);
        if (velocity.y > 0 && (hitCeiling.distance > 0 && hitCeiling.distance <= ColliderHeight) && activeCollision) {
            GroundNormal = hitCeiling.normal;
            hitAngle = getHitAngle();
            if (hitAngle >= 90 && hitAngle <= 135 && playerType == 3){
                //91°以上135°以下、または226°以上270°以下の場合、天井を走ることができる。
                velocity.x = velocity.y * -GroundNormal.x;
                velocity.z = velocity.y * -GroundNormal.z;
                velocity.y = 0;

                transform.rotation = Quaternion.FromToRotation(Vector3.up, GroundNormal);
                Grounded = true;
                rolling = false;
                return; //この関数から抜ける
            } else {
                //頭が痛い
                velocity.y = 0;

                if (hitCeiling.collider.gameObject.GetComponent<QuestionBlockManager>() != null) {
                    hitCeiling.collider.gameObject.GetComponent<QuestionBlockManager>().BlockHit(this.GetComponent<PlayerInfo>(), false);
                }
            }
        }

        //地面判定用の光線（先端が立方体）
        Physics.BoxCast(transform.position+YcenterUp, Vector3.one * 0.5f, -Vector3.up, out hit, Quaternion.identity, (ColliderHeight/2)+0.1f, GroundLayer);
        GroundNormal = Vector3.up;

        //空気抵抗
        if (velocity.y > 0 && velocity.y < 20) {
            velocity.x -= ((float)Math.Floor(velocity.x / 0.125f) / 256);
            velocity.z -= ((float)Math.Floor(velocity.z / 0.125f) / 256);
        }
        //重力
        velocity.y -= AirborneC["grv"] * (metal ? 2 : 1);
        if (velocity.y < AirborneC["terminal"]) {
            velocity.y = AirborneC["terminal"];
        }
        if (velocity.y < 0 && gravityLock) {
            constantChange(false, "grv", Gravity);
            gravityLock = false;
        }

        CommonMoving(AirborneC["airacc"], AirborneC["airacc"], AirborneC["airfrc"], false);

        if (velocity.y < 0 && (hit.distance > 0 && hit.distance <= ((ColliderHeight/2) + 0.2f)) && activeCollision){
            if (groundAttack) {
                if (hit.collider.gameObject.GetComponent<QuestionBlockManager>() != null) {
                    hit.collider.gameObject.GetComponent<QuestionBlockManager>().BlockHit(this.GetComponent<PlayerInfo>(), true);
                } else if (hit.collider.gameObject.GetComponent<PacManSwitch>() != null) {
                    hit.collider.gameObject.GetComponent<PacManSwitch>().press();
                }
            }
            if (playerType != 1 || (playerType == 1 && !underwater)) {
                //着地
                GroundNormal = hit.normal;
                if (!invincible) {
                    combo = 0;
                }

                if (getHitAngle() <= 60) {
                    Grounded = true;
                    hitAngle = getHitAngle();

                    if (!groundEvent) {
                        if (!underwater) {
                            Vector3 pos = transform.position - (transform.up * (ColliderHeight / 2f));
                            Instantiate(groundEffect, pos, Quaternion.FromToRotation(Vector3.up, GroundNormal));
                        }

                        rolling = false;
                        if (metal) {
                            SoundPlay(landSoundMetal);
                        } else {
                            SoundPlay(landSound);
                        }
                    }

                    //地面の取得
                    if (playerType == 3) {
                        if (hitAngle >= 24) {
                            float GroundedMagnitude;
                            if (hitAngle >= 46) {
                                //46°以上
                                velocity.x = velocity.y * Math.Sign(velocity.y) * GroundNormal.x;
                                velocity.z = velocity.y * Math.Sign(velocity.y) * GroundNormal.z;
                            } else {
                                //24°以上
                                velocity.x = velocity.y * Math.Sign(velocity.y) * 0.5f * GroundNormal.x;
                                velocity.z = velocity.y * Math.Sign(velocity.y) * 0.5f * GroundNormal.z;
                            }

                            GroundedMagnitude = new Vector3(velocity.x, 0f, velocity.z).magnitude;
                            if (XZmag > GroundedMagnitude){
                                velocity = SpeedReachToTop(velocity, XZmag);
                            }
                        }
                        transform.rotation = Quaternion.FromToRotation(Vector3.up, GroundNormal);
                    }
                }
            }
        }

        finalVelocity = velocity;
    }
    void CommonMoving(float acceleration, float deceleration, float friction, bool groundRoll){
        Vector3 getInput;

        if (canWallJump) {
            getInput = Vector3.zero;
        } else {
            getInput = input;
        }

        //曲がる速度の調整
        float turnSpeed = Math.Max(0.05f, (60f - XZmag) / 60f);
        if (this.GetComponent<_16MSonic>() != null) {
            if (this.GetComponent<_16MSonic>().boost) {
                turnSpeed = 0.01f;
            }
        }
        if (Grounded && onIce) turnSpeed = 0.01f;

        skin.transform.rotation = saveRotation;

        Vector3 XZvel = new Vector3(velocity.x, 0, velocity.z);
        XZmag = XZvel.magnitude;
        Vector3 sForward = skin.transform.forward;
        finalVelocity = Vector3.zero;

        if ((Crouching || LookingUp) && Grounded) {
            //しゃがんでいるとき
            if (XZmag <= 0 && getInput != Vector3.zero) {
                skin.transform.forward = getInput.normalized;
            }
            velocity.x -= Math.Min(Math.Abs(velocity.x), friction * Math.Abs(sForward.x)) * Math.Sign(velocity.x);
            velocity.z -= Math.Min(Math.Abs(velocity.z), friction * Math.Abs(sForward.z)) * Math.Sign(velocity.z);
        } else {
            if (getInput == Vector3.zero || groundRoll) {
                //摩擦
                if (XZmag < SpeedC["top"]){
                    velocity.x -= Math.Min(Math.Abs(velocity.x), friction * Math.Abs(sForward.x)) * Math.Sign(velocity.x);
                    velocity.z -= Math.Min(Math.Abs(velocity.z), friction * Math.Abs(sForward.z)) * Math.Sign(velocity.z);
                    if (XZmag < friction) {
                        rollingNarrow(groundRoll);
                        skidding = false;
                    }
                    if (XZmag > SpeedC["top"]) velocity = SpeedReachToTop(velocity, SpeedC["top"]);
                }
            }
            if (getInput != Vector3.zero) {
                if (Vector3.Angle(XZvel, getInput) >= 130 && !onIce) {
                    //スリッブ
                    if (!skidding && Grounded && !rolling) {
                        SoundPlay(skiddingSound);
                        currentMagnitude = XZmag;
                    }
                    if (Grounded) skidding = true;
                    velocity.x -= deceleration * Math.Abs(sForward.x) * Math.Sign(velocity.x);
                    velocity.z -= deceleration * Math.Abs(sForward.z) * Math.Sign(velocity.z);
                    if (XZmag < deceleration) {
                        rollingNarrow(groundRoll);
                        if (Grounded && !rolling && getInput != Vector3.zero && currentMagnitude >= 27.5f) {
                            ForwardSetUp(getInput, currentMagnitude);
                        }
                        skidding = false;
                    }

                    if (Grounded && !groundRoll) dustTrailEffect();
                } else {
                    skidding = false;
                    if (XZmag < SpeedC["top"] * input.magnitude){
                        velocity.x += getInput.x * acceleration;
                        velocity.z += getInput.z * acceleration;
                        if (XZmag > SpeedC["top"] * input.magnitude) velocity = SpeedReachToTop(velocity, SpeedC["top"]);
                    }
                    float keepY = velocity.y;
                    if (!Mathf.Approximately(XZvel.sqrMagnitude, 0.0f)){
                        //曲がる
                        Quaternion lateralToInput = Quaternion.FromToRotation(XZvel.normalized, getInput);
                        velocity = Vector3.Lerp(velocity, lateralToInput * velocity, turnSpeed);
                    }
                    velocity.y = keepY;
                }
            } else {
                skidding = false;
            }
            if (XZvel != Vector3.zero && !tookDamage) {
                //進む方向にスキンを向ける
                skin.transform.forward = XZvel.normalized;
            }
        }
        if (XZmag > MaxSpeed) {
            //最大速度を超えた場合
            velocity = SpeedReachToTop(velocity, MaxSpeed);
        }

        saveRotation = skin.transform.rotation;
        skin.transform.rotation = this.transform.rotation * skin.transform.rotation;
        if (transform.up == -Vector3.up) skin.Rotate(0f, 180f, 0f, Space.Self);
    }
    
    void rollingNarrow(bool groundRoll) {
        if (groundRoll) {
            if (isNarrow()) {
                if (playerType == 3) {
                    ForwardSetUp(Vector3.zero, 20f);
                } else {
                    Crouching = true;
                }
            } else {
                rolling = false;
            }
        }
    }
    float getHitAngle(){
        //角度の取得
        return Vector3.Angle(Vector3.up, GroundNormal);
    }
    Vector3 SpeedReachToTop(Vector3 velocity, float top){
        //特定の速度を超えた時の処理
        Vector3 ReducedSpeed = velocity;
        float keepY = velocity.y;
        ReducedSpeed = Vector3.ClampMagnitude(ReducedSpeed, top);
        ReducedSpeed.y = keepY;
        return ReducedSpeed;
    }
    public bool isNarrow() {
        RaycastHit headRaycast;
        Physics.SphereCast(transform.position, 0.4f, transform.up, out headRaycast, 3.5f, GroundLayer);
        return (headRaycast.distance > 0);
        //return (headRaycast.distance > 0 && headRaycast.distance < ((3.5 / 2) - 1f));
    }

    public void SoundPlay(AudioClip sound){
        //効果音の再生
        AudioSource audioSource = GetComponent<AudioSource>();
        if (sound != null) audioSource.PlayOneShot(sound);
    }
    public void StopAllSounds(){
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    public void constantSetUp(){
        /* 通常 */
        SpeedC["acc"] = Acceleration;
        SpeedC["dec"] = Deceleration;
        SpeedC["frc"] = Friction;
        SpeedC["rollfrc"] = RollFrc;
        SpeedC["rolldec"] = RollDec;
        SpeedC["top"] = TopSpeed;
        SpeedC["slp"] = Slope;
        SpeedC["slprollup"] = SlopeRollUp;
        SpeedC["slprolldown"] = SlopeRollDown;
        SpeedC["fall"] = 12.5f;

        AirborneC["airacc"] = AirAcc;
        AirborneC["airfrc"] = AirFrc;
        AirborneC["jmp"] = JumpForce;
        AirborneC["grv"] = Gravity;
        AirborneC["terminal"] = TerminalVelocity;

        if (underwater){
            /* 水中 */
            SpeedC["acc"] = U_Acceleration;
            SpeedC["dec"] = U_Deceleration;
            SpeedC["frc"] = U_Friction;
            SpeedC["rollfrc"] = U_RollFrc;
            SpeedC["rolldec"] = U_RollDec;
            SpeedC["top"] = U_TopSpeed;

            AirborneC["airacc"] = U_AirAcc;
            AirborneC["airfrc"] = U_AirFrc;
            AirborneC["jmp"] = U_JumpForce;
            AirborneC["grv"] = U_Gravity;
        } else if (onIce) {
            /* 氷上 */
            SpeedC["acc"] = I_Acceleration;
            SpeedC["dec"] = I_Deceleration;
            SpeedC["frc"] = I_Friction;
        }

        if (speedUp) {
            /* スピードアップ */
            SpeedC["acc"] *= 2;
            SpeedC["frc"] *= 2;
            SpeedC["rollfrc"] *= 2;
            SpeedC["top"] *= 2;
        }
    }
    public void constantChange(bool ground, string key, float value) {
        if (key != "") {
            if (ground) {
                SpeedC[key] = value;

                if (speedUp && (key == "acc" || key == "frc" || key == "rollfrc" || key == "top")) {
                    /* スピードアップ */
                    SpeedC[key] *= 2;
                }
            } else {
                AirborneC[key] = value;
            }
        }
    }
    
    public void ForwardSetUp(Vector3 forward, float speed){
        Vector3 ReducedSpeed;
        if (forward != Vector3.zero) {
            //特定のオブジェクトの方向にXZ値を設定
            ReducedSpeed = forward * speed;
        } else {
            //プレイヤーの方向にXZ値を設定
            Vector3 thisForward = saveRotation * Vector3.forward;
            ReducedSpeed = thisForward * speed;
        }
        velocity = ReducedSpeed;
    }
    public void YvelSetUp(float speed){
        //Y値を設定
        velocity.y = speed * (metal ? 1.35f : 1);
        if (Grounded) Grounded = false;
    }
    public void VelocitySetUp(Vector3 setVel) {
        if (setVel.y > 0) Grounded = false;
        velocity = setVel;
    }
    public void JumpFromSprings(float speed, Vector3 forward, float lockTime){
        //Y値を設定
        Vector3 jumpVel = forward * speed;
        if (lockTime > 0) {
            controlLockTimer = lockTime;
            gravityLock = true;
            constantChange(false, "grv", 0);

            velocity = jumpVel;
            rolling = false;

            if (Math.Abs(jumpVel.y) > 0 && Grounded) {
                Grounded = false;
            }
        } else {
            if (Math.Abs(jumpVel.x) > 0 || Math.Abs(jumpVel.z) > 0) {
                velocity.x = jumpVel.x;
                velocity.z = jumpVel.z;
            }
            if (Math.Abs(jumpVel.y) > 0) {
                velocity.y = jumpVel.y;
                gravityLock = true;
                if (underwater){
                    constantChange(false, "grv", 0.3125f);
                } else {
                    constantChange(false, "grv", 1.09375f);
                }

                rolling = false;
                if (Grounded) Grounded = false;
            }
        }
    }

    public void dustTrailEffect(){
        Vector3 pos = transform.position - (transform.up * (ColliderHeight / 2f));
        Instantiate(dustTrail, pos, transform.rotation);
    }

    public float GetHalfHeight() {
        return ColliderHeight / 2f;
    }

    // void OnCollisionEnter (Collision hit)
    // {
    //     if (hit.gameObject.tag == "Untagged") {
    //         ContactPoint contact = hit.contacts[0];
    //         if (contact.normal.y < 0.1f) {
    //             ForwardSetUp(Vector3.zero, 0f);
    //         }
    //     }
    // }

    public bool isGroundLayerC(Collision hit) {
        return hit.gameObject.layer == (GroundLayer.value - 1);
    }
}
