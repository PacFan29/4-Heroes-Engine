using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioGoal : GoalManager
{
    [Header("旗")]
    public Transform villainFlag;
    public Transform playerFlag;
    [HideInInspector] public float bottomPos;
    [HideInInspector] public float upperPos;
    [HideInInspector] public float rate = 0.0f;
    [HideInInspector] public int step = 0;

    [Header("効果音")]
    public AudioClip touchedSound;
    public AudioClip touchedTopSound;
    public AudioClip downSound;
    // Start is called before the first frame update
    void Start()
    {
        bottomPos = playerFlag.position.y;
        upperPos = villainFlag.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float diff = upperPos - bottomPos;
        float yPos = diff * rate + bottomPos;
        playerFlag.transform.position = new Vector3(this.transform.position.x, yPos, this.transform.position.z);

        villainFlag.gameObject.SetActive(step == 0);
        playerFlag.gameObject.SetActive(step == 2);
    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player" && step == 0){
            GameManager.timeIncrease = false;
            PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();

            float yPos = col.gameObject.transform.position.y - (this.transform.position.y + 2.55125f);
            int height = (int)Math.Round((yPos / 20) * 7);
            height = Math.Min(height, 7);
            int score = (int)Math.Pow(2, height) * 100;

            if (height == 7) {
                SoundPlay(touchedTopSound);
                step = 1;
            } else {
                SoundPlay(touchedSound);
                step = 2;
            }

            Vector3 position = this.transform.position;
            position.y = col.gameObject.transform.position.y;
            player.scorePopUp(score, false, position);

            StartCoroutine(player.GoalPole(this, height == 7));
        }
    }
}
