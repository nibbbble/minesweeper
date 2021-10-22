using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField]
    TileManager tileManager;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    GuyButton guyButton;
    bool gameStarted, gameOver, win;
    int tileCount, mineCount;

    void Start() {
        Initialize();
    }

    void Update() {
        if (tileCount == 0 && mineCount == 0) {
            Victory();
        }
    }

    public void Initialize() {
        gameStarted = false;
        gameOver = false;
        win = false;

        tileManager.Initialize();
        uiManager.Initialize();
        guyButton.UpdateSprites(0);

        // probably not a good idea lol
        tileCount = tileManager.GetTileCount();
        mineCount = tileManager.GetMineCount();
        tileCount -= mineCount;
    }

    public void GameStart(Vector2Int gridPosition) {
        gameStarted = true;
        tileManager.GameStart(gridPosition);
        uiManager.StartTimer();
    }   

    public void GameOver(Vector2Int gridPosition) {
        gameOver = true;
        tileManager.GameOver(gridPosition);
        uiManager.StopTimer();
        guyButton.UpdateSprites(3);
    }

    public void Victory() {
        win = true;

        uiManager.StopTimer();
        guyButton.UpdateSprites(2);

        Debug.Log("WIN!");
    }

    public void Click() {
        tileCount--;

        Debug.LogFormat("Mine count: {0}; Remaining tile count: {1}", mineCount, tileCount);
    }

    public void Flag(int count) {
        mineCount += count;
        uiManager.UpdateMineCount(count);

        Debug.LogFormat("Mine count: {0}; Remaining tile count: {1}", mineCount, tileCount);
    }

    // -------------------------------------------

    public bool GameStartedBool() {
        return gameStarted;
    }

    public bool GameOverBool() {
        return gameOver;
    }

    public bool WinBool() {
        return win;
    }
}