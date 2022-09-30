using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowSwitch : PacManSwitch
{
    [Header("クッキーグループ")]
    public GameObject cookiesGroup;
    [Header("アニメーション")]
    public AudioClip split;

    void Start() {
        cookiesGroup.SetActive(false);
    }
    
    public override IEnumerator Gimmick() {
        this.GetComponent<AudioSource>().PlayOneShot(split, 1f);
        cookiesGroup.SetActive(true);
        yield return null;
    }
}
