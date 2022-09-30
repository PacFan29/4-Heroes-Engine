using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingSoundManager : MonoBehaviour
{
    private AudioSource source;
    private float LoopStart = 0f;
    private float LoopEnd = 0f;
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (source.isPlaying) {
            if (source.time > LoopEnd && LoopEnd > 0){
                source.time = LoopStart;
                source.Play();
            }
        }
    }

    public void SetUp(AudioClip clip, float lStart, float lEnd) {
        source.clip = clip;
        source.loop = true;

        LoopStart = lStart;
        LoopEnd = lEnd;
        
        source.Play();
    }
    public void Stop() {
        source.clip = null;
        source.loop = false;
        source.Stop();
    }
}
