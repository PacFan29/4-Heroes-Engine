using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameData data;
    public CommandButton[] buttons = new CommandButton[2];
    public CommandButton[] buttons2 = new CommandButton[2];
    public MessageBox nextStep;

    [Header("効果音")]
    public AudioClip continueSound;
    public AudioClip quitSound;
    public AudioClip acceptSound;
    public AudioClip cancelSound;
    private int select = 0;
    private int select2 = 0;
    private float beforeAxis = 0f;
    private bool active = true;
    private bool areYouSure = false;

    private AudioSource source;

    void Awake() {
        source = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (active) {
            if (areYouSure) {
                AreYouSure();
            } else {
                Continue();
            }
        }
    }

    void Continue() {
        float view_axis;
        bool judge;
        (view_axis, judge) = AxisOnce("Vertical");

        if (view_axis > 0 && judge && select > 0){
            select--;
        } else if (view_axis < 0 && judge && select < (buttons.Length-1)){
            select++;
        }

        for (int i = 0; i < buttons.Length; i++){
            buttons[i].selected = (i == select);
        }

        if (Input.GetButtonDown("A") || Input.GetButtonDown("Start")){
            buttons[select].accepted = true;

            switch (select) {
                case 0:
                source.PlayOneShot(continueSound);
                StartCoroutine(Fade(0));
                break;

                case 1:
                source.PlayOneShot(quitSound);
                StartCoroutine("SwitchStep");
                break;
            }
        }
    }
    void AreYouSure() {
        float view_axis;
        bool judge;
        (view_axis, judge) = AxisOnce("Horizontal");

        if (view_axis > 0 && judge && select2 < (buttons2.Length-1)){
            select2++;
        } else if (view_axis < 0 && judge && select2 > 0){
            select2--;
        }

        for (int i = 0; i < buttons2.Length; i++){
            buttons2[i].selected = (i == select2);
        }

        if (Input.GetButtonDown("A") || Input.GetButtonDown("Start")){
            buttons2[select2].accepted = true;
            
            switch (select2) {
                case 0:
                //いいえ
                source.PlayOneShot(cancelSound);
                StartCoroutine("SwitchStep");
                break;

                case 1:
                //はい
                source.PlayOneShot(acceptSound);
                StartCoroutine(Fade(1));
                break;
            }
        }
    }

    (float a, bool b) AxisOnce(string axis){
        bool judge = false;
        float view_axis = Input.GetAxis(axis);

        //RTが押されたら視点を変える：前フレームの入力値が0の場合のみ実施
        judge = Math.Abs(view_axis) > 0 && beforeAxis == 0.0f;
        beforeAxis = view_axis;

        return (view_axis, judge);
    }

    IEnumerator Fade(int select) {
        active = false;
        yield return new WaitForSeconds(0.5f);

        FadeManager.FadeSetUp(true, 1f);
        yield return new WaitForSeconds(2f);

        GameManager.gameOver = false;
        data.ResetAll();

        switch (select) {
            case 0:
            //つづける
            //ワールドマップに戻る。（残機は2にセット）
            SceneManager.LoadScene("WorldMap");
            break;

            case 1:
            //やめる
            //本当にやめますか？（タイトルに戻る）
            SceneManager.LoadScene("TitleScreen");
            break;
        }
    }

    IEnumerator SwitchStep() {
        active = false;
        yield return new WaitForSeconds(0.5f);

        areYouSure = !areYouSure;
        select2 = 0;

        if (areYouSure) {
            for (int i = 0; i < buttons.Length; i++){
                buttons2[i].accepted = false;
            }
        } else {
            for (int i = 0; i < buttons.Length; i++){
                buttons[i].accepted = false;
            }
        }
        nextStep.display = areYouSure ? 1 : -1;

        active = true;
    }
}
