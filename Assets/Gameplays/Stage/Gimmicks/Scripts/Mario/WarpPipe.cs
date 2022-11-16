using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpPipe : MonoBehaviour
{
    private WarpPipe targetPipe;
    public bool changeMusic = false;
    public int enterIndex = -1;
    public int exitIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        WarpPipe[] allPipes = FindObjectsOfType<WarpPipe>();

        foreach (WarpPipe pipe in allPipes) {
            if (pipe.exitIndex == enterIndex) {
                targetPipe = pipe;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay(Collision col) {
        if (col.gameObject.tag == "Player") {
            PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();

            if (Input.GetButtonDown("RB") && player.activeCollision && enterIndex != -1) {
                if (targetPipe.changeMusic || (changeMusic && !targetPipe.changeMusic)) {
                    MusicManager.musicFade = true;
                }
                StartCoroutine(player.PipeEnter(this.transform.position, enterIndex));
            }
        }
    }
}
