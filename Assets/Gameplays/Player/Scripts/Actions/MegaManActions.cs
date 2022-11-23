using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MegaManActions : MonoBehaviour
{
    public SpecialWeaponData weapons;
    public int weaponId = 0;
    protected PlayerInfo info;

    protected bool inputA;
    protected bool canJump = true;
    [Header("エフェクト")]
    public GameObject deathEffect;

    protected PlayerVoiceManager voices;

    private float beforeAxis = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        info = GetComponent<PlayerInfo>();
        info.Stompable = false;
        info.playerType = 2;
        info.desacelerar = true;

        voices = GetComponent<PlayerVoiceManager>();
    }

    // Update is called once per frame
    public void actions()
    {
        float view_axis;
        bool judge;
        (view_axis, judge) = AxisOnce("LT/RT");

        if (view_axis > 0 && judge){
            SwitchWeaponR();
        } else if (view_axis < 0 && judge){
            SwitchWeaponL();
        }

        if (info.ButtonsDown["A"] && canJump && info.Grounded) {
            info.constantSetUp();
            if (this.GetComponent<_08MegaMan>() != null && info.XZmag > 30f) {
                info.ForwardSetUp(info.skin.forward, 0f);
            }
            info.Crouching = false;
            inputA = true;
            info.Jump();
        }
        if (info.Crouching) {
            this.GetComponent<CapsuleCollider>().height = 2.4f;
        } else {
            this.GetComponent<CapsuleCollider>().height = 3.5f;
        }
        
        if (inputA && !info.gravityLock){
            //ジャンプの勢い
            //Aを押し続けるとより高く跳べる。
            if (info.finalVelocity.y > 0 && info.ButtonsUp["A"]){
                //途中でAを離したとき
                info.YvelSetUp(0);
                inputA = false;
            }
            if (info.finalVelocity.y <= -0.1){
                //高く跳びきったとき
                inputA = false;
            }
        }
    }

    public IEnumerator DamageAnimation(Vector3 direction) {
        info.canInput = false;
        float time = 0.3f;
        Vector3 damageVelocity = direction.normalized * 30f;
        damageVelocity.y = 0;
        
        while (time > 0) {
            info.VelocitySetUp(damageVelocity);
            info.skin.transform.forward = -damageVelocity.normalized;
            time -= Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        info.VelocitySetUp(Vector3.zero);
        yield return new WaitForSeconds(0.7f);

        info.canInput = true;
        info.tookDamage = false;
        yield return null;
    }
    public IEnumerator DeathAnimation() {
        info.activePhysics = false;
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.7f);

        Time.timeScale = 1;
        info.StopAllSounds();
        info.skinGroup.SetActive(false);
        GetComponent<CapsuleCollider>().isTrigger = true;
        GameObject d_effect = Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (GameManager.is3D()) {
            //3Dの場合、X軸に90°回転する。
            d_effect.transform.rotation = Quaternion.Euler(90, 0, 0);
        } else if (GameManager.dimension == DimensionType.ZWay2D) {
            d_effect.transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        voices.StartCoroutine(voices.VoiceEcho(voices.DeathVoices));
    }

    (float a, bool b) AxisOnce(string axis){
        bool judge = false;
        float view_axis = Input.GetAxis(axis);

        //RTが押されたら視点を変える：前フレームの入力値が0の場合のみ実施
        judge = Math.Abs(view_axis) > 0 && beforeAxis == 0.0f;
        beforeAxis = view_axis;

        return (view_axis, judge);
    }

    public void SwitchWeaponL() {
        foreach (SpecialWeapon wp in weapons.weapons) {
            if (wp.Enable) {
                weaponId--;
                if (weaponId < 0) weaponId = weapons.weapons.Length;

                if (weaponId != 0) {
                    while (!weapons.weapons[weaponId - 1].Enable && weaponId > 1) {
                        weaponId--;
                    }
                    if (weaponId == 1 && !weapons.weapons[0].Enable) {
                        weaponId = 0;
                    }
                }

                return;
            }
        }
    }
    public void SwitchWeaponR() {
        foreach (SpecialWeapon wp in weapons.weapons) {
            if (wp.Enable) {
                weaponId++;
                if (weaponId > weapons.weapons.Length) weaponId = 0;
                
                if (weaponId != 0)  {
                    while (!weapons.weapons[weaponId - 1].Enable && weaponId < weapons.weapons.Length) {
                        weaponId++;
                        if (weaponId == weapons.weapons.Length && !weapons.weapons[weaponId - 1].Enable) {
                            weaponId = 0;
                            break;
                        }
                    }
                }

                return;
            }
        }
    }

    public void Reset() {
        info.activePhysics = true;
        //actionId = 0;
        GetComponent<CapsuleCollider>().isTrigger = false;
        info.skinGroup.SetActive(true);
    }

    public void WPRestore(int value) {
        if (weaponId == 0 || weaponId >= 9) {
            info.scoreIncrease(-50);
            info.scorePopUp(1000, false, this.transform.position);
        } else {
            if (weapons.weapons[weaponId - 1].Energy >= 28) {
                info.scoreIncrease(-50);
                info.scorePopUp(1000, false, this.transform.position);
            } else {
                weapons.weapons[weaponId - 1].Restore(value);
            }
        }
    }
}
