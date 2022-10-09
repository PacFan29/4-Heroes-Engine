using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBar : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public int fireLength = 3;
    public bool bothSide = false;
    [Header("オブジェクト")]
    public RectTransform group;
    
    void Start() {
        GameObject fireball = group.GetChild(0).gameObject;

        if (bothSide) {
            Instantiate(fireball, this.transform.position - Vector3.right * 3.5f, this.transform.rotation, group);
        }
        for (int i = 2; i <= fireLength; i++) {
            Instantiate(fireball, this.transform.position + Vector3.right * 3.5f * i, this.transform.rotation, group);
            if (bothSide) {
                Instantiate(fireball, this.transform.position - Vector3.right * 3.5f * i, this.transform.rotation, group);
            }
        }
    }
    void FixedUpdate()
    {
        group.Rotate(0f, rotationSpeed / 5f, 0f, Space.Self);
    }
}
