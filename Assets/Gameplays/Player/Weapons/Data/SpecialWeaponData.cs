using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponTypes {
    Fire, Ice, Thunder,
    Laser, Cutter, Water,
    Needle, Bomb, Stone, Dash
}
[Serializable]
public class SpecialWeapon
{
    public bool Enable = false; //使用可能か
    public string Name = "武器名"; //武器名
    public Color WeaponColor = Color.red; //武器のカラー
    public WeaponTypes Type; //属性
    public int Energy = 28; //残りエネルギー
    public int Consumption = 1; //消費エネルギー
    public GameObject prefab;

    public bool isConsumable() {
        return Energy >= Consumption;
    }
    public void Restore(int amount) {
        Energy += amount;
        if (Energy > 28) Energy = 28;
    }
    public void Consume() {
        Energy -= Consumption;
        if (Energy < 0) Energy = 0;
    }
}
[CreateAssetMenu(fileName = "SpecialWeaponData", menuName = "ScriptableObjects/Special Weapon Data")]
public class SpecialWeaponData : ScriptableObject
{
    /*
    特殊武器0：ワイドレーザー
    特殊武器1：スライスキャッチャー
    特殊武器2：ウォータージェット
    特殊武器3：スピア
    特殊武器4：スプレッドダイナマイト
    特殊武器5：ボルダーウォール
    特殊武器6：フレアバースト
    特殊武器7：フラッシュダッシュ 
    */
    public SpecialWeapon[] weapons = new SpecialWeapon[8];
}
