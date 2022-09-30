using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MegaManHealth : MonoBehaviour
{
    public MegaManHealthBar healthBar;
    public MegaManHealthBar weaponBar;
    public Text lives;
    [HideInInspector] public int playerNo = 0;

    // Update is called once per frame
    void Update()
    {
        healthBar.maxValue = GameManager.players[playerNo].getStatus()[1];
        healthBar.currentValue = GameManager.players[playerNo].getStatus()[2];

        weaponBar.maxValue = 28;
        weaponBar.currentValue = 28;
        weaponBar.gameObject.SetActive(false);

        lives.text = GameManager.players[playerNo].getStatus()[3].ToString();
    }
}
