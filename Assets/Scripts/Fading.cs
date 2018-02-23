using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fading : MonoBehaviour {

    public Image black;
    public float defaultFadeTime;

    public void FadeIn(float fadeTime = 0f)
    {
        if (fadeTime == 0f)
        {
            fadeTime = defaultFadeTime;
        }
        StartCoroutine(DoFadeIn(fadeTime));
    }

     IEnumerator DoFadeIn(float fadeTime)
    {
        float startTime = 0f;
        float perc = 0;
        Color originalColor = black.color;
        while (startTime <= fadeTime) {
            perc = startTime / fadeTime;
            black.color = Vector4.Lerp(originalColor, new Vector4(0f, 0f, 0f, 0f), perc);
            startTime += Time.deltaTime;
            yield return null;
        }
        black.color = new Vector4(0f, 0f, 0f, 0f);
    }
}
