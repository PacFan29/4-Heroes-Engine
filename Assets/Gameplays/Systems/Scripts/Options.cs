using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Options", menuName = "ScriptableObjects/Options")]
public class Options : ScriptableObject
{
    [Header("サウンド")]
    public float musicVolume;
    public float soundVolume;
    public float voiceVolume;
    
    [Header("その他")]
    public bool beginnerMode;
    public bool lives;
}
