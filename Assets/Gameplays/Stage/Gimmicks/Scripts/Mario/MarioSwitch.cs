using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioSwitch : MonoBehaviour
{
    [Header("アニメーション")]
    public Animator anim;
    protected bool pressed = false;

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("pressed", pressed);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            // this.GetComponent<AudioSource>().Play();
            pressed = true;

            StartCoroutine(Gimmick());
        }
    }

    public virtual IEnumerator Gimmick() {
        yield return null;
    }
}
