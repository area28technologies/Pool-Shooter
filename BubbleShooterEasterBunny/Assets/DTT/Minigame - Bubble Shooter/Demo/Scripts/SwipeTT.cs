
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwipeTT : MonoBehaviour
{
    [SerializeField] private CanvasGroup panelCanvasGroupTT;
    [SerializeField] private CanvasGroup panelCanvasGroupMain;
    [SerializeField] private CanvasGroup[] images;

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Text pageIndicator;

    [SerializeField] private float fadeDuration = 0.5f;
    private int currentIndex = 0;

    private void Start()
    {
        panelCanvasGroupTT.alpha = 0;
        panelCanvasGroupTT.interactable = false;
        panelCanvasGroupTT.blocksRaycasts = false;
    }

    public void OpenPanel()
    {
        for(int i = 0; i < images.Length; i++)
        {
            images[i].alpha = 0f;
            images[i].interactable = false;
            images[i].blocksRaycasts = false;
        }

        SetActiveImage(0, true);
        StartCoroutine(SwapCanvasGroups(panelCanvasGroupMain, panelCanvasGroupTT));
    }

    public void ClosePanel()
    {
        StartCoroutine(SwapCanvasGroups(panelCanvasGroupTT, panelCanvasGroupMain));
    }

    public void NextImage()
    {
        if (currentIndex < images.Length - 1)
        {
            SetActiveImage(currentIndex + 1);
        }
    }

    public void PrevImage()
    {
        if (currentIndex > 0)
        {
            SetActiveImage(currentIndex - 1);
        }
    }

    private void SetActiveImage(int index, bool immediate = false)
    {
        StartCoroutine(SwapCanvasGroups(images[currentIndex], images[index], immediate));
        currentIndex = index;

        pageIndicator.text = $"{currentIndex + 1}/{images.Length}";

        prevButton.gameObject.SetActive(index > 0);
        nextButton.gameObject.SetActive(index < images.Length - 1);
    }

    private IEnumerator SwapCanvasGroups(CanvasGroup fromGroup, CanvasGroup toGroup, bool immediate = false)
    {
        fromGroup.interactable = false;
        fromGroup.blocksRaycasts = false;
        toGroup.interactable = true;
        toGroup.blocksRaycasts = true;

        if(fromGroup == toGroup || immediate)
        {
            fromGroup.alpha = 0f;
            toGroup.alpha = 1f;

            yield break;
        }

        float timestamp = Time.time;
        float t = 0f;

        while (t < fadeDuration)
        {
            t = Time.time - timestamp;
            float alpha = Mathf.Min(1f, t / fadeDuration);

            fromGroup.alpha = 1f - alpha;
            toGroup.alpha = alpha;

            yield return null;
        }
    }
}
