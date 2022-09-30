using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Alignment {
    Right,
    Center,
    Left
}
public class SpriteNumber : MonoBehaviour
{
    public int value = 0;
    public int zeroFill = 0;
    public Alignment alignment;
    [Header("スプライト")]
    public Sprite[] numimages = new Sprite[10];
    public float xSize = 20f;
    [Header("イメージ")]
    public GameObject numObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<int> number = new List<int>();
        int digit = value;

        foreach (Transform obj in this.transform) {
            if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                Destroy(obj.gameObject);
            }
        }
        
        if (digit == 0) {
            number.Add(0);
        }
        while (digit != 0) {
            number.Add(digit % 10);
            digit = digit / 10;
        }

        switch (alignment) {
            case Alignment.Center:
            numObj.GetComponent<RectTransform>().localPosition = Vector3.right * ((number.Count - 1) * (xSize / 2f));
            break;

            case Alignment.Left:
            numObj.GetComponent<RectTransform>().localPosition = Vector3.right * ((number.Count - 1) * xSize);
            break;
        }
        numObj.GetComponent<Image>().sprite = numimages[number[0]];
        for ( int i = 1 ; i < number.Count ; i++ ) {
            //複製
            RectTransform scoreimage = (RectTransform)Instantiate(numObj).transform;
            scoreimage.SetParent(this.transform , false);
            scoreimage.localPosition = new Vector2(
                scoreimage.localPosition.x - xSize * i ,
                scoreimage.localPosition.y);
            scoreimage.GetComponent<Image>().sprite = numimages[number[i]];
        }
        if (zeroFill > 0) {
            for ( int i = number.Count ; i < zeroFill ; i++ ) {
                //複製
                RectTransform scoreimage = (RectTransform)Instantiate(numObj).transform;
                scoreimage.SetParent(this.transform , false);
                scoreimage.localPosition = new Vector2(
                    scoreimage.localPosition.x - xSize * i ,
                    scoreimage.localPosition.y);
                scoreimage.GetComponent<Image>().sprite = numimages[0];
            }
        }
    }
}
