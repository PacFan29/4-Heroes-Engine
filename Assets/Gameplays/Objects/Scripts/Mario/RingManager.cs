using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RingManager : MonoBehaviour
{
    protected bool gotIt = false;
    [Header("共通オブジェクト")]
    public Transform skin;
    public GameObject effect;

    protected PlayerInfo player = null;

    // Update is called once per frame
    void FixedUpdate()
    {
        skin.Rotate(0f, 0f, 5f, Space.Self);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null && !gotIt) {
            player = other.gameObject.GetComponent<PlayerInfo>();
            player.scorePopUp(1000, false, this.transform.position);

            gotIt = true;

            skin.gameObject.SetActive(false);
            effect.SetActive(true);

            StartCoroutine("EventStart");
        }
    }

    public virtual IEnumerator EventStart() {
        yield return null;
    }
}
