using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRing : RingManager
{
    [Header("コイン")]
    public GameObject coin;

    public override IEnumerator EventStart() {
        for (int i = 0; i < 3; i++) {
            SplitCoin(player);
            yield return new WaitForSeconds(0.15f);
        }
        
        yield return new WaitForSeconds(1.1f);
        Destroy(gameObject);
    }
    
    void SplitCoin(PlayerInfo player) {
        CoinManager coinObj = Instantiate(coin, this.transform.position, Quaternion.identity).GetComponent<CoinManager>();
        coinObj.targetPlayer = player.gameObject;

        coinObj.StartCoroutine(coinObj.SplitCoin());
    }
}
