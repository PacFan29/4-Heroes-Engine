using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSonicGoal : GoalManager
{
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.GetChild(0).Rotate(0f, -3f, 0f, Space.Self);
    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player"){
            GoToResult();
        }
    }
}
