using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public CommandButton[] buttons = new CommandButton[3];
    private int select = 0;
    private float beforeAxis = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Animator[] animators = this.GetComponentsInChildren<Animator>();

        for (int i = 0; i < animators.Length; i++){
			animators[i].updateMode = AnimatorUpdateMode.UnscaledTime;
		}
    }
    void Awake() {
        select = 0;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    (float a, bool b) AxisOnce(string axis){
        bool judge = false;
        float view_axis = Input.GetAxis(axis);

        //RTが押されたら視点を変える：前フレームの入力値が0の場合のみ実施
        judge = Math.Abs(view_axis) > 0 && beforeAxis == 0.0f;
        beforeAxis = view_axis;

        return (view_axis, judge);
    }
}
