using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MegaManHealthBar : MonoBehaviour
{
    public Color mainColor;
    public Image outline;
    public GameObject segment;
    [Header("値")]
    public int maxValue = 28;
    public int currentValue = 28;
    private int leastValue = 28;
    // Start is called before the first frame update
    void Start()
    {
        UpdateBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (leastValue != currentValue) {
            UpdateBar();

            if (leastValue < currentValue) {
                //回復
            } else {
                //ダメージ
            }

            leastValue = currentValue;
        }
    }
    void UpdateBar() {
        outline.color = mainColor;

        int amounts = (int)Math.Ceiling(((float)currentValue / maxValue) * 28f);

        foreach (Transform obj in this.transform) {
            if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                Destroy(obj.gameObject);
            }
        }

        segment.GetComponent<Image>().color = (amounts > 0) ? mainColor : Color.clear;
        for ( int i = 1 ; i < amounts ; i++ ) {
            //複製
            RectTransform scoreimage = (RectTransform)Instantiate(segment).transform;
            scoreimage.SetParent(this.transform , false);
            scoreimage.localPosition = new Vector2(
                scoreimage.localPosition.x,
                scoreimage.localPosition.y + 4f * i);
        }
    }
}
