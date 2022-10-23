using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    //Code by codinginflow(GitHub)
    private GameObject[] waypoints;
    public float speed = 1f;

    int currentWaypointIndex = 0;

    void Start()
    {
        int length = this.transform.parent.transform.childCount - 1;
        waypoints = new GameObject[length];

        for (int i = 1; i <= length; i++) {
            waypoints[i-1] = this.transform.parent.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
    void Update() {
        if (waypoints[currentWaypointIndex] != null) {
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < 0.1f){
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = 0;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            other.gameObject.transform.parent = transform;
        }
    }
    void OnCollisionExit(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            other.gameObject.transform.parent = null;
        }
    }
}
