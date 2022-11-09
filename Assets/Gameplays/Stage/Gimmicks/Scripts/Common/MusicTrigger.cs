using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public AudioClip music;
	public float loopBegin = 0f;
	public float loopEnd = 999999f;

    private MusicManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindObjectOfType<MusicManager>();

        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            PlayerInfo player = other.gameObject.GetComponent<PlayerInfo>();

            if (player.playerNumber == 0) {
                manager.ChangeMusic(music, loopBegin, loopEnd);
            }
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            PlayerInfo player = other.gameObject.GetComponent<PlayerInfo>();

            if (player.playerNumber == 0) {
                manager.ReturnMusic();
            }
        }
    }
}
