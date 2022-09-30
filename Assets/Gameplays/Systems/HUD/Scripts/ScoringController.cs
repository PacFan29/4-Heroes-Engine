using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringController : MonoBehaviour
{
    public int points;
    public int rate = 1;
    public GameObject textParent;
    private int previousPts = 0;
    public bool active;
    
    void Awake() {
        ;
    }
    void Update(){
        if (active) {
            if (rate < 1) rate = 1;
            float size = Math.Min(5.5f, (points * rate / 10000f) + 0.5f);
            if (points * rate > 10000) {
                size = Math.Min(2.5f, (points * rate / 90000f) + 1.5f);
            }
            this.transform.localScale = new Vector3 (size, size, 1f);

            if (points <= 0){
                destroyObject();
            } else {
                if (points > 0 && previousPts <= 0) {
                    previousPts = points;
                    GetChildren(textParent);
                }
            }
        }
        //スケール 0.51~1.50

        this.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
    }
    public void destroyObject() {
        Destroy(gameObject);
    }
    void GetChildren(GameObject obj) {
        Transform children = obj.GetComponentInChildren<Transform>();
        //子要素がいなければ終了
        if (children.childCount == 0) {
            return;
        }
        foreach(Transform ob in children) {
            //ここに何かしらの処理
            //例　ボーンについてる武器を取得する
            string ptsStr = points.ToString();
            if (rate > 1) {
                ptsStr += "×";
                ptsStr += rate.ToString();
            }
            ob.gameObject.GetComponent<TextMesh>().text = ptsStr;
            GetChildren(ob.gameObject);
        }
    }
}
