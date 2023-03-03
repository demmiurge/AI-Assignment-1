using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteToggle : MonoBehaviour
{
    [SerializeField] private Sprite[] _buttonSprites;
    [SerializeField] private Image _targetButton;
    
    public void ToggleSprite()
    {
        if (_targetButton.sprite == _buttonSprites[0])
        {
            _targetButton.sprite = _buttonSprites[1];
            return;
        }

        _targetButton.sprite = _buttonSprites[0];
    }
}
