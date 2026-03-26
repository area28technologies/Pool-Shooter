using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
    [SerializeField] private CanvasGroup targetGroup;
    [SerializeField] private GameObject panel;
    public float fadeDuration = 1f;
    public void ToggleVisibility(bool isOn)
    {
        if (isOn)
        {
            StartCoroutine(FadeCanvasGroup(targetGroup, 0 , 1));
        }
        else
        {
            StartCoroutine(FadeCanvasGroup(targetGroup, 1, 0));
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        canvasGroup.alpha = startAlpha;
        if (endAlpha > 0)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
        if (endAlpha == 0)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
