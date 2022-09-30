using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WipeManager : MonoBehaviour
{
    public static bool isWipeOut = false;
    public static float speed = 1f;
    public static float scale = 1;

    public RectTransform wipeShape;

    void Start()
    {
        if (isWipeOut) {
            WipeSetUp(false, 2.5f);
        }
    }

    void Update()
    {
        if (!isWipeOut && scale < 1) {
            scale += speed * Time.deltaTime;
        } else if (isWipeOut && scale > 0) {
            scale -= speed * Time.deltaTime;
        }
        scale = Mathf.Clamp(scale, 0f, 1f);

        float size = scale * 1250f;

        wipeShape.sizeDelta = new Vector2(size, size);

        if (scale >= 1f) {
            this.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        } else {
            this.GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
        }
    }

    public static void WipeSetUp(bool wipeOut, float setSpeed) {
        isWipeOut = wipeOut;
        speed = setSpeed;
    }
}