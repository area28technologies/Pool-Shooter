using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    public class RandomShiningEffectController : MonoBehaviour
    {
        [SerializeField] SpriteRenderer bubbleSpriteRenderer; //if this equal null hide effect
        [SerializeField] GameObject shiningEffectObject;
        [SerializeField] float minTime = 10;
        [SerializeField] float maxTime = 20;
        [SerializeField] float randomPositionRadius = 1;
        [SerializeField] float randomRotationRadius = 180;
        [SerializeField] float randomScaleRadius = 0.3f;
        [SerializeField] bool isValidate = false;

        private Animator animator;
        private Coroutine animationCoroutine;
        private Vector3 shiningEffectOriginPosition;
        private Vector3 shiningEffectOriginRotation;
        private Vector3 shiningEffectOriginScale;
        private void OnEnable()
        {
            Init();
            StopAnimation();
            animationCoroutine = StartCoroutine(PlayAnimationCoroutine());
        }

        private void Init()
        {
            animator = shiningEffectObject.GetComponent<Animator>();
            shiningEffectOriginPosition = shiningEffectObject.transform.localPosition;
            shiningEffectOriginRotation = shiningEffectObject.transform.localRotation.eulerAngles;
            shiningEffectOriginScale = shiningEffectObject.transform.localScale;
            Validate();
        }

        private void Validate()
        {
            if (ReferenceEquals(bubbleSpriteRenderer.sprite, null))
            {
                isValidate = false;
                return;
            }
            if (animator != null)
            {
                if (animator.runtimeAnimatorController.animationClips[0] != null)
                {
                    isValidate = true;
                    return;
                }
            }
            else
            {
                animator = shiningEffectObject.GetComponent<Animator>();
                if (animator != null)
                {
                    if (animator.runtimeAnimatorController.animationClips[0] != null)
                    {
                        isValidate = true;
                        return;
                    }
                }
                else
                {
                    isValidate = false;
                    return;

                }
            }
            isValidate = false;
        }

        IEnumerator PlayAnimationCoroutine()
        {
            StopAnimation();
            float randomTime = Random.Range(minTime, maxTime); 
            yield return new WaitForSeconds(randomTime);

            float randomDeltaX = Random.Range(-randomPositionRadius, randomPositionRadius);
            float randomDeltaY = Random.Range(-randomPositionRadius, randomPositionRadius);
            float randomDeltaAngel = Random.Range(-randomRotationRadius, randomRotationRadius);
            float randomDeltaScale = Random.Range (-randomScaleRadius, randomScaleRadius);
            
            shiningEffectObject.transform.localPosition = new(shiningEffectOriginPosition.x + randomDeltaX, shiningEffectOriginPosition.y + randomDeltaY);
            shiningEffectObject.transform.localRotation = Quaternion.Euler(shiningEffectOriginRotation.x, shiningEffectOriginRotation.y, shiningEffectOriginRotation.z + randomDeltaAngel);
            shiningEffectObject.transform.localScale = new(shiningEffectOriginScale.x, shiningEffectOriginScale.y);

            PlayAnimation();
            yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[0].length);
            StopAnimation();
            animationCoroutine = StartCoroutine(PlayAnimationCoroutine());
        }

        public void PlayAnimation()
        {
            Validate();
            if (!isValidate) return;
            shiningEffectObject.SetActive(true);
            animator.Play(0);
        }

        public void StopAnimation()
        {
            shiningEffectObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}