using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperiment : MonoBehaviour
{
    //ボールが当たった物体の法線ベクトル
    private Vector3 objNomalVector = Vector3.zero;
    // ボールのrigidbody
    private Rigidbody rb;
    // 跳ね返った後のverocity
    [HideInInspector] public Vector3 afterReflectVero = Vector3.zero;

    public void OnCollisionEnter (Collision collision) {
        // 当たった物体の法線ベクトルを取得
        objNomalVector = collision.contacts[0].normal;
        Vector3 reflectVec = Vector3.Reflect (afterReflectVero, objNomalVector);
        rb.velocity = reflectVec;
        // 計算した反射ベクトルを保存
        afterReflectVero = rb.velocity;
        Debug.Log ("normal:" + objNomalVector);
    }

    // Use this for initialization
    void Start () {
        rb = this.GetComponent<Rigidbody> ();
        rb.velocity = transform.forward * 30f;
        afterReflectVero = rb.velocity;
    }
}
