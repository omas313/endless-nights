using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FearPanel : MonoBehaviour
{
    [SerializeField] RectTransform _fillBar;
    [SerializeField] RectTransform _bgBar;
    [SerializeField] TextMeshProUGUI _text;

    float _totalWidth;
    
    public void SetFearLevel(float percent)
    {
        var size = new Vector2(percent * _totalWidth, _fillBar.sizeDelta.y);
        _fillBar.sizeDelta = size;

        _text.fontSize = 25f + percent * 20f;

        if (percent >= 1f)
        {
            var darkRed = new Color(0.75f, 0f, 0f, 1f);
            _fillBar.GetComponent<Image>().color = darkRed;
            _text.color = darkRed;
        }
    }

    void Awake()
    {
        _totalWidth = _bgBar.rect.width;
    }
}
