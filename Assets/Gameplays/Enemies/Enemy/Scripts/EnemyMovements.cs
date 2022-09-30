using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovements : MonoBehaviour
{
    protected EnemyManager manager;
    protected GameObject targetPlayer;

    protected int action = 0;
    protected float time = 0.2f;

    protected Vector3 direction = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.isActive() && manager.isNear()) {
            Movements();
            if (direction != Vector3.zero) manager.skin.transform.forward = direction.normalized;
        } else {
            action = 0;
            time = 0.2f;
        }
    }

    public virtual void Movements() {
        ;
    }

    public void SetPlayer() {
        float minDistance = Mathf.Infinity;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<PlayerInfo>() != null && Vector3.Distance(player.transform.position, this.transform.position) < minDistance) {
                minDistance = Vector3.Distance(player.transform.position, this.transform.position);
                targetPlayer = player;
            }
        }
    }

    public float GetDistance() {
        if (targetPlayer != null) {
            return Vector3.Distance(targetPlayer.transform.position, this.transform.position);
        } else {
            return Mathf.Infinity;
        }
    }

    public bool IsLooking() {
        if (targetPlayer != null) {
            return Vector3.Angle(manager.skin.transform.forward, (targetPlayer.transform.position - this.transform.position)) <= 90;
        } else {
            return false;
        }
    }

    public bool SeenPlayer() {
        if (targetPlayer != null) {
            return Math.Abs(targetPlayer.transform.position.y - this.transform.position.y) < 10f;
        } else {
            return false;
        }
    }

    public bool PlayerFound() {
        if (targetPlayer != null) {
            PlayerInfo plInfo = targetPlayer.GetComponent<PlayerInfo>();
            return GetDistance() >= 2 && GetDistance() <= 30 && IsLooking() && SeenPlayer() && plInfo.HP > 0 && !plInfo.tookDamage;
        } else {
            return false;
        }
    }
}
