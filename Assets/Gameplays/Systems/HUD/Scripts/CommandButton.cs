using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandButton : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[2];
    public bool selected;
    private Image button;
    private RectTransform rect;
    private Shadow shadow;
    private Animator anim;

    public GameObject selectArrow;
    [HideInInspector] public bool accepted;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        shadow = GetComponent<Shadow>();

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected) {
            button.sprite = sprites[1];
            rect.localScale = new Vector3(1.05f, 1.05f, 1f);
        } else {
            button.sprite = sprites[0];
            rect.localScale = Vector3.one;
        }
        shadow.enabled = selected;
        selectArrow.SetActive(selected && !accepted);

        anim.SetBool("Accepted", accepted);
    }
}
