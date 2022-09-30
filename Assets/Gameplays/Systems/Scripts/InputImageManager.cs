using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AxisEnum {
    LeftStick, RightStick, Left, Up,
    Right, Down, LT, RT
}
public enum ButtonEnum {
    A, B, X,
    Y, LB, RB
}
public class InputImageManager : MonoBehaviour
{
    private Image image;
    public Sprite[] AxisImages = new Sprite[8];
    public Sprite[] ButtonImages = new Sprite[6];
    public AxisEnum axisList;
    public ButtonEnum buttonList;
    public bool isButton;
    public string access = "";
    private Dictionary<string, int> Axises = new Dictionary<string, int>() {
        {"LeftStick", 0},
        {"RightStick", 1},
        {"Left", 2},
        {"Up", 3},
        {"Right", 4},
        {"Down", 5},
        {"LT", 6},
        {"RT", 7}
    };
    private Dictionary<string, int> Buttons = new Dictionary<string, int>() {
        {"A", 0},
        {"B", 1},
        {"X", 2},
        {"Y", 3},
        {"LB", 4},
        {"RB", 5}
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        image = GetComponent<Image>();
        if (access != "") {
            try {
                image.sprite = AxisImages[Axises[access]];
            } catch (KeyNotFoundException) {
                try {
                    image.sprite = ButtonImages[Buttons[access]];
                } catch (KeyNotFoundException) {
                    return;
                }
            }
        } else {
            if (isButton) {
                image.sprite = ButtonImages[Buttons[buttonList.ToString()]];
            } else {
                image.sprite = AxisImages[Axises[axisList.ToString()]];
            }
        }
    }
}
