using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfo : PlayerController
{
    private Vector3 currentPosition;
    [HideInInspector] public int playerId;  //プレイヤーのID（例：０ → マリオ）
    public int maxHP = 30; //最大ＨＰ
    [HideInInspector] public int HP;        //ＨＰ
    private float hpRate;
    
    [HideInInspector] public int lives = 0; //残機
    public int localScore = 0; //プレイヤーのスコア
    public float maxAirAmount = 24f;
    [HideInInspector] public float airAmount = 24f;
    [HideInInspector] public int airCountdown = 5;

    [Header("エフェクト等のオブジェクト")]
    public GameObject OneUpObj; //1UPアピール用オブジェクト
    private GameObject OneUpObjControl;
    public GameObject hitEffect;
    public GameObject target;
    public GameObject ScorePopUp;
    private int points;
    private int rate = 1;
    public float rateTime = 0f;

    [Header("３大パワーアップ")]
    public int powerUpActive = 0; //0＝なし、1＝ファイア、2＝アイス、3＝サンダー
    public GameObject fireBall;
    public GameObject iceBall;
    [Header("４大バリア")]
    public int shieldActive = 0; //0＝なし、1＝バリア、2＝フレイム、3＝アクア、4＝サンダー

    private int cookiesAmount;
    private int ringsAmount;

    private AudioSource source;
    private PlayerVoiceManager voices;

    private float damageTime;

    [Header("データ")]
    public GameData data;
    public Options options;

    void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine("setPlayerInfo");

        if (options.beginnerMode) {
            maxHP *= 2;
        }
        if (data.HPs[playerNumber] <= 0) {
            HP = maxHP;
        } else {
            HP = data.HPs[playerNumber];
        }

        lives = data.lives[playerNumber];

        if (playerId != 16) powerUpActive = data.powerUps[playerNumber];
        shieldActive = data.shields[playerNumber];

        voices = GetComponent<PlayerVoiceManager>();

        currentPosition = this.transform.position;

        metalSkin.gameObject.SetActive(true);
    }
    void Update()
    {
        //最大HPの比率（エクストラの場合：60%）
        hpRate = GameManager.extra ? 0.6f : 1f;

        //最大HP（1～50）
        maxHP = Mathf.Clamp(maxHP, 5, 100);
        //HP（0～最大HP）
        HP = Mathf.Clamp(HP, 0, (int)((float)maxHP * hpRate));
        if (playerId != 16) data.HPs[playerNumber] = (HP == maxHP) ? 999 : HP;
        //残機（0～99）
        lives = Mathf.Clamp(lives, 0, 99);
        //スコア（0～999,999,999）
        localScore = Mathf.Clamp(localScore, 0, 999999999);

        data.powerUps[playerNumber] = powerUpActive;
        data.shields[playerNumber] = shieldActive;

        if (GameManager.players.Count > playerNumber) {
            //プレイヤークラスの値を設定
            if (playerId != 16) {
                GameManager.players[playerNumber].setStatus(playerId, maxHP, HP, lives, localScore);
            }
        }
        Controller();

        RaycastHit crushRaycast;
        Physics.SphereCast(transform.position, 0.4f, transform.up, out crushRaycast, ColliderHeight, GroundLayer);
        if (crushRaycast.distance > 0 && crushRaycast.distance < ((ColliderHeight / 2) - 1f) && HP > 0 && Grounded) {
            //上から挟まれた！！
            //Debug.Log("Oh no");
            HP = 0;
            Death();
        }

        if (this.transform.position.y <= -100f && HP > 0) {
            //落下
            Fallen();
        }

        if (HP > 0) {
            //ダメージを受けた後の点滅
            if (damageTime > 0 && !tookDamage) {
                damageTime -= Time.deltaTime;
                BlinkingSkin((damageTime % 0.1f) / 0.05f < 1);
            } else {
                BlinkingSkin(true);
            }
        }
        MetalSkinRender();

        //1UPアピール用オブジェクト
        if (OneUpObjControl != null) {
            OneUpObjControl.transform.position = transform.position + (Vector3.up*2);
        }

        //水中
        if (maxAirAmount > 0) {
            if (underwater && shieldActive != 3 && !metal && HP > 0) {
                if (shieldActive == 2 || shieldActive == 4) {
                    //フレイムバリア及びサンダーバリアが無効となる。
                    shieldActive = 0;
                }
                if (airAmount <= 0) {
                    //カウントダウン
                    if (12 + airAmount <= airCountdown * 2) {
                        airCountdown--;
                    }
                }
                if (airAmount <= -12 && airAmount > -13) {
                    Drown();
                    airAmount = -13f;
                } else {
                    airAmount -= Time.deltaTime;
                    if (airAmount <= 0 && airCountdown == 5 && playerType == 3) {
                        MusicManager.musicIndex = 3;
                    }
                }
            } else {
                airAmount = maxAirAmount;
                airCountdown = 5;
            }

            if (MusicManager.musicIndex == 3 && (!underwater || airAmount > 0)) {
                MusicManager.musicIndex = 0;
            }
        }

        //スピードアップ
        if (speedUpTime > 0) {
            speedUpTime -= Time.deltaTime;
            if (speedUpTime <= 0) {
                speedUp = false;
                if (MusicManager.musicIndex == 4) {
                    MusicManager.musicIndex = 0;
                }
            }
        }

        var invincibleEmission = invincibleEffect.emission;
        if (invincibleTime > 0) {
            //無敵エフェクト
            invincibleEmission.rateOverTime = 100f;
            ParticleSystem.ShapeModule invincibleShape = invincibleEffect.shape;
            invincibleShape.position = this.transform.position - this.gameObject.transform.parent.gameObject.transform.position;

            //エフェクトタイム
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0) {
                invincible = false;
                if (MusicManager.musicIndex == 5) {
                    MusicManager.musicIndex = 0;
                }
            }
        } else {
            invincibleEmission.rateOverTime = 0f;
        }
        if (metalTime > 0) {
            //エフェクトタイム
            metalTime -= Time.deltaTime;
            if (metalTime <= 0) {
                metal = false;
                if (MusicManager.musicIndex == 6) {
                    MusicManager.musicIndex = 0;
                }
            }
        }

        //上を見上げている状態
        LookingUp = (
            XZmag <= 1f && 
            ((Axises["Vertical"] > 0.5) || (Axises["Alt Vertical"] > 0.5)) && 
            GameManager.dimension != DimensionType.Normal3D
        );

        if (rateTime > 0) {
            if (rateTime > 10) {
                rateTime = 10f;
            }
            rateTime -= Time.deltaTime;
            if (rateTime <= 0) {
                rate = 1;
            }
        }
    }
    IEnumerator setPlayerInfo(){
        //例）P2の場合、0.2秒待ってから追加する
        yield return new WaitForSeconds (0.05f * (playerNumber+1));
        GameManager.players.Add(new PlayerStatus());
    }

    void BlinkingSkin(bool enabled) {
        Renderer[] childrenRenderer = skin.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < childrenRenderer.Length; i++){
			childrenRenderer[i].enabled = enabled && !metal;
		}
    }
    void MetalSkinRender() {
        if (metalSkin == null) {
            return;
        }
        Renderer[] childrenRenderer = metalSkin.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < childrenRenderer.Length; i++){
			childrenRenderer[i].enabled = metal;
		}
    }

    public void Jump() {
        //ジャンプ
        if (metal) {
            SoundPlay(jumpSoundMetal);
        } else {
            SoundPlay(jumpSound);
        }
        if (speedPassed) {
            velocity = finalVelocity + AirborneC["jmp"] * (metal ? 1.35f : 1) * transform.up;
        } else {
            velocity.y = AirborneC["jmp"] * (metal ? 1.35f : 1);
        }

        GroundNormal = Vector3.up;

        Grounded = false;
    }
    public void Swim() {
        velocity.y = AirborneC["jmp"];
        GroundNormal = Vector3.up;
        Grounded = false;
    }
    public void Stomp(){
        //弱い敵を踏んだ時
        attacked = true;

        switch (playerType) {
            case 0:
            //マリオ
            Grounded = false;

            if (this.GetComponent<MarioActions>().actionId != 5) {
                velocity.y = 30f;
                this.GetComponent<MarioActions>().actionId = 0;
            }
            axisInput = true;
            break;
            
            case 1:
            //パックマン
            ;
            break;

            case 2:
            //ロックマン
            ;
            break;

            case 3:
            //ソニック
            if (velocity.y < 0 && !Grounded) velocity.y *= -1;
            activePhysics = true;
            break;
        }
    }
    public void BigStomp(){
        //敵を踏んだ時（マリオ）
        attacked = true;
        
        switch (playerType) {
            case 0:
            //マリオ
            Grounded = false;
            velocity.y = 40f;

            this.GetComponent<MarioActions>().actionId = 0;
            axisInput = true;
            break;
            
            case 1:
            //パックマン
            Grounded = true;
            break;

            case 2:
            //ロックマン
            ;
            break;

            case 3:
            //ソニック
            velocity.x *= -1;
            velocity.y *= -1;
            velocity.z *= -1;
            break;
        }
        
    }
    public int ComboIncrease(){
        //コンボ上昇（8以上で1UP）
        if (combo < 99) combo++;
        if (GameManager.maxCombo < combo) GameManager.maxCombo = combo;

        if (combo >= 8) {
            OneUp();
        }

        return Math.Min(5, combo);
    }
    public int GetCombo(){
        //コンボの取得
        return Math.Min(5, combo);
    }
    public void ComboReset(){
        if (!invincible || playerId == 16) {
            //コンボの初期化
            combo = 0;
        }
    }
    public void RateUp() {
        rateTime = 10f;
        if (rate < 16) {
            rate *= 2;
        } else {
            scorePopUp(1000, false, this.transform.position);
        }
    }

    public bool getNormalAttack(){
        //通常攻撃か否かを取得
        return true;
    }
    public void TakeDamage(int value, Vector3 colPos, int damageType = 0){
        if (metal) {
            //メタルでダメージ無効化
            return;
        }
        if (damageType == 1 && shieldActive == 2) {
            //フレイムバリアで火によるダメージを防ぐ
            return;
        }
        Vector3 thisPos = this.transform.position;
        (thisPos.y, colPos.y) = (0, 0);
        Vector3 direction = this.transform.position - colPos;
        float dMagnitude;

        switch (GameManager.dimension){
            case DimensionType.XWay2D:
            //2D(X方向、Z固定)
            dMagnitude = direction.magnitude * Math.Sign(direction.x);
            direction = new Vector3(dMagnitude, 0f, 0f);
            break;
            
            case DimensionType.ZWay2D:
            //2D(Z方向、X固定)
            dMagnitude = direction.magnitude * Math.Sign(direction.z);
            direction = new Vector3(0f, 0f, dMagnitude);
            break;
        }
        
        if (damageTime <= 0 && HP > 0 && !invincible) {
            if (playerNumber == 0 && GameManager.perfect > 3) GameManager.perfect = 3;
            bool powerDown = false;

            //ダメージの処理
            if (shieldActive > 0) {
                shieldActive = 0;
            } else {
                HP -= Math.Abs(value);
                if (powerUpActive > 0) {
                    powerDown = true;
                    StartCoroutine(PowerDown(powerUpActive));
                    if (HP <= 0) HP = 1;
                }
                if (GetComponent<_16MSonic>() != null && HP > 0) {
                    GetComponent<_16MSonic>().RingSpread();
                }
            }
            if (HP <= 0){
                Death();
            } else {
                voices.Hurt();
                Instantiate(hitEffect, transform.position, Quaternion.identity);
                switch (damageType) {
                    case 1:
                    //火によるダメージ
                    SoundPlay(fireDamageSound);
                    break;
                    
                    case 2:
                    //氷によるダメージ
                    SoundPlay(iceDamageSound);
                    break;
                    
                    case 3:
                    //電撃によるダメージ
                    SoundPlay(elecDamageSound);
                    break;
                    
                    case 4:
                    //トゲによるダメージ
                    SoundPlay(spikeDamageSound);
                    break;

                    default:
                    //通常ダメージ
                    SoundPlay(damageSound);
                    break;
                }
                damageTime = 2f;
                tookDamage = true;

                if (GetComponent<MarioActions>() != null) {
                    //マリオ
                    if (!powerDown) SoundPlay(powerDownSound);
                    GetComponent<MarioActions>().StartCoroutine("DamageAnimation", direction);
                } else if (GetComponent<PacManActions>() != null) {
                    //パックマン
                    GetComponent<PacManActions>().StartCoroutine("DamageAnimation");
                } else if (GetComponent<MegaManActions>() != null) {
                    //ロックマン
                    GetComponent<MegaManActions>().StartCoroutine("DamageAnimation", direction);
                } else if (GetComponent<SonicActions>() != null) {
                    //ソニック
                    GetComponent<SonicActions>().StartCoroutine("DamageAnimation", direction);
                }
            }
        }
    }
    public void HPRestore(int value){
        //回復の処理
        HP += Math.Abs(value);
    }
    public void SpeedUp(){
        //スピードアップ
        MusicManager.musicIndex = 4;
        speedUp = true;
        speedUpTime = 20f;
    }
    public void Invincible(float time){
        //無敵
        MusicManager.musicIndex = 5;
        invincible = true;
        invincibleTime = time;
    }
    public void MetalCookie(){
        //メタル
        //MusicManager.musicIndex = 5;
        metal = true;
        metalTime = 20f;
        if (playerType == 0) {
            MusicManager.musicIndex = 6;
        }
    }
    public void setPlayerId(int id){
        //プレイヤーIDの設定
        playerId = id;
    }
    public void OneUp(){
        if (options.lives) {
            SoundPlay(oneUpSound);
            if (OneUpObjControl != null) Destroy(OneUpObjControl);
            OneUpObjControl = Instantiate(OneUpObj, transform.position + (Vector3.up*2), Quaternion.identity);
            lives++;
        }
    }
    public void scorePopUp(int num, bool doubleRate, Vector3 position){
        if (!GameManager.bossHPShow && GameManager.ScoreShow){
            int points = num * rate;
            localScore += points * (doubleRate ? 2 : 1);
            GameObject popup = Instantiate(ScorePopUp, position, Quaternion.identity);
            popup.GetComponent<ScoringController>().points = points;

            if (doubleRate) {
                popup.GetComponent<ScoringController>().rate = 2;
            }
        }
    }
    public void scoreIncrease(int num){
        if (!GameManager.bossHPShow && GameManager.ScoreShow){
            localScore += (num * rate);
        }
    }

    void Death() {
        canInput = false;
        SoundPlay(deathSound);

        if (GetComponent<MarioActions>() != null) {
            //マリオ
            GetComponent<MarioActions>().StartCoroutine("DeathAnimation");
        } else if (GetComponent<PacManActions>() != null) {
            //パックマン
            GetComponent<PacManActions>().StartCoroutine("DeathAnimation");
        } else if (GetComponent<MegaManActions>() != null) {
            //ロックマン
            GetComponent<MegaManActions>().StartCoroutine("DeathAnimation");
        } else if (GetComponent<SonicActions>() != null) {
            //ソニック
            GetComponent<SonicActions>().StartCoroutine("DeathAnimation");
        }
        GetComponent<PlayerAnimManager>().actionId = -2;

        StartCoroutine("LostLife");
    }

    IEnumerator LostLife(){
        //やられた
        if (playerNumber == 0) GameManager.perfect = 0;

        if (!options.lives && lives < 1) {
            lives = 1;
        }

        if (areTheyPerished()) {
            //全滅した時の処理
            if (GetComponent<MarioActions>() != null) {
                MusicManager.musicIndex = 1;
            } else if (GetComponent<MegaManActions>() != null) {
                MusicManager.musicIndex = 2;
            }
            
            GameManager.death = true;

            yield return new WaitForSeconds(2f);

            if (lives <= 0){
                yield return new WaitForSeconds(1f);

                //ゲームオーバー
                MusicManager.musicFade = true;
                GameManager.gameOver = true;
            } else {
                //ワイプ
                WipeManager.WipeSetUp(true, 1.5f);
                MusicManager.musicFade = true;

                yield return new WaitForSeconds(2f);

                //残機を減らす
                data.ResetExceptTime();
                if (options.lives) data.LostLife(playerNumber);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        } else {
            //単体時の処理
            if (lives > 0) {
                yield return new WaitForSeconds(5f);

                //泡状態になり、他のプレイヤーに追従
                this.transform.position = currentPosition;

                if (options.lives) data.LostLife(playerNumber);
                lives = data.lives[playerNumber];
                HP = maxHP;

                canInput = true;
                GetComponent<PlayerAnimManager>().actionId = 0;

                if (GetComponent<MarioActions>() != null) {
                    //マリオ
                    GetComponent<MarioActions>().Reset();
                } else if (GetComponent<PacManActions>() != null) {
                    //パックマン
                    GetComponent<PacManActions>().Reset();
                } else if (GetComponent<MegaManActions>() != null) {
                    //ロックマン
                    GetComponent<MegaManActions>().Reset();
                } else if (GetComponent<SonicActions>() != null) {
                    //ソニック
                    GetComponent<SonicActions>().Reset();
                }
            }
        }
    }
    void Fallen(){
        //落ちた
        if (options.beginnerMode) {
            HP -= 8;
            if (GetComponent<_16MSonic>() != null && HP > 0) {
                GetComponent<_16MSonic>().RingSpread();
            }
        } else {
            HP = 0;
        }
        if (HP <= 0) {
            GameManager.falling = true;
            SoundPlay(deathSound);
            voices.Fallen();

            if (GetComponent<MegaManActions>() != null) {
                GetComponent<MegaManActions>().StartCoroutine("DeathAnimation");
            }

            StartCoroutine("LostLife");
        } else {
            SoundPlay(damageSound);
            this.transform.position = previousLandedPos;
        }
    }
    void Drown(){
        //溺れた
        SoundPlay(drownSound);
        
        if (GetComponent<MarioActions>() != null) {
            GetComponent<MarioActions>().StartCoroutine("DeathAnimation");
        } else if (GetComponent<PacManActions>() != null) {
            GetComponent<PacManActions>().StartCoroutine("DeathAnimation");
        } else if (GetComponent<MegaManActions>() != null) {
            GetComponent<MegaManActions>().StartCoroutine("DeathAnimation");
        } else if (GetComponent<SonicActions>() != null) {
            MusicManager.musicIndex = 2;
            GetComponent<SonicActions>().StartCoroutine("DrownAnimation");
        }

        HP = 0;
        StartCoroutine("LostLife");
    }

    bool areTheyPerished() {
        GameManager.players[playerNumber].setStatus(playerId, maxHP, HP, lives, localScore);

        foreach (PlayerStatus player in GameManager.players) {
            if (player.getStatus()[2] > 0) {
                return false;
            }
        }
        return true;
    }
    public void LookFront() {
        GameObject cam = Camera.main.gameObject;
        //カメラの前方向
        Vector3 lookDir = -cam.transform.forward;
        lookDir.y = 0;
        lookDir = lookDir.normalized;

        skin.transform.forward = lookDir;
    }

    public void GotCookie(int amounts) {
        cookiesAmount += amounts;
        while (cookiesAmount >= 50) {
            cookiesAmount -= 50;
            HPRestore(8);
        }
    }
    public void GotRing(int amounts) {
        ringsAmount += amounts;
        while (ringsAmount >= 2) {
            ringsAmount -= 2;
            HPRestore(1);
        }
    }
    public void GotShroom() {
        if (HP == maxHP) {
            if (data.stockItems[0].amounts == 1) {
                scoreIncrease(-50);
                scorePopUp(1000, false, this.transform.position);
            } else {
                StartCoroutine(ItemStock(0, 1));
                data.stockItems[0].amounts = 1;
            }
        }
    }
    public void GotRestoreWP() {
        if (this.GetComponent<MegaManActions>() == null) {
            scoreIncrease(-50);
            scorePopUp(1000, false, this.transform.position);
        } else {
            return;
        }
    }

    public void PowerUpAnim(int index) {
        if (powerUpActive == index) {
            if (powerUpActive == data.stockItems[2].amounts) {
                scoreIncrease(-50);
                scorePopUp(1000, false, this.transform.position);
            } else {
                StartCoroutine(ItemStock(2, index));
            }
        } else if (powerUpActive > 0) {
            StartCoroutine(ItemStock(2, powerUpActive));
            StartCoroutine(PowerUp(index));
        } else {
            StartCoroutine(PowerUp(index));
        }
    }
    IEnumerator PowerUp(int index) {
        Time.timeScale = 0f;
        SoundPlay(powerUpSound);

        for (int i = 0; i < 3; i++) {
            powerUpActive = index;
            yield return new WaitForSecondsRealtime(0.1f);
            powerUpActive = 0;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        Time.timeScale = 1;
        powerUpActive = index;
    }

    IEnumerator PowerDown(int index) {
        Time.timeScale = 0;
        SoundPlay(powerDownSound);

        for (int i = 0; i < 3; i++) {
            powerUpActive = 0;
            yield return new WaitForSecondsRealtime(0.1f);
            powerUpActive = index;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        Time.timeScale = 1;
        powerUpActive = 0;
    }

    public IEnumerator PipeEnter(Vector3 pipePos, int exitIndex) {
        activeCollision = false;
        activePhysics = false;
        GetComponent<CapsuleCollider>().isTrigger = true;
        canInput = false;
        Grounded = true;
        float keepY = pipePos.y;

        pipePos.y = this.transform.position.y;
        this.transform.position = pipePos;
        VelocitySetUp(Vector3.zero);

        SoundPlay(pipeSound);

        for (int i = 0; i < 64; i++) {
            yield return null;
            this.transform.position += Vector3.up * -0.1f;
        }

        yield return new WaitForSeconds(0.5f);

        float difference = this.transform.position.y - keepY;
        WarpPipe[] pipes = FindObjectsOfType<WarpPipe>();
        foreach (WarpPipe pipe in pipes) {
            if (pipe.exitIndex == exitIndex) {
                this.transform.position = pipe.gameObject.transform.position + (Vector3.up * difference);
                break;
            }
        }

        SoundPlay(pipeSound);

        for (int i = 0; i < 64; i++) {
            yield return null;
            this.transform.position += Vector3.up * 0.1f;
        }

        activeCollision = true;
        activePhysics = true;
        GetComponent<CapsuleCollider>().isTrigger = false;
        canInput = true;
    }
    public IEnumerator ItemStock(int stockIndex, int powerNo) {
        yield return new WaitForSeconds(0.3f);

        SoundPlay(stockSound);
        data.stockItems[stockIndex].amounts = powerNo;
    }
}
