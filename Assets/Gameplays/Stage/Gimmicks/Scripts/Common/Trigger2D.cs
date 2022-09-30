using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Trigger2D : MonoBehaviour
{
    public Sprite[] images = new Sprite[3];
    public DimensionType Dimension;
    private GameObject hideObject;
    void Start() {
        hideObject = transform.GetChild(0).gameObject;
        Destroy(hideObject);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<DimensionManager>() != null){
            DimensionManager obj = other.gameObject.GetComponent<DimensionManager>();
            obj.dimension = Dimension;
            Vector3 playerPos = other.gameObject.transform.position;
            Vector3 pos;

            switch(Dimension) {
                case DimensionType.XWay2D:
                pos = new Vector3(playerPos.x, playerPos.y, this.transform.position.z);
                other.gameObject.transform.position = pos;
                obj.layerPos = this.transform.position.z;
                obj.layerRate = 0;
                break;
                    
                case DimensionType.ZWay2D:
                pos = new Vector3(this.transform.position.x, playerPos.y, playerPos.z);
                other.gameObject.transform.position = pos;
                obj.layerPos = this.transform.position.x;
                obj.layerRate = 0;
                break;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<DimensionManager>() != null) {
            other.gameObject.GetComponent<DimensionManager>().dimension = DimensionType.Normal3D;
        }
    }

    void EditorUpdate(){
        try {
            if (transform.GetChild(0) != null) {
                SpriteRenderer Display = transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>();
                switch(Dimension) {
                    case DimensionType.Normal3D:
                    Display.sprite = images[0];
                    break;

                    case DimensionType.XWay2D:
                    Display.sprite = images[1];
                    break;
                    
                    case DimensionType.ZWay2D:
                    Display.sprite = images[2];
                    break;
                }
            }
        } catch (UnityException) {
            return;
        }
    }
}
