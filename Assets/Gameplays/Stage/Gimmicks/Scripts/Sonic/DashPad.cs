using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPad : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && col.gameObject.GetComponent<PlayerInfo>().Grounded){
            this.GetComponent<AudioSource>().Play();

            PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();
            player.ForwardSetUp(this.transform.forward, 80f);
            //player.controlLockTimer = 0.5f;
        }
    }
}
