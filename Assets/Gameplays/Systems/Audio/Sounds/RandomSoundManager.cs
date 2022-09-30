using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundManager : MonoBehaviour
{
    private AudioSource source;
    public AudioClip[] sounds;
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();

        int index = UnityEngine.Random.Range(0, sounds.Length);

        source.Stop();
        source.PlayOneShot(sounds[index]);
    }
}
