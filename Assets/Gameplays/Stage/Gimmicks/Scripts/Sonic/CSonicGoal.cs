using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSonicGoal : GoalManager
{
    [Header("当たり判定")]
    public BoxCollider beforeGoal;
    public CapsuleCollider bonusCollider;
    private int step = 0;
    private int bonus = 1000;
    [Header("効果音")]
    public AudioClip goalSound;
    public AudioClip bonusSound;
    public AudioClip landSound;

    [Header("リジッドボディ")]
    public LayerMask GroundLayer;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        beforeGoal.enabled = (step == 0);
        bonusCollider.enabled = (step == 1);
    }
    void FixedUpdate() {
        if (step == 1) {
            velocity.y -= 0.25f;

            RaycastHit hit;
            Physics.Raycast(transform.position, -Vector3.up, out hit, 3f, GroundLayer);

            if (hit.distance > 0 && velocity.y < 0) {
                step = 2;
                velocity = Vector3.zero;
                this.transform.position += Vector3.up * (2.83f - hit.distance);
                SoundPlay(landSound);

                GoToResult();
            }

            this.GetComponent<Rigidbody>().velocity = velocity;
        }
    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player"){
            PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();
            switch (step) {
                case 0:
                SoundPlay(goalSound);
                if (player.XZmag >= 20f) {
                    step = 1;
                    velocity.y = player.XZmag * 0.3f;
                } else {
                    step = 2;
                    GoToResult();
                }
                break;

                case 1:
                if (!player.Grounded && player.finalVelocity.y > 0) {
                    player.scorePopUp(bonus, false, this.transform.position);
                    if (bonus > 0) bonus -= 100;
                    SoundPlay(bonusSound);
                    velocity.y = 10f;
                }
                break;
            }
        }
    }
    void SoundPlay(AudioClip clip) {
        this.GetComponent<AudioSource>().PlayOneShot(clip);
    }
}
