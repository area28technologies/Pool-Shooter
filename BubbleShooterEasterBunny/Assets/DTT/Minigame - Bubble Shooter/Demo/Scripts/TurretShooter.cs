using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class handles reloading bubbles and shooting bubbles from the turret.
    /// </summary>
    public class TurretShooter : MonoBehaviour
    {
        /// <summary>
        /// The _manager field is a <see cref="BubbleShooterManager"/> reference to access the turret's current information.
        /// </summary>
        [Header("Object references")]
        [SerializeField]
        private BubbleShooterManager _manager;

        /// <summary>
        /// The _renderer field is a <see cref="DemoGridRenderer"/> reference used to properly render the bubble shot
        /// from the turret.
        /// </summary>
        [SerializeField]
        private DemoGridRenderer _renderer;

        /// <summary>
        /// The _turretBarrelColorSprite is a reference to the <see cref="SpriteRenderer"/> component of the middle
        /// part of the turret.
        /// </summary>
        [SerializeField]
        private SpriteRenderer _turretBarrelColorSprite;

        ///// <summary>
        ///// The _turretBarrelOpeningSprite field is a reference to the <see cref="SpriteRenderer"/> component of
        ///// the turret's opening.
        ///// </summary>
        //[SerializeField]
        //private SpriteRenderer _turretBarrelOpeningSprite;

        ///// <summary>
        ///// The _turretText field is a reference to the <see cref="Text"/> component that displays a potential number
        ///// of the currently loaded bubble.
        ///// </summary>
        //[SerializeField]
        //private Text _turretText;

        /// <summary>
        /// The _inputManager field refers to the manager that handles incoming input from a specific platform.
        /// </summary>
        [SerializeField]
        private InputManager _inputManager;

        /// <summary>
        /// The _spawnPoint field is a transform's position from which the bubble will be instantiated from when shooting.
        /// </summary>
        [Header("Bubble spawning settings")]
        [SerializeField]
        private Transform _spawnPoint;

        /// <summary>
        /// The _controllerTemplate field is a prefab reference used when instantiating a bubble upon firing the turret.
        /// </summary>
        [SerializeField]
        private BubbleController _controllerTemplate;

        /// <summary>
        /// Duration of the animation.
        /// </summary>
        [SerializeField]
        private float animationDuration = 0.5f;

        /// <summary>
        /// Amplitude of the animation.
        /// </summary>
        [SerializeField]
        private float animationAmplitude = 1f;

        /// <summary>
        /// Transform of the canon.
        /// </summary>
        public Transform _canonTransform;

        [SerializeField]
        private Button swapButton;

        [SerializeField]
        private SpriteRenderer swapColor;

        [SerializeField]
        private GameObject cancelShotObject;

        [SerializeField]
        private List<AudioClip> swapAnimAudioClip;

        /// <summary>
        /// The Awake method initializes event listeners for once the turret reloads and fires.
        /// </summary>
        private void Awake()
        {
            _manager.Initialized += () => _manager.Turret.Reloaded += HandleReload;
            _manager.Started += () => _manager.Turret.Shot += HandleShot;
            _canonTransform = transform.GetChild(0);

            _manager.Initialized += () => _manager.Turret.Swaped += HandleReload;

            swapButton.onClick.AddListener(() => { _manager.HandleSwap(); });
        }

        /// <summary>
        /// The OnEnable method attaches a listener to the input manager for shooting the turret.
        /// </summary>
        private void OnEnable() => _inputManager.ControllerInitialized += InitializeInput;

        /// <summary>
        /// The OnDisable method detaches a listener to the input manager for shooting the turret.
        /// </summary>
        private void OnDisable() => _inputManager.ControllerInitialized -= InitializeInput;

        /// <summary>
        /// The InitializeInput initializes input from the <see cref="InputManager"/> for clicking/tapping on the screen.
        /// </summary>
        private void InitializeInput()
        {
            _inputManager.Controller.Perform += (eventArgs) =>
            {
                if (cancelShotObject.activeInHierarchy && CancelShootArea.isCanShoot)
                {
                    cancelShotObject.SetActive(false);
                }

                _manager.ShootTurret(_spawnPoint.up);
            };
        }
        //private void InitializeInput() => _inputManager.Controller.Perform += _ => _manager.ShootTurret(_spawnPoint.up);

        /// <summary>
        /// Is the canon being animated.
        /// </summary>
        private bool _isAnimated = false;

        /// <summary>
        /// The HandleReload method reloads the given bubble in the turret and redraws its visuals.
        /// </summary>
        /// <param name="bubble">The <see cref="Bubble"/> instance the turret gets reloaded with.</param>
        private void HandleReload(List<Bubble> bubbleList, bool isSwap = false)
        {
            _manager.Turret.SetSwaping(true);

            if (!isSwap)
            {
                _turretBarrelColorSprite.sprite = null;
            }

            // Start animation from swap bubble to turret
            StartCoroutine(AnimateSwapToMain(swapColor.transform, _turretBarrelColorSprite.transform, 0.5f, bubbleList, isSwap));
        }

        private IEnumerator AnimateSwapToMain(Transform swapTransform, Transform mainTransform, float duration, List<Bubble> bubbleList, bool isSwap)
        {
            if (!RCGSoundSetting.Instance.IsSoundMuted)
            {
                RCGSoundSetting.Instance.PlaySFX(swapAnimAudioClip);
            }

            Vector3 startPosition = swapTransform.position;
            Vector3 endPosition = mainTransform.position;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Smooth the movement
                swapTransform.position = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0f, 1f, t));

                if (isSwap)
                {
                    mainTransform.position = Vector3.Lerp(endPosition, startPosition, Mathf.SmoothStep(0f, 1f, t));
                }

                yield return null;
            }

            swapTransform.position = startPosition;

            if (isSwap)
            {
                mainTransform.position = endPosition;
            }

            Color bubbleColor = Color.white;
            Color bubbleColorSwap = Color.white;

            {
                if (bubbleList.Count > 0)
                {
                    Bubble currentBubble = bubbleList[0];

                    if (currentBubble is ColoredBubble coloredBubble)
                        bubbleColor = coloredBubble.Color;

                    _turretBarrelColorSprite.color = bubbleColor;
                    //_turretBarrelOpeningSprite.color = bubbleColor;

                    string turretText = string.Empty;
                    if (currentBubble is NumberedBubble numberedBubble)
                        turretText = numberedBubble.Number.ToString();

                    //_turretText.text = turretText;
                }

                if (bubbleList.Count > 1)
                {
                    Bubble swapBubble = bubbleList[1];

                    if (swapBubble is ColoredBubble coloredBubblee)
                        bubbleColorSwap = coloredBubblee.Color;

                    swapColor.color = bubbleColorSwap;
                }
            }

            List<Sprite> sprites = _renderer.BubbleSprites;
            ColorConfig[] colorConfigs = _manager.Config.ColorConfiguration;

            for (int i = 0; i < colorConfigs.Length; i++)
            {
                Color color = colorConfigs[i].BubbleColors;

                //Thay bi bubble color
                if (bubbleColor.Equals(color))
                {
                    if (sprites.Count > 0 && i < sprites.Count)
                    {
                        _turretBarrelColorSprite.sprite = sprites[i];
                        _turretBarrelColorSprite.color = Color.white;
                    }
                }

                //Thay bi bubble color swap
                if (bubbleColorSwap.Equals(color))
                {
                    if (sprites.Count > 0 && i < sprites.Count)
                    {
                        swapColor.sprite = sprites[i];
                        swapColor.color = Color.white;
                    }
                }
            }

            _manager.Turret.SetSwaping(false);
        }


        /// <summary>
        /// The HandleShot method instantiates the loaded bubble into a <see cref="BubbleController"/> and fires it into
        /// the given direction.
        /// </summary>
        /// <param name="bubble">The <see cref="Bubble"/> instance to shoot.</param>
        /// <param name="direction">The direction to shoot the bubble into.</param>
        private void HandleShot(Bubble bubble, Vector2 direction)
        {
            animationDuration = 0.06f;
            StartCoroutine(ShootAnimation());
            BubbleController currentController = Instantiate(_controllerTemplate, _spawnPoint.position, Quaternion.identity);
            currentController.Initialize(_manager, _renderer, bubble);
            _renderer.Render(bubble, currentController, new Vector3(0, 0, 0));
            currentController.Movement.Initialize(direction);

            _turretBarrelColorSprite.color = Color.white;
            //_turretBarrelOpeningSprite.color = Color.white;
            //_turretText.text = string.Empty;
        }

        /// <summary>
        /// Make an animation on the cannon when we shoot a bubble.
        /// </summary>
        /// <returns>IEnumerator</returns>
        IEnumerator ShootAnimation()
        {
            if (_isAnimated)
                yield break;
            _isAnimated = true;
            float animationTime = animationDuration;
            while (true)
            {
                animationTime -= Time.deltaTime;
                if (animationTime <= 0)
                {
                    _canonTransform.localScale += new Vector3(0.001f * animationAmplitude, 0.002f * animationAmplitude, 0);
                    if (_canonTransform.localScale.x >= 1)
                    {
                        _isAnimated = false;
                        _canonTransform.localScale = new Vector3(1, 1, 1);
                        break;
                    }
                }
                else
                {
                    _canonTransform.localScale -= new Vector3(0.001f * animationAmplitude, 0.002f * animationAmplitude, 0);
                }

                yield return new WaitForSeconds(0.01f);
            }

        }
    }
}