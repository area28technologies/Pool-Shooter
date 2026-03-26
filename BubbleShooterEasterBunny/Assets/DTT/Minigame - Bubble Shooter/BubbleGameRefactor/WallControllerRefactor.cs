using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    public class WallControllerRefactor : MonoBehaviour
    {
        private BubbleGameControllerRefactor _bubbleGameController;

        public void Init(BubbleGameControllerRefactor bubbleGameController)
        {
            _bubbleGameController = bubbleGameController;
        }
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            collision.TryGetComponent(out BubbleControllerRefactor otherBubble);
            if (otherBubble != null)
            {
                if (otherBubble.State == BubbleState.Flying)
                {
                    otherBubble.ChangeDirection();
                    RCGSoundSetting.Instance.PlaySFX(_bubbleGameController.BallBoundAudioClips);
                }
            }
        }

    }
}