using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        transform.LookAt(player.transform.position);
        rb.velocity = transform.forward * 25;

        StartCoroutine("LifeTime");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "HomingTarget"){
            if (other.gameObject.tag == "Player" && other.GetComponent<PlayerInfo>().shieldActive <= 0){
                other.GetComponent<PlayerInfo>().TakeDamage(4, this.transform.position);
            }
            if (
                other.gameObject.tag != "PlayerAttack" && 
                other.gameObject.tag != "EnemyAttack"
            ) {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator LifeTime(){
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
