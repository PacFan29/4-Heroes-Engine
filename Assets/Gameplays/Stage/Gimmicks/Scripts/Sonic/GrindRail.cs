using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class GrindRail : MonoBehaviour
{
    public PathCreator rail;

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            PlayerInfo player = other.gameObject.GetComponent<PlayerInfo>();
            player.GrindRailStart(rail);
        }
    }
}
