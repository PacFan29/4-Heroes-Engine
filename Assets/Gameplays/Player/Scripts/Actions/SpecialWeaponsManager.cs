using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialWeaponsManager : MonoBehaviour
{
    private MegaManActions megaMan;
    private PlayerInfo info;

    private int weaponId;
    // Start is called before the first frame update
    void Start()
    {
        megaMan = GetComponent<MegaManActions>();
        info = GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        weaponId = megaMan.weaponId - 1;

        int count = GameObject.FindGameObjectsWithTag("SpecialWeapon").Length;
        
        if (info.ButtonsDown["X"] && (weaponId >= 0 && weaponId <= 7)){
            SpecialWeapon weapon = megaMan.weapons.weapons[weaponId];
            bool canShoot = (weapon.isSingle && count < 1) || !weapon.isSingle;

            if (weapon.isConsumable() && canShoot) {
                weapon.Consume();
                
                if (weapon.prefab != null) {
                    if (weaponId == 4) {
                        for (int i = 0; i < 3; i++) {
                            Vector3 rot = info.skin.rotation.eulerAngles;

                            switch (i) {
                                case 1:
                                rot += new Vector3(0f, -30f, 0f);
                                break;

                                case 2:
                                rot += new Vector3(0f, 30f, 0f);
                                break;
                            }
                            WeaponMovements wp = Instantiate(weapon.prefab, this.transform.position, Quaternion.Euler(rot)).GetComponent<WeaponMovements>();
                            wp.VelocityChange(1, (i > 0) ? 15f : 20f);
                            wp.player = info;
                        }
                    } else {
                        WeaponMovements wp = Instantiate(weapon.prefab, this.transform.position, info.skin.rotation).GetComponent<WeaponMovements>();
                        wp.player = info;
                    }
                }
            }
        }
    }
}
