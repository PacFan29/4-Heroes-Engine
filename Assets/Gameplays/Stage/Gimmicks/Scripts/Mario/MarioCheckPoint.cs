using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioCheckPoint : CheckPointManager
{
    [Header("旗の柄")]
    public SkinnedMeshRenderer flagMesh;
    public Material[] allFlags = new Material[18];

    // Update is called once per frame
    void Update()
    {
        Material[] mat = new Material[1];
        if (passed) {
            mat[0] = allFlags[(playerId + 1)];
        } else {
            mat[0] = allFlags[0];
        }
        flagMesh.materials = mat;
    }
}
