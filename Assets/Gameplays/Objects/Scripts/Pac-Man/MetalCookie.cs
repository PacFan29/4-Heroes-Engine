using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalCookie : MonoBehaviour
{
    public GameObject skin;
    private bool isMetal = false;
    private PlayerInfo player;

    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            skin.SetActive(!player.metal);
        }
        if (isMetal && !player.metal) {
            isMetal = false;
        }
    }
    public void OnTriggerStay (Collider col) {
        if (col.gameObject.GetComponent<PlayerInfo>() != null && !isMetal) {
            player = col.gameObject.GetComponent<PlayerInfo>();
            player.MetalCookie();
            this.GetComponent<AudioSource>().Play();
            isMetal = true;
        }
    }
}
