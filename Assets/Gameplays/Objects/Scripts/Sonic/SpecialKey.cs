using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialKey : MonoBehaviour
{
    public GameObject skin;
    public GameObject effect;
    private bool earned = false;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!earned) {
            this.transform.Rotate(0f, -3f, 0f, Space.Self);
        }
    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player" && col.gameObject.GetComponent<PlayerInfo>() != null && !earned){
            this.GetComponent<AudioSource>().Play();
            StartCoroutine("GotIt");

            earned = true;
        }
    }

    IEnumerator GotIt() {
        skin.SetActive(false);
        effect.SetActive(true);

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
}
