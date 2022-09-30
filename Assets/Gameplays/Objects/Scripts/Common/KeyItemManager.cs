using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItemManager : MonoBehaviour
{
    public GameData data;
    private bool gotIt;
    private Animator animator;

    [Header("スキン")]
    public Transform group;
    public GameObject[] skins = new GameObject[4];
    public GameObject[] pacManSkins = new GameObject[13];

    [Header("エフェクト")]
    public ParticleSystem effect;
    public GameObject gotEffect;
    [Header("効果音")]
    public AudioClip[] gotSounds = new AudioClip[4];
    private AudioSource source;
    public int index;
    private int skinIndex;
    private float volume;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        switch (data.character) {
            case Character.Mario:
            skinIndex = 0;
            volume = 1f;
            break;

            case Character.PacMan:
            skinIndex = 1;
            volume = 0.5f;
            break;

            case Character.RockMan:
            skinIndex = 2;
            volume = 1f;
            break;

            case Character.Sonic:
            skinIndex = 3;
            volume = 0.5f;
            break;
        }

        //Debug.Log(this.GetInstanceID());
    }
    void FixedUpdate()
    {
        float rotateSpeed = gotIt ? 30f : 3f;
        group.Rotate(0f, -rotateSpeed, 0f, Space.Self);

        foreach (GameObject obj in skins) {
            obj.SetActive(false);
        }
        skins[skinIndex].SetActive(true);

        foreach (GameObject pacman in pacManSkins) {
            pacman.SetActive(false);
        }
        if (data.greenStars.Length == 7) {
            pacManSkins[index + 6].SetActive(true);
        } else {
            pacManSkins[index].SetActive(true);
        }

        animator.SetBool("GotIt", gotIt);
    }

    void DestroyObj(){
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player" && !gotIt) {
            source.clip = gotSounds[skinIndex];
            source.volume = volume;
            source.Play();

            PlayerInfo player = other.GetComponent<PlayerInfo>();
            gotIt = true;
            player.scorePopUp(2000, false, this.transform.position);

            var emi = effect.emission;
            emi.rateOverTime = 0f;
            gotEffect.SetActive(true);

            GameManager.keyItems[index] = true;
        }
    }
}
