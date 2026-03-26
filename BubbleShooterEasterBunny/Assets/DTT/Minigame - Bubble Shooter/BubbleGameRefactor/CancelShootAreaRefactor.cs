using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DTT.BubbleShooter
{
    public class CancelShootAreaRefactor : MonoBehaviour, IPointerEnterHandler
    {
        private TurretControllerRefactor _turretController;
        public void Init(TurretControllerRefactor turretController)
        {
            _turretController = turretController;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _turretController.DisableTurret();
        }

        
    }
}