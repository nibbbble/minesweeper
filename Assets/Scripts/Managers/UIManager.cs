using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TileManager tileManager;
    [SerializeField]
    Text mineCountText, timerText;
    int mineCount, maxMineCount;
    float timer;
    bool timeStart;
    
    public void Initialize() {
        maxMineCount = tileManager.GetMineCount();
        mineCount = maxMineCount;
        UpdateMineCount(0);

        timer = 0f;
        timeStart = false;
    }

    void Update() {
        // https://answers.unity.com/questions/45676/making-a-timer-0000-minutes-and-seconds.html
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer - minutes * 60f);
        int milliseconds = Mathf.FloorToInt((timer * 1000f) % 1000);
        timerText.text = string.Format(
            "{0:0}:{1:00}:{2:000}", minutes, seconds, milliseconds
        );

        if (timeStart) {
            timer += Time.deltaTime;
        }
    }

    public void UpdateMineCount(int count) {
        mineCount += count;
        mineCountText.text = string.Format("{0}", mineCount);
    }

    public void StartTimer() {
        timeStart = true;
    }

    public void StopTimer() {
        timeStart = false;
    }
}
