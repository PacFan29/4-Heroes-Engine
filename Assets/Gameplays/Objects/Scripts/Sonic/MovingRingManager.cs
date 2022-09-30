using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRingManager : MonoBehaviour
{
    public Transform shape;
    public float angle;
    public float speed; //１重目：20f、２重目：10f
    public int amount = 1;
    public bool hyper = false;
    public GameObject ringEffect;
    public AudioClip ringSound;
    public AudioClip superRingSound;

    private Rigidbody rb;
    private Vector3 velocity;
    private float time = 5.2f;
    private bool touched = true;
    private float radius;

    //ボールが当たった物体の法線ベクトル
    private Vector3 objNormalVector = Vector3.zero;
    // 跳ね返った後のvelocity
    private Vector3 afterReflectVelo = Vector3.zero;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void setSpeed()
    {
        float cosRad = (float)Math.Cos(angle * Mathf.Deg2Rad); //X・Z方向に使用
        float sinRad = (float)Math.Sin(angle * Mathf.Deg2Rad); //Y方向に使用

        switch(GameManager.dimension) {
            case DimensionType.Normal3D:
            //3D
            velocity.x = sinRad * speed;
            velocity.y = 6f;
            velocity.z = cosRad * speed;
            break;

            case DimensionType.XWay2D:
            //X方向2D
            velocity.x = sinRad * speed;
            velocity.y = cosRad * speed;
            break;
            
            case DimensionType.ZWay2D:
            //Z方向2D
            velocity.y = cosRad * speed;
            velocity.z = sinRad * speed;
            break;
        }

        afterReflectVelo = velocity;
        afterReflectVelo.y = 0;
    }
    void FixedUpdate()
    {
        shape.Rotate(0f, -6f, 0f, Space.Self);
        
        time -= Time.deltaTime;
        if (time <= 0) Destroy(gameObject);

        velocity.y -= 0.46875f;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Gravity : 0.46875f
        Ground Bounce : Yvel * 0.75f
        Time : 256 steps → ?? seconds
        */
        rb.velocity = velocity;

        this.GetComponent<SphereCollider>().radius = hyper ? 1.5f : 0.6f;
        radius = this.GetComponent<SphereCollider>().radius;
        if (hyper) {
            shape.gameObject.GetComponent<Animator>().Play("HyperRing");
        } else {
            shape.gameObject.GetComponent<Animator>().Play("RingStatic");
        }
    }
    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player" && !touched){
            AudioSource playerGotit = col.transform.parent.gameObject.GetComponent<AudioSource>();
            playerGotit.clip = hyper ? superRingSound : ringSound;
            playerGotit.volume = 0.7f;
            playerGotit.Play();

            PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();
            Instantiate(ringEffect, this.transform.position, Quaternion.identity);
            GameManager.Coins += amount;
            player.scoreIncrease(amount);
            Destroy(gameObject);
        }
    }
    void OnTriggerStay(Collider col) {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Default") {
            //跳ね返る
            RaycastHit hit;
            Vector3 XZvel = new Vector3(velocity.x, 0f, velocity.z);

            bool groundHit = Physics.Raycast(transform.position, -Vector3.up, out hit, Math.Abs(velocity.y) * radius * 20f);
            bool wallHit = Physics.Raycast(transform.position, XZvel, out hit, radius * 5f);

            if (groundHit && velocity.y <= 0) {
                velocity.x *= 0.9f;
                velocity.y *= -0.75f;
                velocity.z *= 0.9f;
            }
            if (wallHit) {
                objNormalVector = hit.normal;
                Vector3 reflectVec = Vector3.Reflect (afterReflectVelo, objNormalVector);
                velocity.x = reflectVec.x;
                velocity.z = reflectVec.z;
                // 計算した反射ベクトルを保存
                afterReflectVelo = velocity;
                afterReflectVelo.y = 0;
            }
        }
    }
    void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Player" && touched){
            touched = false;
        }
    }
}