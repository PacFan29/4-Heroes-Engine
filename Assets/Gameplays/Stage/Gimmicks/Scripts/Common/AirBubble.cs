using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBubble : MonoBehaviour
{
    private float scaleRate = 0.1f;
    private float lifeTime = 5f;

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f) {
            Destroy(gameObject);
        }

        scaleRate += 1.5f * Time.deltaTime;
        scaleRate = Mathf.Clamp(scaleRate, 0f, 1f);

        this.transform.localScale = Vector3.one * 5 * scaleRate;
    }

    void FixedUpdate() {
        transform.Translate(0, 0.1f, 0);
    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player"){
            col.gameObject.GetComponent<PlayerInfo>().Breath();
            Destroy(gameObject);
        }
    }
}
