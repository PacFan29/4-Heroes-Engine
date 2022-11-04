using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemAmounts : MonoBehaviour
{
    public int amounts;
    [HideInInspector] public Sprite sprite;
    [HideInInspector] public int index;
    [Header("アイコン")]
    public Image icon;
    [Header("個数")]
    public Text amountText;

    void Start() {
        icon.sprite = sprite;
    }
    // Update is called once per frame
    void Update()
    {
        amountText.text = amounts.ToString();
        amountText.gameObject.SetActive(amounts > 1);
        icon.sprite = sprite;

        icon.gameObject.SetActive(amounts > 0);
    }
}
