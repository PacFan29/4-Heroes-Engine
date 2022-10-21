using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InspiredBy : MonoBehaviour
{
    public GameObject[] screens = new GameObject[2];
    public Image fade;

    private bool fadeOut = true;
    private float fadeOpacity = 1.0f;
    private int step = 0;

    private const float skipTime = 0.75f;
    private float time = 1f;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < screens.Length; i++) {
            screens[i].SetActive(step - 2 == i);
        }

        //フェード
        if (fadeOut) {
            fadeOpacity += 4 * Time.deltaTime;
        } else {
            fadeOpacity -= 4 * Time.deltaTime;
        }
        fadeOpacity = Mathf.Clamp(fadeOpacity, 0f, 1f);
        fade.color = new Color(0f, 0f, 0f, fadeOpacity);

        time -= Time.deltaTime;

        if ((Input.GetButtonDown("A") || Input.GetButtonDown("Start")) && time <= skipTime && step > 0 && step < 4) {
            time = 0f;
        }

        if (time <= 0) {
            if (step == 0) {
                time = 1.5f;
                fadeOpacity = 0f;
                fadeOut = false;
                step++;
                return;
            } if (step == 4) {
                SceneManager.LoadScene("LogoScreen");
            } else {
                fadeOut = true;

                if (fadeOpacity >= 1f) {
                    step++;
                    if (step == 4) {
                        time = 1f;
                    } else {
                        fadeOut = false;
                        time = 1.75f;
                    }
                }
            }
        }
    }
}
