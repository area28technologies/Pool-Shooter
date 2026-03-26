using DTT.MinigameBase.LevelSelect;
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class handles behaviour for the popup when pausing the game or finishing a game.
    /// </summary>
    public class Popup : MonoBehaviour
    {
        /// <summary>
        /// The _manager field is a reference to the <see cref="BubbleShooterManager"/> instance used to listen to certain
        /// game flow events.
        /// </summary>
        [SerializeField]
        private BubbleShooterManager _manager;

        /// <summary>
        /// The _levelSelectHandler field is used to navigate back to the level selection from this popup.
        /// </summary>
        [SerializeField]
        private BubbleShooterLevelSelectHandler _levelSelectHandler;

        ///// <summary>
        ///// The _titleText field is a <see cref="Text"/> component reference used to set the title text.
        ///// </summary>
        //[SerializeField]
        //private Text _titleText;

        ///// <summary>
        ///// The _contentText field is a <see cref="Text"/> component reference used to set the content text of the popup.
        ///// </summary>
        //[SerializeField]
        //private Text _contentText;

        ///// <summary>
        ///// The _playButton field is a <see cref="Button"/> component reference used to continue the game when clicked.
        ///// </summary>
        //[SerializeField]
        //private Button _playButton;

        ///// <summary>
        ///// The _restartButton field is a <see cref="Button"/> component reference used to restart the game when clicked.
        ///// </summary>
        //[SerializeField]
        //private Button _restartButton;

        [SerializeField]
        private TextMeshProUGUI _textScore;
        public TextMeshProUGUI textMessage;
        [SerializeField]
        private GameObject _gameOverPanel;

        //[SerializeField]
        //private GameObject _settingPanel;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        /// <summary>
        /// The _exitButton field is a <see cref="Button"/> component reference used to navigate to the level selection when
        /// clicked.
        /// </summary>
        //[SerializeField]
        // private Button _exitButton;

        [SerializeField]
        private Button _backButton;

        /// <summary>
        /// The _overlay field is a GameObject reference that overlays the game with a slightly transparent black background.
        /// This will be visible when the popup is shown.
        /// </summary>
        //[SerializeField]
        //private GameObject _overlay;

        private void Awake()
        {
            _canvasGroup.alpha = 0; // Đảm bảo ban đầu popup bị ẩn
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// The OnEnable method attaches event listeners on the buttons of the popup.
        /// </summary>
        private void OnEnable()
        {
            //if(_manager.IsGameActive)
            //    _playButton.onClick.AddListener(Resume);
            //else
            //    _playButton.onClick.AddListener(Restart);
            //_restartButton.onClick.AddListener(Restart);
            //_exitButton.onClick.AddListener(Restart);
            _backButton.onClick.AddListener(() => { SceneManager.LoadScene("Main"); });
        }

        /// <summary>
        /// The OnDisable method detaches event listeners on the buttons of the popup.
        /// </summary>
        private void OnDisable()
        {
            //if (_manager.IsGameActive)
            //    _playButton.onClick.RemoveListener(Resume);
            //else
            //    _playButton.onClick.RemoveListener(Restart);
            //_restartButton.onClick.RemoveListener(Restart);
            //_exitButton.onClick.RemoveListener(Restart);
            _backButton.onClick.RemoveListener(Restart);
        }

        /// <summary>
        /// The Show method shows the popup with a given title and optional content text.
        /// </summary>
        /// <param name="title">The title for the popup to display.</param>
        /// <param name="content">The content within the popup to display.</param>
        public void Show(string title, string content = "")
        {
            //_titleText.text = title;
            //_contentText.text = content;

            gameObject.SetActive(true);
            //_overlay.SetActive(true);
            // Fade in popup
            StartCoroutine(FadeCanvasGroup(_canvasGroup, 0f, 1f, 0.5f)); // Hiệu ứng fade in
        }

        /// <summary>
        /// The ShowPaused method shows the popup with a corresponding paused title.
        /// </summary>
        public void ShowPaused()
        {
            //_playButton.gameObject.SetActive(true);
            Show("Paused");
        }
        
        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float duration, System.Action onComplete = null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = end;

            canvasGroup.interactable = (end == 1);
            canvasGroup.blocksRaycasts = (end == 1);

            onComplete?.Invoke();
        }

        /// <summary>
        /// The ShowGameResults method shows the popup with the game results in the content text of the popup.
        /// </summary>
        /// <param name="results">The <see cref="BubbleShooterResult"/> results of the game.</param>
        public void ShowGameResults(BubbleShooterResult results)
        {
			throw new NotImplementedException();
            TimeSpan time = TimeSpan.FromSeconds(results.timeTaken);
            string formattedTime = string.Format("{0:00}:{1:00}", Mathf.Floor((float)time.TotalMinutes), time.Seconds);

            StringBuilder builder = new StringBuilder();
            builder.Append($"Score: {results.score}\n");
            builder.Append("\n");
            builder.Append($"Time: {formattedTime}\n");

            //_playButton.gameObject.SetActive(false);
            string title = $"You {(results.hasWon ? "won" : "lost")}!";
            _textScore.text = results.score.ToString();
            //APIController.Instance.ImplementPostEndRound(results.score);
            //textMessage.text = APIController.Instance.messageSever;
            Show(title, builder.ToString());
        }

        /// <summary>
        /// The Hide method hides the popup.
        /// </summary>
        public void Hide()
        {
            StartCoroutine(FadeCanvasGroup(_canvasGroup, 1f, 0f, 0.5f, () =>
            {
                gameObject.SetActive(false);
                //_overlay.SetActive(false);
            }));
        }

        /// <summary>
        /// The Resume method hides the popup and resumes the game.
        /// </summary>
        public void Resume()
        {
            Hide();
            //_playButton.gameObject.SetActive(false);
            _manager.Continue();
        }

        /// <summary>
        /// The Restart method hides the popup and restarts the game.
        /// </summary>
        public void Restart()
        {
            Hide();
            _manager.Restart();
        }

        /// <summary>
        /// The NavigateHome method hides the popup and fades in the level selection.
        /// </summary>
        public void NavigateHome()
        {
            Hide();
            _manager.Stop();
            _levelSelectHandler.ShowLevelSelect();
        }
    }
}