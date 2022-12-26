using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ElRaccoone.Tweens;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image _localizingImage;

    public void EnableLocalizationImage()
    {
        if (!_localizingImage.gameObject.activeSelf)
        {
            _localizingImage.gameObject.SetActive(true);    
            _localizingImage.gameObject.TweenAnchoredPositionY(gameObject.transform.localPosition.y + 100, 1).SetPingPong().SetInfinite();
        }
    }
    
    public void DisableLocalizationImage()
    {
        _localizingImage.gameObject.SetActive(false);
    }
}
