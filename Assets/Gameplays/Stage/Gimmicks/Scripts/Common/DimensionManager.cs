using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionManager : MonoBehaviour
{   
    protected Rigidbody rb;
    [HideInInspector] public DimensionType dimension;
    [HideInInspector] public float layerPos;
    [HideInInspector] public int layerRate;

    public void FreezePos(DimensionType dimension){
        if (rb != null) {
            switch (dimension){
                case DimensionType.Normal3D:
                //3D
                rb.constraints = RigidbodyConstraints.None;
                break;

                case DimensionType.XWay2D:
                //2D(X方向、Z固定)
                rb.constraints = RigidbodyConstraints.FreezePositionZ;
                break;
                
                case DimensionType.ZWay2D:
                //2D(Z方向、X固定)
                rb.constraints = RigidbodyConstraints.FreezePositionX;
                break;
            }
            rb.constraints |= RigidbodyConstraints.FreezeRotation;
        }
    }
    public bool is3D() {
        return dimension == DimensionType.Normal3D;
    }
}
