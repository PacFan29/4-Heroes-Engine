using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _01Koopa : EnemyMovements
{
    [Header("アニメーション")]
    public Animator nakedAnim;

    [Header("スキン")]
    public GameObject normalSkin;
    public GameObject nakedSkin;

    [Header("甲羅")]
    public GameObject shell;
    
    void FixedUpdate()
    {
        bool defended = this.GetComponent<EnemyManager>().defended;
        normalSkin.SetActive(defended);
        nakedSkin.SetActive(!defended);
    }

    public override void Movements()
    {
        ;
    }

    public override void DamageDefended() {
        Instantiate(shell, this.transform.position, this.GetComponent<EnemyManager>().skin.transform.rotation);
        this.transform.position += this.GetComponent<EnemyManager>().skin.transform.forward * 2f;
        this.GetComponent<EnemyManager>().defended = false;
    }
}
