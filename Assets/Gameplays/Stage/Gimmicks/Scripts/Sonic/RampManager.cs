using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampManager : MonoBehaviour
{
    [Header("トリック")]
    public bool canTrick = false;
    public int[] buttonCounts;
    public float[] timeLimits;
    [Header("効果音")]
    public AudioClip jumpBoard;
    private AudioSource source;
    bool trick;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay(Collision other) {
        PlayerInfo player = other.gameObject.GetComponent<PlayerInfo>();
        if (player != null && !trick) {
            //Debug.Log(this.transform.rotation.eulerAngles + new Vector3(45, 0, 0));
            //Debug.Log(Quaternion.Euler(this.transform.rotation.eulerAngles + new Vector3(45, 0, 0)));
            _16MSonic sonic = player.GetComponent<_16MSonic>();

            player.ForwardSetUp(-this.transform.forward, 80f);

            if (sonic != null && canTrick) {
                sonic.trickManager.StartCoroutine("Command");
                
                sonic.trickManager.buttonCounts = new int[buttonCounts.Length];
                sonic.trickManager.timeLimits = new float[buttonCounts.Length];

                for (int i = 0; i < buttonCounts.Length; i++) {
                    if (timeLimits.Length < (i+1)) {
                        break;
                    } else {
                        sonic.trickManager.buttonCounts[i] = buttonCounts[i];
                        sonic.trickManager.timeLimits[i] = timeLimits[i];
                    }
                }
            }
            source.PlayOneShot(jumpBoard);
            trick = true;
        }
    }
    void OnCollisionExit(Collision other) {
        PlayerInfo player = other.gameObject.GetComponent<PlayerInfo>();
        if (player != null && trick) {
            trick = false;
        }
    }
}
