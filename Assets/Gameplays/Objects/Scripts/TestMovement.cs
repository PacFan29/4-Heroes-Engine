using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    private Vector3 pos;
    private float angle = 0;
    // Start is called before the first frame update
    void Start()
    {
        pos = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        angle += Time.deltaTime*2;
        angle %= 360;
        this.transform.position = pos + new Vector3((float)Math.Sin(angle) * 5f, 0f, (float)Math.Cos(angle) * 5f);
    }
}
