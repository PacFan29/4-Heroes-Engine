using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAirAmounts : MonoBehaviour
{
    public PlayerInfo info;
    public int targetPlayer = 0;
    [Header("出力")]
    public Image fill;
    public Text countDown;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            if (player.GetComponent<PlayerInfo>() != null) {
                if (player.GetComponent<PlayerInfo>().playerNumber == targetPlayer) {
                    info = player.GetComponent<PlayerInfo>();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (info != null) {
            this.GetComponent<CanvasGroup>().alpha = (info.underwater && info.HP > 0) ? 1f : 0f;

            this.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, info.transform.position);

            float percent = info.airAmount / info.maxAirAmount;
            fill.fillAmount = percent;

            countDown.gameObject.SetActive(info.airAmount <= 0 && info.airCountdown >= 0);
            countDown.text = (info.airCountdown).ToString();
        }
    }
}
