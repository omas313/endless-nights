using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextLinesPlayer : MonoBehaviour
{
    public event Action Finished;

    [SerializeField] [TextArea(3,5)] string[] _lines;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] CanvasGroup _canvasGroup;

    public IEnumerator PlayLines()
    {
        yield return FadeInCanvas();

        for (int i = 0; i < _lines.Length; i++)
        {
            _text.SetText(_lines[i].Trim());

            yield return new WaitUntil(() => Input.GetButtonDown("Fire1"));
            yield return new WaitForSeconds(0.15f);
        }

        yield return FadeOutCanvas();
        Finished?.Invoke();
    }

    IEnumerator FadeInCanvas()
    {
        while (_canvasGroup.alpha < 1f)
        {
            _canvasGroup.alpha += Time.deltaTime * 2f;
            yield return null;
        }
    }

    IEnumerator FadeOutCanvas()
    {
        while (_canvasGroup.alpha > 0f)
        {
            _canvasGroup.alpha -= Time.deltaTime * 2f;
            yield return null;
        }
    }
}
