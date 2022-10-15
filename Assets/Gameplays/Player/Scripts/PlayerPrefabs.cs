using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefabs", menuName = "ScriptableObjects/Player Prefabs")]
public class PlayerPrefabs : ScriptableObject
{
    [Serializable]
    public class PlayerData {
        public Sprite icon;
        public Color color;
        public string name;
        public GameObject prefab;
    }
    public PlayerData[] playerData = new PlayerData[17];
}
