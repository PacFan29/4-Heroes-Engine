using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostType {
    Blinky,
    Pinky,
    Inky,
    Clyde,
}

public class MazeGhost : MazeCharacter
{
    private Transform targetPlayer;

    void Update()
    {
        if (targetPlayer == null) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players) {
                if (player.GetComponent<MazePlayer>() != null) {
                    targetPlayer = player.transform;
                }
            }
        }

        float minDis = Mathf.Infinity;
        for (int i = 0; i < dirs.Length; i++) {
            float distance = playerDistance(dirs[i]);
            if (distance < minDis) {
                minDis = distance;
            }
        }
    }

    float playerDistance(Vector2 offset) {
        Vector3 start = this.transform.position + (new Vector3(offset.x, 0, offset.y) * tilePixel);
        return Vector3.Distance(targetPlayer.position, start);
    }
}
