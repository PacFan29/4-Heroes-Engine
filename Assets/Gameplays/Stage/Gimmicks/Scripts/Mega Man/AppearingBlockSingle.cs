using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearingBlockSingle : MonoBehaviour
{
    public int index;
    [HideInInspector] public float lifeTime = 1.5f;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine("Appear");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Appear() {
        yield return new WaitForSeconds(lifeTime);

        this.gameObject.SetActive(false);
    }
}
