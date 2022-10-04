using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _14Amy : SonicActions
{
    private bool sliding = false;
    [Header("効果音")]
    public AudioClip slidingSound;
    public LoopingSoundManager lManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //共通アクションの実行
        actions();

        /*
        エミーでしかできない技
        ・ハンマーで攻撃
        */

        //スライディング
        if ((info.GetCrouchButton("RB") || info.GetCrouchButton("B")) && info.Grounded && !sliding && !info.rolling && actionId != 5) {
            if (info.XZmag <= 0 && info.input != Vector3.zero) {
                info.ForwardSetUp(Vector3.zero, 15f);
                info.Crouching = false;
            }
            if (info.XZmag >= 2.5f) {
                info.constantChange(true, "rollfrc", info.RollFrc * 3);
                info.rolling = true;
                sliding = true;

                lManager.SetUp(slidingSound, 1f, 2.595f);
            }
        } else if (sliding && ((!(info.GetCrouchButton("RB") || info.GetCrouchButton("B")) && transform.up == Vector3.up) || !info.Grounded || info.Crouching)) {
            info.constantChange(true, "rollfrc", info.RollFrc);
            if (info.Grounded || (!info.Grounded && info.finalVelocity.y <= 0)) info.rolling = false;
            sliding = false;

            lManager.Stop();
        }
    }
}
