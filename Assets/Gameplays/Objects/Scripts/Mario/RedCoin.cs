using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCoin : MonoBehaviour
{
    public GameObject coinEffect;
    // Start is called before the first frame update
    void Awake()
    {
        
    }
    
    void FixedUpdate() {
        this.transform.Rotate(0f, -3f, 0f, Space.Self);
    }
    
    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player"){
            PlayerInfo player = col.GetComponent<PlayerInfo>();

            GameManager.Coins += 1;
            player.scoreIncrease(50);

            if (col.GetComponent<_16MSonic>() != null) {
                col.GetComponent<_16MSonic>().BoostIncrease(1, false);
            }

            Instantiate(coinEffect, this.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
