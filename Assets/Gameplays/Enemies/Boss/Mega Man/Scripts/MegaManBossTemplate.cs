using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaManBossTemplate : MegaManBossManager
{
    public override IEnumerator Attack()
    {
        while (true){
            yield return null;
        }
    }
    public override IEnumerator PhaseChange() {
        yield return null;
    }
    public override IEnumerator DefeatedAnimation(){
        sound.PlayOneShot(defeatedSound);
        Time.timeScale = 0.5f;

        yield return new WaitForSeconds(0.875f);

        sound.PlayOneShot(explosionSound);
        Time.timeScale = 1f;

        yield return new WaitForSeconds(2.5f);
        
        OnDestroy();
        
        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }
}
