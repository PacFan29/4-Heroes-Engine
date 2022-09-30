using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAnimations : MonoBehaviour
{
    public PlayerInfo info;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Animator>().SetBool("Active", info.groundEvent);
    }
}
