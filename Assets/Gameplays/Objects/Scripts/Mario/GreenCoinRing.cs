using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCoinRing : RingManager
{
    private float time = 10f;

    // Update is called once per frame
    void Update()
    {
        if (gotIt) {
            time -= Time.deltaTime;
            if (time <= 0) {
                Destroy(gameObject);
            }
        }
    }
}
