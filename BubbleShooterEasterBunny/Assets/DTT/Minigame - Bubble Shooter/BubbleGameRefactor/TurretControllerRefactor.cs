using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DTT.BubbleShooter
{
    public class TurretControllerRefactor : MonoBehaviour
    {
        [Header("Init Data")]
        [SerializeField] float _minAngle = -60;
        [SerializeField] float _maxAngle = 60;

        [Header("Runtime Data")]
        [SerializeField] bool _isShootable = true;
        [SerializeField] bool _isSwapable = true;
        [SerializeField] bool _isRotateable = true;
        [SerializeField] bool _isSwapping = false;

        [Header("Child Component")]
        [SerializeField] BubbleLoader _loadedBubble;
        [SerializeField] BubbleLoader _pendingBubble;
        [SerializeField] BubbleLoader _shootedBubble;
        [SerializeField] Transform _barrel;
        [SerializeField] Button _swapButton;
        [SerializeField] CancelShootAreaRefactor _cancelShootArea;

        private BubbleGameControllerRefactor _bubbleGameController;

        public bool IsShootable { get => _isShootable; }
        public bool IsSwapable { get => _isSwapable; }

        public void Init(BubbleGameControllerRefactor gameControllerRefactor)
        {
            _bubbleGameController = gameControllerRefactor;
            _cancelShootArea.Init(this);
            Reload();
        }

        private void Update()
        {
            if (_bubbleGameController == null) return;
            if (_bubbleGameController.GameState != GameState.Playing) { return; }
            CheckShoot();
            RotateBarrelFollowMouse();
        }

        public void CheckShoot()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (_isShootable )
                {
                    if (_loadedBubble.Bubble != null && _pendingBubble.Bubble != null && !_isSwapping)
                    {
                        DisableTurret();
                        _shootedBubble.Bubble = _loadedBubble.Bubble;
                        _loadedBubble.Bubble = null;
                        _shootedBubble.Bubble.ChangeToState(BubbleState.Flying);
                        _shootedBubble.Bubble.transform.parent = _bubbleGameController.BubbleGridHolder;
                        _shootedBubble.Bubble.transform.position = _barrel.position;

                        float dirX = -Mathf.Sin(((Mathf.Repeat(_barrel.transform.rotation.eulerAngles.z + 180, 360) - 180) * Mathf.PI) / 180);
                        float dirY = Mathf.Cos(((Mathf.Repeat(_barrel.transform.rotation.eulerAngles.z + 180, 360) - 180) * Mathf.PI) / 180);
                        _shootedBubble.Bubble.Direction = new Vector3(dirX, dirY);
                        DisableTurret();

                        RCGSoundSetting.Instance.PlaySFX(_bubbleGameController.ShotAudioClips);

                        //Update difficult
                        _bubbleGameController.ShotsCount++;
                    }
                }
                _cancelShootArea.gameObject.SetActive(false);
                if (_loadedBubble.Bubble != null && _pendingBubble.Bubble != null && !_isSwapping) EnableTurret();
            }
        }

        public void Reload()
        {
            if (_pendingBubble.Bubble == null)
            {
                GeneratePendingBubble();
            }
            if (_loadedBubble.Bubble == null)
            {
                _loadedBubble.Bubble = _pendingBubble.Bubble;
                _pendingBubble.Bubble = null;
                _loadedBubble.Bubble.transform.DOLocalMove(Vector3.zero, 0.5f);
                _loadedBubble.Bubble.transform.DOScale(Vector3.one, 0.5f);
                StartCoroutine(WaitUnilReloadDone());
            }
            else return;
        }

        IEnumerator WaitUnilReloadDone()
        {
            yield return new WaitForSeconds(0.6f);
            if (_pendingBubble.Bubble == null)
            {
                GeneratePendingBubble();
            }
            EnableTurret();
        }

        private void GeneratePendingBubble()
        {
            _pendingBubble.Bubble = ObjectPooling.Instance.SpawnObject(_bubbleGameController.BubblePrefab.gameObject, _pendingBubble.BubbleHolder, Vector3.zero, Quaternion.identity).GetComponent<BubbleControllerRefactor>();
            _pendingBubble.Bubble.Init(_bubbleGameController.GetBubbleBaseOnProbability(), null, _bubbleGameController);
            _pendingBubble.Bubble.ChangeToState(BubbleState.Pending);
            _pendingBubble.Bubble.transform.localScale = Vector3.one * 0.5f;
            _pendingBubble.Bubble.SpriteRenderer.sortingLayerName = "UIFront";
            _pendingBubble.Bubble.SpriteRenderer.sortingOrder = 10;
            _pendingBubble.Bubble.transform.localPosition = Vector3.zero;

        }
        public void Swap()
        {
            if (_loadedBubble.Bubble == null || _pendingBubble.Bubble == null) return;
            if (!_isSwapable) return;
            DisableTurret();
            (_pendingBubble.Bubble, _loadedBubble.Bubble) = (_loadedBubble.Bubble, _pendingBubble.Bubble);
            _loadedBubble.Bubble.transform.DOLocalMove(Vector3.zero, 0.5f);
            _loadedBubble.Bubble.transform.DOScale(Vector3.one, 0.5f);
            _pendingBubble.Bubble.transform.DOLocalMove(Vector3.zero, 0.5f);
            _pendingBubble.Bubble.transform.DOScale(Vector3.one * 0.5f, 0.5f);
            RCGSoundSetting.Instance.PlaySFX(_bubbleGameController.SwapAudioClips);
            StartCoroutine(WaitUntilSwapIsDoneCoroutine());
        }

        IEnumerator WaitUntilSwapIsDoneCoroutine()
        {
            _isSwapping = true;
            yield return new WaitForSeconds(0.6f);
            _isSwapping = false;
            EnableTurret();
        }
        public void RotateBarrelFollowMouse()
        {
            if (Input.GetMouseButton(0))
            {
                if (_isRotateable)
                {
                    Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    float angle = AngleBetweenTwoPoints(_barrel.position, mouseOnScreen);
                    if (angle > _maxAngle)
                    {
                        _barrel.transform.rotation = Quaternion.Euler(new(0, 0, _maxAngle));
                        return;
                    }
                    else if (angle < _minAngle)
                    {
                        _barrel.transform.rotation = Quaternion.Euler(new(0, 0, _minAngle));
                        return;
                    }
                    else
                    {
                        _barrel.rotation = Quaternion.Euler(new Vector3(0f, 0f, 360f));
                    }
                    _barrel.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
                }
                if (_isShootable) _cancelShootArea.gameObject.SetActive(true);
            }
            else
            {
                _cancelShootArea.gameObject.SetActive(false);
            }
        }
        private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            var angle = Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg + 90;
            angle = NormalizeAngle(angle);
            return angle;
        }
        private float NormalizeAngle(float angle)
        {
            angle %= 360f; // Keep within 0–360
            if (angle > 180f)
                angle -= 360f; // Convert to -180–180
            return angle;
        }
        public void EnableTurret()
        {
            _isShootable = true;
            _isSwapable = true;
            _swapButton.interactable = true;
        }
        public void DisableTurret()
        {
            _isShootable = false;
            _isSwapable = false;
            _swapButton.interactable = false;
        }

        public void EnableRotate()
        {
            _isRotateable = true;
        }

        public void DisableRotate()
        {
            _isRotateable = false;
        }

        public void ResetRotate()
        {
            _barrel.rotation = Quaternion.Euler(new Vector3(0f, 0f, 360f));
        }
    }

    [Serializable]
    public class BubbleLoader
    {
        [SerializeField] Transform _bubbleHolder;
        [SerializeField] BubbleControllerRefactor _bubble;

        public Transform BubbleHolder { get => _bubbleHolder; set => _bubbleHolder = value; }
        public BubbleControllerRefactor Bubble
        {
            get
            {
                return _bubble;
            }
            set
            {
                _bubble = value;
                if (_bubble != null) _bubble.transform.parent = _bubbleHolder;
            }
        }
    }
}