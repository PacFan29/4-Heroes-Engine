using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaShield : MonoBehaviour
{
    public PlayerInfo player;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<QuestionBlockManager>() != null) {
            other.gameObject.GetComponent<QuestionBlockManager>().BlockHit(player, false);
        }
    }
}
