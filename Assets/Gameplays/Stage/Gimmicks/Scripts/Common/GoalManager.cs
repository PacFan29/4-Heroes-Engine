using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GoalManager : MonoBehaviour
{
    [Header("データ")]
    public GameData data;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void GoToResult() {
        data.ResetCheckPoint();
        SceneManager.LoadScene("ResultScreen");
    }
}
