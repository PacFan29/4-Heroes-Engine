using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerShifter : MonoBehaviour
{
    public bool isFront = false;
    
    void Awake() {
        Transform children = this.GetComponentInChildren<Transform>();
        //子要素がいなければ終了
        if (children.childCount == 0) {
            return;
        }
        foreach(Transform ob in children) {
            //ここに何かしらの処理
            Destroy(ob.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<DimensionManager>() != null){
            DimensionManager obj = other.gameObject.GetComponent<DimensionManager>();
            if (obj.layerRate == 0) {
                obj.layerRate = isFront ? 1 : -1;
            } else {
                obj.layerRate = 0;
            }
        }
    }
}
