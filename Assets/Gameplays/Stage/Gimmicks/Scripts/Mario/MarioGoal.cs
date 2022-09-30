using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioGoal : GoalManager
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player"){
            PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();

            float yPos = col.gameObject.transform.position.y - (this.transform.position.y + 2.55125f);
            int height = (int)Math.Round((yPos / 20) * 7);
            height = Math.Min(height, 7);
            int score = (int)Math.Pow(2, height) * 100;

            Vector3 position = this.transform.position;
            position.y = col.gameObject.transform.position.y;
            player.scorePopUp(score, false, position);

            GoToResult();
        }
    }
}
