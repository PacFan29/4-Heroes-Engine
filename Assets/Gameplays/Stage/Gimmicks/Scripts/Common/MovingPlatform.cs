using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // angle += 1.0;
        // angle %= 360;

        // float xPlus = (float)Math.Cos(angle * Mathf.Deg2Rad) * 5f;
        // float yPlus = (float)Math.Sin(angle * Mathf.Deg2Rad) * 20f;

        // this.transform.parent.position = new Vector3(pos.x + xPlus, pos.y + yPlus, pos.z + xPlus);
    }
    void Update() {
        ;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            other.gameObject.transform.parent = transform.parent;
        }
    }
    void OnCollisionExit(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            other.gameObject.transform.parent = null;
        }
    }
}
