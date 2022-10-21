using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject[] UIPrefabs = new GameObject[9];
    public GameObject disactive;
    public PlayerPrefabs playerPrefs;
    public GameData data;
    // Start is called before the first frame update
    void Awake()
    {
        if (UIPrefabs != null) {
            Instantiate(UIPrefabs[(int)data.HUDStyle], this.transform.position, Quaternion.identity);
        }
        disactive.SetActive(false);

        int playerId = 0;
        int playerLength = 0;
        foreach (int id in data.characterIds) {
            if (id < 0) {
                break;
            } else {
                playerLength++;
            }
        }

        bool sonicStage = false;
        if (data.HUDType == GameType.Sonic || data.HUDType == GameType.SonicBoss) {
            playerId = 16;
            playerLength = 1;
            sonicStage = true;
        }

        Vector3 spawnPos;
        if (data.isCpPassed) {
            spawnPos = data.CpPosition;
        } else {
            spawnPos = this.transform.position;
        }
        spawnPos += Vector3.left * (2f * (playerLength - 1));
        
        int len = sonicStage ? 1 : playerLength;
        for (int i = 0; i < len; i++) {
            if (!sonicStage) {
                playerId = data.characterIds[i];
                playerId = Mathf.Clamp(playerId, -1, 15);
            }

            if (playerId >= 0) {
                PlayerInfo pl = Instantiate(playerPrefs.playerData[playerId].prefab, spawnPos, Quaternion.identity).GetComponent<PlayerInfo>();
                pl.playerNumber = i;
                spawnPos += Vector3.right * 4f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
