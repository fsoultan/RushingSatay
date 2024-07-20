using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReactionText : MonoBehaviour
{
    public static ReactionText instance;
    TextMeshProUGUI tmp;
    Coroutine coroutine;
    private void Awake()
    {
        instance = this;
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.color = Color.clear;
    }

    public void SetText(string text)
    {
        tmp.text = text;
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        tmp.color = Color.white;
        yield return new WaitForSeconds(3f);
        tmp.color = Color.clear;
    }
}
