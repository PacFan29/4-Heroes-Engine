using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVoiceManager : MonoBehaviour
{
    public AudioClip[] JumpVoices;
    public AudioClip[] AttackVoices;
    public AudioClip[] ClearVoices;
    public AudioClip[] HurtVoices;
    public AudioClip[] DeathVoices;
    public AudioClip[] FallenVoices;
    public AudioClip[][] OtherVoices;

    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Jump(){
        VoicePlay(JumpVoices);
    }
    public void Attack(){
        VoicePlay(AttackVoices);
    }
    public void Clear(){
        VoicePlay(ClearVoices);
    }
    public void Hurt(){
        VoicePlay(HurtVoices);
    }
    public void Death(){
        VoicePlay(DeathVoices);
    }
    public void Fallen(){
        VoicePlay(FallenVoices);
    }
    public void Other(int index) {
        VoicePlay(OtherVoices[index]);
    }

    void VoicePlay(AudioClip[] voices) {
        if (voices.Length < 1) return;
        
        int voiceIndex = UnityEngine.Random.Range(0, voices.Length);

        source.clip = voices[voiceIndex];
        if (voices[voiceIndex] != null) source.Play();
    }

    public IEnumerator VoiceEcho(AudioClip[] voices) {
        if (voices.Length < 1) yield break;

        int voiceIndex = UnityEngine.Random.Range(0, voices.Length);

        for (int i = 4; i > 0; i--) {
            float volume = 0.25f * i;
            source.PlayOneShot(voices[voiceIndex], volume);

            yield return new WaitForSeconds(0.7f);
        }
    }
}
