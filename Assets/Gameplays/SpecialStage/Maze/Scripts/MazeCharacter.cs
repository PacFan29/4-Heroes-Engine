using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCharacter : MonoBehaviour
{
    private Rigidbody rb;
    protected const float tilePixel = 6f;
    protected Vector2 direction = Vector2.left;
    private Vector3 velocity;
    private float speed = 22.5f;
    protected Vector2[] dirs = {Vector2.up, Vector2.left, Vector2.down, Vector2.right};

    [Header("スキン")]
    public Transform skin;
    [Header("アニメーション")]
    public Animator playerAnim;
    // Start is called before the first frame update
    void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SpecialStageManager.ready) {
            velocity = new Vector3(direction.x, 0f, direction.y) * speed;
        } else {
            velocity = Vector3.zero;
        }
        if (velocity != Vector3.zero) {
            skin.forward = velocity.normalized;
            RaycastHit hitInfo;
            Physics.Raycast(this.transform.position, skin.forward, out hitInfo, tilePixel, getLayer());

            if ((hitInfo.distance - 3f) <= 0f && hitInfo.distance > 0f) {
                // Debug.Log(hitInfo.distance);
                this.transform.position += skin.forward * (hitInfo.distance - 3);
                velocity = Vector3.zero;
            }
        }
        rb.velocity = velocity;

        if (playerAnim != null) {
            playerAnim.SetFloat("Speed", velocity.magnitude);
            playerAnim.SetBool("Crouching", false);
            playerAnim.SetBool("Rolling", false);
            playerAnim.SetBool("Skidding", false);

            playerAnim.SetBool("Grounded", true);
            playerAnim.SetFloat("Y velocity", 0f);

            playerAnim.SetInteger("Actions", 0);
        }
    }
    
    public bool canRotate(Vector2 direction) {
        RaycastHit hitInfo;
        Physics.BoxCast(transform.position, Vector3.one * tilePixel * 0.5f, direction, out hitInfo, Quaternion.identity, tilePixel * 0.5f, getLayer());
        // Debug.Log(hitInfo.distance);
        return hitInfo.distance <= 0;
        // return true;
    }
    public void gridPos() {
        Vector3 pos = this.transform.position;
        pos.x = (float)Math.Round(pos.x / tilePixel);
        pos.z = (float)Math.Round(pos.z / tilePixel);

        pos.x *= tilePixel;
        pos.z *= tilePixel;

        this.transform.position = pos;
    }

    int getLayer() {
        string[] MaskNameList = new string[] {
            LayerMask.LayerToName(0)
        };
        int mask = LayerMask.GetMask(MaskNameList);
        return mask;
    }
}
