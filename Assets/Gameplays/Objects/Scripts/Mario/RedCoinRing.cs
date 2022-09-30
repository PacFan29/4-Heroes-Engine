using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCoinRing : RingManager
{
    private float time = 10f;

    [Header("赤コイン")]
    public GameObject redCoinsParent;

    // Update is called once per frame
    void Update()
    {
        if (gotIt) {
            time -= Time.deltaTime;
            if (time <= 0) {
                Destroy(gameObject);
            }
        }
        redCoinsParent.SetActive(gotIt);
    }
}
