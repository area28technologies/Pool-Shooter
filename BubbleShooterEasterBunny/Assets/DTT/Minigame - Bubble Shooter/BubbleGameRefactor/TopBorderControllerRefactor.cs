using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    public class TopBorderControllerRefactor : WallControllerRefactor
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            collision.TryGetComponent(out BubbleControllerRefactor otherBubble);
            if (otherBubble != null)
            {
                if (otherBubble.State == BubbleState.Flying)
                {
                    otherBubble.AttachTop();
                }
            }
        }
    }
}