using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class GrindRail : MonoBehaviour
{
    public PathCreator rail;
    bool railTrigger = false;

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null && !railTrigger) {
            PlayerInfo player = other.gameObject.GetComponent<PlayerInfo>();
            player.GrindRailStart(rail);

            railTrigger = true;
        }
    }

    void OnCollisionExit(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null && railTrigger) {
            StartCoroutine("EndRail");
        }
    }
    IEnumerator EndRail() {
        yield return new WaitForSeconds(0.1f);

        railTrigger = false;
    }
}
