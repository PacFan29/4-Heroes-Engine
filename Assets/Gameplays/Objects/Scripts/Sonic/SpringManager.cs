using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringManager : MonoBehaviour
{
    private AudioSource audioS;
    public float force;
    public float lockTime;
    public AudioClip springSound;
    // Start is called before the first frame update
    void Start()
    {
        audioS = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player"){
            audioS.PlayOneShot(springSound);
            col.gameObject.GetComponent<PlayerInfo>().JumpFromSprings(force, transform.up, lockTime);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player"){
            audioS.PlayOneShot(springSound);
            col.gameObject.GetComponent<PlayerInfo>().JumpFromSprings(force, transform.up, lockTime);
        }
    }
}
