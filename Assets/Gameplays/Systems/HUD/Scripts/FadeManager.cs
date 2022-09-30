using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static bool isFadeOut = false;
    public static float speed = 1f;
    public static float alpha = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (isFadeOut) {
            FadeSetUp(false, 2.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFadeOut && alpha < 1) {
            alpha += speed * Time.deltaTime;
        } else if (!isFadeOut && alpha > 0) {
            alpha -= speed * Time.deltaTime;
        }
        alpha = Mathf.Clamp(alpha, 0f, 1f);

        this.GetComponent<Image>().color = new Color(0f, 0f, 0f, alpha);
    }

    public static void FadeSetUp(bool fadeOut, float setSpeed) {
        isFadeOut = fadeOut;
        speed = setSpeed;
    }
}
