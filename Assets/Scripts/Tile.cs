// for right clicks:
// https://forum.unity.com/threads/can-the-ui-buttons-detect-a-right-mouse-click.279027/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    TileSprites tileSprites;
    TileManager tileManager;
    StateManager stateManager;
    
    // if tile isnt mine this determines the number
    int mineNumber;
    bool hasMine, flagged, clicked, initialized;
    Vector2Int gridPosition;
    [SerializeField]
    Button button;
    Image image;

    void Start() {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        clicked = false;
        flagged = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (!stateManager.GameStartedBool()) {
                GameStart();
                Click();
                return;
            } else
                if (!stateManager.GameOverBool()) {
                    if (!clicked)
                        Click();
                }
        }
        
        if (eventData.button == PointerEventData.InputButton.Right) {
            if (stateManager.GameStartedBool())
                Flag();
        }
    }

    // ---------------------------------------
    
    public void Click() {
        if (!flagged) {
            clicked = true;
            stateManager.Click();
            if (hasMine) {
                if (stateManager.GameOverBool()) {
                    image.sprite = tileSprites.mineTile;
                } else {
                    image.sprite = tileSprites.mineGameOverTile;
                    stateManager.GameOver(gridPosition);
                }
            } else {
                if (mineNumber > 0) {
                    image.sprite = tileSprites.numberedTiles[mineNumber - 1];
                } else {
                    tileManager.EmptyTileClick(gridPosition);
                    
                    image.sprite = tileSprites.emptyTile;
                }
            }
        } else {
            if (stateManager.GameOverBool()) {
                if (!hasMine) {
                    image.sprite = tileSprites.mineWrongFlagTile;
                }
            }
        }
    }

    public void Flag() {
        if (!clicked && !stateManager.WinBool()) {
            int count;
            
            if (!flagged) {
                image.sprite = tileSprites.flaggedTile;
                flagged = true;
                count = -1;
            } else {
                image.sprite = tileSprites.hiddenTile;
                flagged = false;
                count = 1;
            }

            stateManager.Flag(count);
        }
    }

    public void GameOver() {
        Click();
    }

    public bool ClickedBool() {
        return clicked;
    }

    // ---------------------------------------

    // idk why i decided to do it this way
    // i just wrote this a minute ago and i still dont know
    public void SetManagers(TileManager t, StateManager s) {
        tileManager = t;
        stateManager = s;
    }

    public void SetGridPosition(Vector2Int _gridPosition) {
        gridPosition = _gridPosition;
    }

    void GameStart() {
        if (stateManager) {
            stateManager.GameStart(gridPosition);
        }
    }

    public void SetMine() {
        hasMine = true;
    }

    public void SetEmpty(int _mineNumber) {
        hasMine = false;
        mineNumber = _mineNumber;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
