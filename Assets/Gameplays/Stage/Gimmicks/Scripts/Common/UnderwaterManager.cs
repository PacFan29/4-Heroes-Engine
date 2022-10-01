using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterManager : MonoBehaviour
{
    public GameObject splashEffect;
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            PlayerInfo player = other.GetComponent<PlayerInfo>();
            player.underwaterTrigger = true;
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            PlayerInfo player = other.GetComponent<PlayerInfo>();
            player.underwaterTrigger = false;
        }
    }
}
