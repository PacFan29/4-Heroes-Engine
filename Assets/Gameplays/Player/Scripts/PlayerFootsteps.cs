using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("エフェクト")]
    public GameObject effect;
    [Header("効果音")]
    public AudioClip[] footstepPlane;
    public AudioClip[] footstepGrass;
    public AudioClip[] footstepMetal;
    public AudioClip[] footstepSand;

    public void Footstep() {
        bool enabled = true;
        Renderer[] childrenRenderer = this.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < childrenRenderer.Length; i++){
			enabled = enabled ? childrenRenderer[i].enabled : false;
		}
        if (!enabled) return;

        Instantiate(effect, this.transform.position, Quaternion.identity);

        AudioClip[] clips = new AudioClip[1];

        RaycastHit hit;
        Physics.Raycast(transform.position, -Vector3.up, out hit, 20f);

        //Debug.Log(transform.up);

        if (hit.collider != null) {
            switch (hit.collider.gameObject.tag) {
                case "FloorGrass":
                clips = footstepGrass;
                break;

                default:
                clips = footstepPlane;
                break;
            }

            int index = UnityEngine.Random.Range(0, clips.Length);
            this.GetComponent<AudioSource>().PlayOneShot(clips[index]);
        }
    }
}
