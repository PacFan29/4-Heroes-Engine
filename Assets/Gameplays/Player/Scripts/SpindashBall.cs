using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpindashBall : MonoBehaviour
{
    public Animator anim;
    [HideInInspector] public bool isSpindash = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.GetChild(0).Rotate(60f, 0, 0);

        anim.SetBool("Spindash", isSpindash);
    }
}
