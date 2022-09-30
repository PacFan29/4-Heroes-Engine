using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearingBlockManager : MonoBehaviour
{
    public float interval = 1f;
    public float singleLifetime = 1f;
    private AppearingBlockSingle[] blocks;
    private int maxIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        blocks = GetComponentsInChildren<AppearingBlockSingle>();

        foreach (AppearingBlockSingle block in blocks) {
            block.lifeTime = singleLifetime;
            if (block.index > maxIndex) {
                maxIndex = block.index;
            }
            block.gameObject.SetActive(false);
        }

        StartCoroutine("Appear");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Appear() {
        yield return new WaitForSeconds(3f);

        while (true) {
            int i = 0;
            while (i <= maxIndex) {
                foreach (AppearingBlockSingle block in blocks) {
                    if (i == block.index) {
                        block.gameObject.SetActive(true);
                    }
                }
                yield return new WaitForSeconds(interval);
                i++;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
