using UnityEngine;
using UnityEngine.UI;

public class SoundSwitch : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _enabledSprite;
    [SerializeField] private Sprite _disabledSprite;

    public void SetState(bool isEnabled)
    {
        var sprite = isEnabled
            ? _enabledSprite 
            : _disabledSprite;

        _image.sprite = sprite;
    }
}
