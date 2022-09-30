using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitTarget : MonoBehaviour
{
    public GameObject sprite;
    public GameObject effect;
    [Header("インデックス")]
    public int index = 0;
    [Header("得点")]
    public int score = 100;
    private bool earned = false;
    
    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player" && col.gameObject.GetComponent<PlayerInfo>() != null && !earned){
            col.gameObject.GetComponent<PlayerInfo>().scorePopUp(score, false, this.transform.position);
            this.GetComponent<AudioSource>().Play();
            StartCoroutine("GotIt");

            GameManager.fruits[index]++;
            earned = true;
        }
    }

    IEnumerator GotIt() {
        sprite.SetActive(false);
        effect.SetActive(true);

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
