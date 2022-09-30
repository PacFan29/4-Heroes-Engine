using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterManager : MonoBehaviour
{
    public GameObject splashEffect;
    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            PlayerInfo player = other.GetComponent<PlayerInfo>();
            player.underwater = (player.transform.position.y < this.transform.position.y);
        }
    }
}
