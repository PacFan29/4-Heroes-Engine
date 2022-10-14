using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickManager : MonoBehaviour
{
    [HideInInspector] public bool gimmick = false;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (gimmick) {
            gimmickMove();
        }
    }

    public virtual void gimmickMove() {
        ;
    }
}
