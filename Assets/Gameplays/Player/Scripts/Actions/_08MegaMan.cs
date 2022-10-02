using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _08MegaMan : MegaManActions
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
    public AudioClip slidingSound;

    public void Update()
    {
        //共通アクションの実行
        actions();
        
        /*
        ロックマンでしかできない技
        ・攻撃力3のチャージショット
        ・スライディング
        ・ダブルギアシステム
        */
        if (info != null){
            //プレイヤーIDを8に設定
            info.setPlayerId(8);
        }

        /*
        ～時間間隔・攻撃力～
        チャージショット（通常）：1秒（攻撃力 3）
        チャージショット（パワーギア）：0.5秒（攻撃力 3+5）
        チャージショット（フルパワー）：4秒（満タンになったときはゲージをMAXにする、攻撃力 15）
        チャージショット（フルパワー、ダブルギア）：1秒（攻撃力 15）

        スピードギア：5秒（周囲のタイムレート：??%、プレイヤーのタイムレート：1??%）
        パワーギア：5秒
        ダブルギア：6秒（途中解除不可）
        クールダウン（通常）：7秒500（チャージショット不可能）
        クールダウン（ダブルギア）：20秒（バスター一弾しか撃てない）
        */
        if (info.ButtonsDown["X"] && (weaponId == 0 || weaponId >= 9)){
            info.SoundPlay(busterSound);

            BusterShot(0, false);
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
                
                info.SoundPlay(chargeShotSound);
                BusterShot(2, false);
            } else if (time >= 0.5) {
                info.SoundPlay(mBusterSound);
                BusterShot(1, false);
            }
            time = 0;
        }

        canJump = !info.GetCrouchButton("RB");

        if (!canJump && info.ButtonsDown["A"] && !sliding && info.Grounded) {
            //info.axisInput = false;
            info.SoundPlay(slidingSound);
            StartCoroutine("Sliding");
        }
        if (sliding) {
            info.dustTrailEffect();

            Vector3 XZvel = new Vector3(info.finalVelocity.x, 0, info.finalVelocity.z);
            info.skin.transform.forward = XZvel.normalized;
            
            if (info.isNarrow() && Vector3.Angle(XZvel, info.input) >= 130) {
                info.ForwardSetUp(-XZvel.normalized, info.TopSpeed*2);
            }
        }

        if (sliding && !info.Grounded) {
            //info.axisInput = true;
            info.Crouching = false;
            sliding = false;
            StopCoroutine("Sliding");
            info.constantSetUp();
            if (info.input != Vector3.zero) info.ForwardSetUp(Vector3.zero, info.TopSpeed);
        }
        
        // if (info.ButtonsDown["X"]){
        //     info.ComboReset();
        //     StartCoroutine("ForteShoot");
        // } else if (info.ButtonsUp["X"]) {
        //     StopCoroutine("ForteShoot");
        // }
    }

    IEnumerator ForteShoot() {
        while (info.Buttons["X"]) {
            info.SoundPlay(busterSound);

            BusterShot(0, true);
            yield return new WaitForSeconds(0.075f);
        }
    }

    IEnumerator Sliding() {
        sliding = true;
        info.Crouching = true;
        info.ForwardSetUp(Vector3.zero, 40f);
        info.constantChange(true, "frc", 0);
        info.constantChange(true, "top", 40f);

        yield return new WaitForSeconds(0.4342f);
        
        while (info.isNarrow()) {
            yield return new WaitForSeconds(0f);
        }

        sliding = false;
        info.Crouching = false;
        info.constantSetUp();
        info.ForwardSetUp(Vector3.zero, 0);
        info.axisInput = true;
    }

    void BusterShot(int level, bool weak) {
        GameObject solarbrit = Instantiate(buster, transform.position, info.skin.rotation);
        MegaBuster bust = solarbrit.GetComponent<MegaBuster>();
        bust.player = GetComponent<PlayerInfo>();
        bust.level = level;

        if (weak) {
            bust.power = 0.5f;
        } else {
            bust.power = 1f;
        }
        bust.powerUp = info.powerUpActive;
    }
}
