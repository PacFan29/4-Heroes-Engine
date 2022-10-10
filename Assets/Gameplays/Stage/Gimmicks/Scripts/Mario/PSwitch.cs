using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSwitch : MarioSwitch
{
    [Header("レンガとコインの入れ替え")]
    public GameObject brick;
    public GameObject coin;
    [Header("青コイン出現")]
    public GameObject blueCoins;
    
    private void Start() {
        blueCoins.SetActive(false);
    }
    public override IEnumerator Gimmick() {
        switchObj();
        blueCoins.SetActive(true);

        yield return new WaitForSeconds(10f);

        switchObj();
        Destroy(blueCoins);
    }

    void switchObj() {
        BrickManager[] bricks = FindObjectsOfType<BrickManager>();
        CoinManager[] coins = FindObjectsOfType<CoinManager>();
        Dictionary<Vector3, Transform> brickPos = new Dictionary<Vector3, Transform>();
        Dictionary<Vector3, Transform> coinPos = new Dictionary<Vector3, Transform>();

        //レンガとコインの入れ替え
        foreach (BrickManager b in bricks) {
            if (!b.breaked && b.blockType == BlockType.None) {
                Transform brickTrans = b.gameObject.transform;
                Transform parent = brickTrans.parent;
                brickPos.Add(brickTrans.position, parent);
                Destroy(b.gameObject);
            }
        }
        foreach (var pair in brickPos) {
            if (pair.Value != null) {
                Instantiate(coin, pair.Key, Quaternion.identity, pair.Value);
            } else {
                Instantiate(coin, pair.Key, Quaternion.identity);
            }
        }

        //コインとレンガの入れ替え
        foreach (CoinManager c in coins) {
            if (c.amounts < 2 && !c.magnetised && !c.split) {
                Transform coinTrans = c.gameObject.transform;
                Transform parent = coinTrans.parent;
                coinPos.Add(coinTrans.position, parent);
                Destroy(c.gameObject);
            }
        }
        foreach (var pair in coinPos) {
            if (pair.Value != null) {
                Instantiate(brick, pair.Key, Quaternion.identity, pair.Value);
            } else {
                Instantiate(brick, pair.Key, Quaternion.identity);
            }
        }
    }
}
