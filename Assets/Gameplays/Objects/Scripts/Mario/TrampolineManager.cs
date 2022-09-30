using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineManager : MonoBehaviour
{
    private AudioSource audioS;
    public AudioClip bounceSound;
    public AudioClip bounceHighSound;
    // Start is called before the first frame update
    void Start()
    {
        audioS = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player"){
            float force = 30f;
            if (Input.GetButton("A")) {
                force = 60f;
                audioS.PlayOneShot(bounceHighSound);
            } else {
                audioS.PlayOneShot(bounceSound);
            }
            col.gameObject.GetComponent<PlayerInfo>().JumpFromSprings(force, transform.up, 0);
        }
    }
}
