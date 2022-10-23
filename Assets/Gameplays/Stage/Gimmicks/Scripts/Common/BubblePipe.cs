using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePipe : MonoBehaviour
{
    public GameObject bubble;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("makeBubble");
    }
    
    IEnumerator makeBubble() {
        while (true) {
            yield return new WaitForSeconds(3f);

            Instantiate(bubble, this.transform.position + Vector3.up, Quaternion.identity);
        }
    }
}
