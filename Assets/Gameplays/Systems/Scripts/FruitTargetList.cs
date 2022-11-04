using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitTargetList : MonoBehaviour
{
    public RectTransform group;
    public GameObject item;
    public Sprite[] sprites = new Sprite[10];

    private List<ItemAmounts> list = new List<ItemAmounts>();
    private int latestTotal = 10;

    private bool toggle = false;
    private float hideTime = 0f;
    private float showTime = 5f;

    // Update is called once per frame
    void Update()
    {
        //リストの更新
        int total = 0;
        foreach (int amounts in GameManager.fruits) {
            total += amounts;
        }
        if (latestTotal != total) {
            list = new List<ItemAmounts>();

            int count = 0;
            ItemAmounts stk = item.GetComponent<ItemAmounts>();
            stk.sprite = sprites[0];
            stk.amounts = GameManager.fruits[0];
            stk.index = 0;
            if (GameManager.fruits[0] > 0) {
                list.Add(stk);
                count = 1;
            }
            stk.gameObject.SetActive(GameManager.fruits[0] > 0);

            foreach (Transform obj in group) {
                if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                    Destroy(obj.gameObject);
                }
            }
            for (int i = 1 ; i < GameManager.fruits.Length; i++) {
                if (GameManager.fruits[i] > 0) {
                    RectTransform scoreimage = (RectTransform)Instantiate(item).transform;
                    scoreimage.SetParent(group, false);
                    scoreimage.localPosition = new Vector2(
                        scoreimage.localPosition.x,
                        scoreimage.localPosition.y - scoreimage.sizeDelta.y * count);
                    count++;

                    stk = scoreimage.gameObject.GetComponent<ItemAmounts>();
                    stk.index = i;
                    stk.sprite = sprites[i];
                    stk.amounts = GameManager.fruits[i];
                    list.Add(stk);
                }
            }
            latestTotal = total;

            hideTime = 2f;
        }

        bool toggleTrigger = false;
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < allPlayers.Length; i++){
            //プレイヤー全員分1UP
            if (allPlayers[i].GetComponent<PlayerInfo>() != null){
                PlayerInfo player = allPlayers[i].GetComponent<PlayerInfo>();
                if (player.playerNumber == 0) {
                    toggleTrigger = !(player.finalVelocity.magnitude > 0f);
                }
            }
        }

        //トグル
        if (hideTime > 0) {
            toggle = true;
            hideTime -= Time.deltaTime;
        } else {
            if (!toggle && toggleTrigger) {
                showTime -= Time.deltaTime;
                if (showTime <= 0f) {
                    toggle = true;
                }
            } else if (toggle && !toggleTrigger) {
                toggle = false;
                showTime = 5f;
            }
        }
        if (toggle || GameManager.isPaused) {
            group.localPosition = Vector3.Lerp(group.localPosition, Vector3.zero, 0.2f);
            if (toggleTrigger) showTime = 0f;
        } else {
            group.localPosition = Vector3.Lerp(group.localPosition, Vector3.right * 70f, 0.2f);
        }
    }
}
