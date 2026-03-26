using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    public class ICloneable<T>
    {
        private T objectToClone;

        public T Clone {  get {
                Debug.Log(this);
                
                return objectToClone; 
            } }
    }
}