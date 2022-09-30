using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    List<PlayerInfo> playerList = new List<PlayerInfo>();
    //private double angle;
    //private Vector3 pos;
    //private float yPos;
    // Start is called before the first frame update
    void Start()
    {
        // pos = this.transform.parent.position;
        // yPos = this.transform.parent.position.y;
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
        // if (playerList.Count > 0) {
        //     foreach (PlayerInfo player in playerList) {
        //         if (!player.Grounded) {
        //             playerList.Remove(player);
        //             player.gameObject.transform.parent = null;
        //             return;
        //         }
        //     }
        // }
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            other.gameObject.transform.parent = transform.parent;
            //playerList.Add(other.gameObject.GetComponent<PlayerInfo>());
        }
    }
    void OnCollisionExit(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            other.gameObject.transform.parent = null;
        }
    }
}
