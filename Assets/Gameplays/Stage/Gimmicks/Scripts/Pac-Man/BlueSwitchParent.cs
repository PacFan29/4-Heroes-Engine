using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GimmickType {
    Gimmick,
    Cookies
}
[Serializable]
public class GimmickList {
    public GimmickType gimmickType;
    public GimmickManager gimmick;
    public GameObject cookies;
    public int index;
}

public class BlueSwitchParent : MonoBehaviour
{
    private List<BlueSwitch> switches = new List<BlueSwitch>();
    private int step = 0;

    public GimmickList[] gimmickList = new GimmickList[5];
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform s in this.transform) {
            if (s.GetChild(0).gameObject.GetComponent<BlueSwitch>() != null) {
                BlueSwitch blue = s.GetChild(0).gameObject.GetComponent<BlueSwitch>();
                switches.Add(blue);
                if (blue.index != 0) {
                    blue.pressed = true;
                }
            }
        }
        foreach (GimmickList gim in gimmickList) {
            if (gim.cookies != null) {
                gim.cookies.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (step < switches.Count) {
            if (switches[step].pressed) {
                //〇番目のスイッチが押された時
                foreach (GimmickList gim in gimmickList) {
                    if (step == gim.index) {
                        switch (gim.gimmickType) {
                            case GimmickType.Gimmick:
                            if (gim.gimmick != null) {
                                gim.gimmick.gimmick = true;
                            }
                            break;

                            case GimmickType.Cookies:
                            if (gim.cookies != null) {
                                gim.cookies.SetActive(true);
                            }
                            break;
                        }
                    }
                }

                step++;
                if (step < switches.Count) {
                    switches[step].pressed = false;
                }
            }
        }
    }
}
