using UnityEngine;
using UnityEngine.EventSystems;

namespace DTT.BubbleShooter
{
    public class CancelShootArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static bool isCanShoot = true;

        public void OnPointerEnter(PointerEventData eventData)
        {
            isCanShoot = false;
            Debug.Log("OnPointerEnter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isCanShoot = true;
            this.gameObject.SetActive(false);
            Debug.Log("OnPointerExit");
        }
    }
}