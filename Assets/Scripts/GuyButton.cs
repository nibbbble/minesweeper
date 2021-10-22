using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuyButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    StateManager stateManager;
    [SerializeField]
    GuySprites sprites;
    Sprite[] currentSprite;
    [SerializeField]
    Image image;
    [SerializeField]
    Button button;

    public void UpdateSprites(int mode) {
        switch (mode) {
            case (0): 
                currentSprite = sprites.normal;
                break;
            case (1): 
                currentSprite = sprites.curious;
                break;
            case (2): 
                currentSprite = sprites.cool;
                break;
            case (3): 
                currentSprite = sprites.dead;
                break;
        }

        image.sprite = currentSprite[0];
        SpriteState buttonState = button.spriteState;
        buttonState.pressedSprite = currentSprite[1];
        button.spriteState = buttonState;
    }
    
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            stateManager.Initialize();
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            stateManager.Initialize();
        }
    }
}
