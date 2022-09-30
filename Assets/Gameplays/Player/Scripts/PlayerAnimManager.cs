using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimManager : MonoBehaviour
{
    public Animator skin;
    public Animator metalSkin;
    private PlayerInfo info;
    [HideInInspector] public int actionId;
    public AudioClip[] footstepSounds;
    // Start is called before the first frame update
    void Start()
    {
        info = GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (skin != null) {
            skin.SetFloat("Speed", info.XZmag);
            skin.SetBool("Crouching", info.Crouching);
            skin.SetBool("Rolling", info.rolling);
            skin.SetBool("Skidding", info.skidding);

            skin.SetBool("Grounded", info.Grounded);
            skin.SetFloat("Y velocity", info.finalVelocity.y);

            skin.SetInteger("Actions", actionId);
        }
        if (metalSkin != null) {
            metalSkin.SetFloat("Speed", info.XZmag);
            metalSkin.SetBool("Crouching", info.Crouching);
            metalSkin.SetBool("Rolling", info.rolling);
            metalSkin.SetBool("Skidding", info.skidding);

            metalSkin.SetBool("Grounded", info.Grounded);
            metalSkin.SetFloat("Y velocity", info.finalVelocity.y);

            metalSkin.SetInteger("Actions", actionId);
        }
    }
}
