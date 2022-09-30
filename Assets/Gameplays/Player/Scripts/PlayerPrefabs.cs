using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefabs", menuName = "ScriptableObjects/Player Prefabs")]
public class PlayerPrefabs : ScriptableObject
{
    public GameObject[] Prefabs = new GameObject[17];
}
