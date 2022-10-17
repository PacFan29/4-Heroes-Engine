using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GoalManager : MonoBehaviour
{
    [Header("データ")]
    public GameData data;

    protected void GoToResult() {
        data.ResetCheckPoint();
        SceneManager.LoadScene("ResultScreen");
    }

    public void cleared(GameObject player) {
        MusicManager.musicFade = true;
        GameManager.timeIncrease = false;
        StartCoroutine(player.GetComponent<PlayerInfo>().CourseCleared());
    }
}
