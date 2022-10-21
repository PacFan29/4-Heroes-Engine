using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoScreen : MonoBehaviour
{
    public Image fade;

    private bool fadeOut = false;
    private float fadeOpacity = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Logo");
    }

    // Update is called once per frame
    void Update()
    {
        //フェード
        if (fadeOut) {
            fadeOpacity += 2 * Time.deltaTime;
            this.GetComponent<AudioSource>().volume = (1f - fadeOpacity) * 0.3f;
        } else {
            fadeOpacity -= 2 * Time.deltaTime;
        }
        fadeOpacity = Mathf.Clamp(fadeOpacity, 0f, 1f);
        fade.color = new Color(0f, 0f, 0f, fadeOpacity);

        if (!fadeOut && (Input.GetButtonDown("A") || Input.GetButtonDown("Start"))) {
            StartCoroutine("GoToTitle");
        }
    }
    IEnumerator Logo() {
        yield return new WaitForSeconds(7f);

        if (!fadeOut) {
            StartCoroutine("GoToTitle");
        }
    }
    IEnumerator GoToTitle() {
        fadeOut = true;

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("TitleScreen");
    }
}
