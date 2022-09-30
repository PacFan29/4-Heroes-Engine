using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSkins : MonoBehaviour
{
    public PlayerInfo info;
    public Material[] powerUpMats = new Material[5];
    public int index;

    // Update is called once per frame
    void Update()
    {
        Material[] currentMat = this.GetComponent<SkinnedMeshRenderer>().materials;

        currentMat[index] = powerUpMats[info.powerUpActive];

        this.GetComponent<SkinnedMeshRenderer>().materials = currentMat;
    }
}
