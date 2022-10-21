using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("設定")]
    public bool freezePosition = false;
    public bool freezeRotation = false;
    [Header("スキン")]
    public GameObject skin;
    public GameObject boxBreak;
    [Header("効果音")]
    public AudioClip[] breakedSounds = new AudioClip[2];

    private AudioSource source;
    private Rigidbody rb;
    [HideInInspector] public bool breaked = false;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.None;

        if (freezePosition) {
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if (freezeRotation) {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public IEnumerator DestroyBox(PlayerInfo player) {
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        rb.useGravity = false;

        skin.SetActive(false);
        boxBreak.SetActive(true);
        
        int index = UnityEngine.Random.Range(0, breakedSounds.Length);
        source.PlayOneShot(breakedSounds[index]);
        player.scoreIncrease(10);

        this.GetComponent<BoxCollider>().isTrigger = true;
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        this.gameObject.tag = "Other";
        
        breaked = true;

        yield return new WaitForSeconds(1.1f);

        Destroy(gameObject);
    }
}
