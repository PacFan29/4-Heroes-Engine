using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCookie : MonoBehaviour
{
    public GameObject skin;
    private bool enable = true;
    public bool power = false;
    
    void OnTriggerEnter(Collider other) {
        if (enable && other.gameObject.GetComponent<MazePlayer>() != null) {
            SpecialStageManager.Score += power ? 50 : 20;

            if (power) {
                other.gameObject.GetComponent<MazePlayer>().PowerUp();
            }

            enable = false;
            skin.SetActive(false);
            this.GetComponent<AudioSource>().Play();
            StartCoroutine("GotIt");
        }
    }

    IEnumerator GotIt() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
