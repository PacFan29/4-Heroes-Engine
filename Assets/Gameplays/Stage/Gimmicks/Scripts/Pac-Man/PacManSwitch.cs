using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PacManSwitch : MonoBehaviour
{
    [Header("アニメーション")]
    public Animator anim;
    [HideInInspector] public bool pressed = false;

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("pressed", pressed);
    }

    public void press() {
        if (!pressed) {
            this.GetComponent<AudioSource>().Play();
            pressed = true;

            StartCoroutine(Gimmick());
        }
    }

    public virtual IEnumerator Gimmick() {
        yield return null;
    }
}
