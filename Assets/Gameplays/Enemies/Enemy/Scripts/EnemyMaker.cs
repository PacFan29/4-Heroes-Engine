using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaker : MonoBehaviour
{
    public GameObject enemy;
    public GameObject hide;
    private GameObject current;
    private float time = 2f;
    // Start is called before the first frame update
    void Start()
    {
        current = Instantiate(enemy, transform.position, Quaternion.identity);
        hide.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (time > 0 && current == null){
            time -= Time.deltaTime;
            if (time <= 0){
                current = Instantiate(enemy, transform.position, Quaternion.identity);
                time = 2;
            }
        }
    }
}
