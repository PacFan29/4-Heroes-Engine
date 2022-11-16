using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _10ProtoMan : MegaManActions
{
    private float time = 0f;
    public GameObject charge1;
    public GameObject charge2;
    private GameObject[] actives = new GameObject[2];
    public GameObject buster;

    private bool sliding;

    [Header("効果音")]
    public AudioClip busterSound;
    public AudioClip mBusterSound;
    public AudioClip chargeShotSound;
    public AudioClip dashSound;
    public AudioClip fireSound;
    public AudioClip chargeFireSound;
    public AudioClip iceSound;
    public AudioClip chargeIceSound;
    public AudioClip thunderSound;
    public AudioClip chargeThunderSound;

    void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        ブルースでしかできない技
        ・攻撃力5のブルースストライク（チャージショット）
        ・ダッシュ
        */
        if (info != null){
            //プレイヤーIDを10に設定
            info.setPlayerId(10);
        }

        if (info.ButtonsDown["X"] && (weaponId == 0 || weaponId >= 9)){
            switch (info.powerUpActive) {
                case 1:
                info.SoundPlay(fireSound);
                break;
                
                case 2:
                info.SoundPlay(iceSound);
                break;
                
                case 3:
                info.SoundPlay(thunderSound);
                break;
                
                default:
                info.SoundPlay(busterSound);
                break;
            }

            BusterShot(0);
        }

        if (info.Buttons["X"] && weaponId == 0) {
            if (time > 0 && time < 1.5){
                time += Time.deltaTime;
                if (time > 0.5 && actives[0] == null) {
                    actives[0] = Instantiate(charge1, transform.position, Quaternion.identity, this.transform);
                } else if (time >= 1.5 && time < 2){
                    time = 1.7f;
                    Destroy(actives[0]);
                    actives[1] = Instantiate(charge2, transform.position, Quaternion.identity, this.transform);
                }
            } else if (time <= 0){
                time = Time.deltaTime;
            }
        }
        
        if (!info.Buttons["X"] && time > 0){
            Destroy(actives[0]);
            Destroy(actives[1]);
            if (time >= 1.5) {
                voices.Attack();
                
                switch (info.powerUpActive) {
                    case 1:
                    info.SoundPlay(chargeFireSound);
                    break;
                    
                    case 2:
                    info.SoundPlay(chargeIceSound);
                    break;
                    
                    case 3:
                    info.SoundPlay(chargeThunderSound);
                    break;
                }
                info.SoundPlay(chargeShotSound);
                BusterShot(3);
            } else if (time >= 0.5) {
                switch (info.powerUpActive) {
                    case 1:
                    info.SoundPlay(fireSound);
                    break;
                    
                    case 2:
                    info.SoundPlay(iceSound);
                    break;
                    
                    case 3:
                    info.SoundPlay(thunderSound);
                    break;
                }
                info.SoundPlay(mBusterSound);
                BusterShot(1);
            }
            time = 0;
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
                if (info.isNarrow()) {
                    info.ForwardSetUp(-XZvel.normalized, info.TopSpeed*2);
                } else {
                    info.ForwardSetUp(-XZvel.normalized, 5f);
                    sliding = false;
                    info.constantSetUp();
                    info.ForwardSetUp(Vector3.zero, 0);
                    info.axisInput = true;

                    StopCoroutine("Dash");
                }
            }
            if (!info.Grounded) {
                info.axisInput = true;
                info.Crouching = false;
                sliding = false;
                StopCoroutine("Dash");
                info.constantSetUp();
                if (info.input != Vector3.zero) info.ForwardSetUp(Vector3.zero, info.TopSpeed);
            }
        }
    }

    IEnumerator Dash() {
        sliding = true;
        info.ForwardSetUp(Vector3.zero, 40f);
        info.constantChange(true, "frc", 0);
        info.constantChange(true, "top", 40f);

        yield return new WaitForSeconds(0.4342f);
        
        while (info.isNarrow()) {
            yield return new WaitForSeconds(0f);
        }

        sliding = false;
        info.constantSetUp();
        info.ForwardSetUp(Vector3.zero, 0);
        info.axisInput = true;
    }

    void BusterShot(int level) {
        GameObject solarbrit = Instantiate(buster, transform.position, info.skin.rotation);
        MegaBuster bust = solarbrit.GetComponent<MegaBuster>();
        bust.player = GetComponent<PlayerInfo>();
        bust.level = level;
        
        bust.power = 1f;
        bust.powerUp = info.powerUpActive;
    }
}
