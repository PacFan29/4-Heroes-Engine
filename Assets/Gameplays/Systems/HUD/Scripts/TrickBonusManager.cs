using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrickBonusManager : MonoBehaviour
{
    public static PlayerInfo player;
    public static int startBonus;
    private Text trickBonus;
    private Animator anim;
    private int index;
    private bool increased = true;

    private string[] bonusHeaders = {
        "Good!",
        "Nice!",
        "Great!",
        "Jammin\'!",
        "Cool!",
        "Radical!",
        "Tight!",
        "Awesome!",
        "Wonderful!",
        "Excellent!"
    };
    private int[] bonuses = {
        500,
        1000,
        1500,
        2000,
        2500,
        3000,
        4000,
        5000,
        7500,
        10000
    };
    private Color[] bonusColors = {
        new Color(0f, 0.75f, 1f, 0f),
        new Color(0f, 1f, 0.5f, 0f),
        new Color(0.25f, 1f, 0f, 0f),
        new Color(1f, 0.9f, 0f, 0f),
        new Color(1f, 0.72f, 0f, 0f),
        new Color(1f, 0.54f, 0f, 0f),
        new Color(1f, 0.36f, 0f, 0f),
        new Color(1f, 0.18f, 0f, 0f),
        new Color(1f, 0f, 0f, 0f),
        new Color(1f, 0f, 1f, 0f)
    };
    // Start is called before the first frame update
    void Awake()
    {
        trickBonus = GetComponent<Text>();
        anim = GetComponent<Animator>();
        anim.enabled = false;
    }

    void Update() {
        if (startBonus > 0) {
            if (!increased) player.scoreIncrease(bonuses[index]);

            index = startBonus - 1;
            GetClassification();
            startBonus = 0;
        }
    }

    public void GetClassification() {
        increased = false;
        if (BonusValue() >= 100000) {
            trickBonus.text = "Unbelievable!";
            trickBonus.color = new Color(0.8f, 1.0f, 0.95f, 1.0f);
        } else {
            trickBonus.text = bonusHeaders[Math.Min(index, 9)];
            trickBonus.color = bonusColors[Math.Min(index, 9)];
        }

        anim.enabled = true;
        anim.Play("TrickBonus", 0, 0);
    }
    public void GetPoints() {
        trickBonus.text = BonusValue().ToString("+###,###,##0");
	}
    public void ScoreIncrease() {
        increased = true;
        player.scoreIncrease(BonusValue());
    }

    int BonusValue() {
        if (index >= 9) {
            return (index - 5) * 2500;
        } else {
            return bonuses[index];
        }
    }
}
