using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTemplate : BossManager
{
    [Header("攻撃オブジェクト")]
    public GameObject bullet;
    [Header("エフェクト")]
    public GameObject vanishEffect;
    
    public override IEnumerator Attack()
    {
        while (true){
            if (phase == 0) {
                if (bullet != null) {
                    GameObject blt = Instantiate(bullet, transform.position, Quaternion.identity);
                }
                yield return new WaitForSeconds(0.1f);
            } else {
                for (int i = 0; i < 30; i++) {
                    if (bullet != null) {
                        GameObject blt = Instantiate(bullet, transform.position, Quaternion.identity);
                    }
                    yield return new WaitForSeconds(0.2f);
                }
                yield return new WaitForSeconds(4f);
            }
        }
    }
    public override IEnumerator PhaseChange() {
        StopCoroutine("Attack");
        StartCoroutine("Attack");
        yield return null;
    }
    public override IEnumerator DefeatedAnimation(){
        for (int i = 0; i < 30; i++){
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f));
            Instantiate(vanishEffect, transform.position + pos, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }
}
