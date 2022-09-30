using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    public GameObject[] shields = new GameObject[4];
    private int shieldActive = 0; //0＝なし、1＝バリア、2＝フレイム、3＝アクア、4＝サンダー
    
    void Update()
    {
        shieldActive = transform.parent.gameObject.GetComponent<PlayerInfo>().shieldActive;

        for (int i = 0; i < shields.Length; i++) {
            //バリアの表示・非表示
            shields[i].SetActive(shieldActive == (i + 1));
        }
    }
}
