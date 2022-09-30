using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorColliderChecker : MonoBehaviour
{
    private MonitorManager parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent.gameObject.GetComponent<MonitorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player" && !parent.destroyed){
            parent.player = col.gameObject.GetComponent<PlayerInfo>();
            if (parent.player.rolling || (!parent.player.Grounded && parent.player.Stompable)) {
                col.gameObject.GetComponent<PlayerInfo>().Stomp();
                parent.destroyed = true;
            }
        }
    }
}
