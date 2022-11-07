using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpBox : MonoBehaviour
{
    public GameObject exitBox;
    [Header("アニメーション")]
    public Animator entranceAnim;
    public Animator exitAnim;

    private bool warpTrigger = false;
    PlayerInfo player;
    
    void Start() {
        exitBox.SetActive(false);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null && !warpTrigger) {
            player = other.gameObject.GetComponent<PlayerInfo>();

            warpTrigger = true;
            player.gameObject.SetActive(false);
            StartCoroutine("Warp");
        }
    }

    IEnumerator Warp() {
        yield return new WaitForSeconds(1f);

        exitBox.SetActive(true);
        player.dimension = DimensionType.Normal3D;
        player.gameObject.transform.position = exitBox.transform.position;

        yield return new WaitForSeconds(1f);

        player.gameObject.SetActive(true);
        player.ForwardSetUp(exitBox.transform.forward, 10f);
        player.YvelSetUp(22.5f);
        warpTrigger = false;

        yield return new WaitForSeconds(0.75f);
        exitBox.SetActive(false);
    }
}
