using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManCheckPoint : CheckPointManager
{
    public GameObject skin;
    public GameObject effect;

    void Start() {
        if (passed) {
            Destroy(gameObject);
        }
    }
    void FixedUpdate() {
        skin.transform.Rotate(-3f, 0f, 0f, Space.Self);
    }

    public override IEnumerator Animation() {
        skin.SetActive(false);
        effect.SetActive(true);

        yield return new WaitForSeconds(4f);

        Destroy(gameObject);
    }
}
