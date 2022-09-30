using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMaker : MonoBehaviour
{
    public GameObject makeObject;
    public GameObject hide;
    private GameObject current = null;
    private float time = 0.1f;
    private bool produced = false;
    // Start is called before the first frame update
    void Start()
    {
        hide.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (time > 0 && current == null){
            time -= Time.deltaTime;
            if (time <= 0){
                current = Instantiate(makeObject, transform.position, Quaternion.identity);

                if (current.GetComponent<EnemyManager>() != null && produced) {
                    //current.GetComponent<EnemyManager>().Score = 0;
                }
                
                time = 2;
            }
        } else {
            produced = true;
        }
    }
}
