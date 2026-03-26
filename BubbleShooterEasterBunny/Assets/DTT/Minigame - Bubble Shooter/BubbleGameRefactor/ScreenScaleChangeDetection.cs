using DTT.BubbleShooter;
using System.Collections;
using UnityEngine;

public class ScreenScaleChangeDetection : MonoBehaviour
{
    [SerializeField] float _safeRectWidth;

    [Header("Safe Rect UI")]
    [SerializeField] RectTransform _gridTopSafeRectUI;
    [SerializeField] RectTransform _gridBottomSafeRectUI;
    [SerializeField] RectTransform _gridLeftSafeRectUI;
    [SerializeField] RectTransform _gridRightSafeRectUI;

    private bool m_firstFrame = true;

    private void Start()
    {
        StartCoroutine(RefreshWithDelay());
    }

    private void LateUpdate()
    {
        if (m_firstFrame)
        {
            //This is executed just before rendering the first frame - ensures that the view is not very zoomed in when rendering the first frame
            Refresh();
            m_firstFrame = false;
        }
    }

    private IEnumerator RefreshWithDelay()
    {
        yield return new WaitForEndOfFrame();
        //This is executed after the first late update but before the next frame - now we have the final size
        Refresh();
    }

    private void Refresh()
    {
        _safeRectWidth = _gridRightSafeRectUI.transform.position.x - _gridLeftSafeRectUI.transform.position.x;
        FindAnyObjectByType<BubbleGameControllerRefactor>().ChangeScreenScaleHandle(_safeRectWidth);
    }
}
