using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockItemList : MonoBehaviour
{
    public GameData data;
    public GameObject item;

    private List<StockItem> list = new List<StockItem>();
    // Start is called before the first frame update
    void Awake()
    {
        StockItem stk = item.GetComponent<StockItem>();
        stk.index = 0;
        stk.special = data.stockItems[0].special;
        list.Add(stk);

        int count = 1;
        for (int i = 1 ; i < data.stockItems.Length; i++) {
            if (!data.stockItems[i].megaManOnly) {
                RectTransform scoreimage = (RectTransform)Instantiate(item).transform;
                scoreimage.SetParent(this.transform , false);
                scoreimage.localPosition = new Vector2(
                    scoreimage.localPosition.x + scoreimage.sizeDelta.x * count ,
                    scoreimage.localPosition.y);
                count++;

                stk = scoreimage.gameObject.GetComponent<StockItem>();
                stk.index = i;
                stk.special = data.stockItems[i].special;
                list.Add(stk);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (StockItem item in list) {
            item.sprites = data.stockItems[item.index].sprites;
            item.amounts = data.stockItems[item.index].amounts;
        }
    }
}
