using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void VoicePlay(AudioClip voiceClip) {
        if (voiceClip != null) {
            // int voiceIndex = UnityEngine.Random.Range(0, voiceClips.Length);

            //効果音の再生
            // source.clip = voiceClips[voiceIndex];
            source.clip = voiceClip;
            source.Play();
        }
    }
}
