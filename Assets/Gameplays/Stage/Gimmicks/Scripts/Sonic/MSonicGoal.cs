using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSonicGoal : GoalManager
{
    [Header("スキン")]
    public Transform skin;
    private float scale = 1f;

    [Header("効果音")]
    public AudioClip goalSound;

    private bool touchable = true;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.GetChild(0).Rotate(0f, -3f, 0f, Space.Self);
    }

    void Update()
    {
        if (!touchable) {
            scale -= Time.deltaTime;
            if (scale < 0) scale = 0;
        }
        skin.localScale = Vector3.one * 0.125f * scale;
    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player" && touchable){
            AudioSource source = this.GetComponent<AudioSource>();
            source.clip = goalSound;
            source.loop = false;
            source.Play();

            touchable = false;
            cleared(col.gameObject);
        }
    }
}
