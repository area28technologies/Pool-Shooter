using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.BubbleShooter
{
    public class ScoreDisplayControllerRefactor : MonoBehaviour
    {
        [Header("Child Component")]
        [SerializeField] TextMeshProUGUI _scoreText;
        [SerializeField] TextMeshProUGUI _coinText;
        [SerializeField] TextMeshProUGUI _timeText;

        private int _currentScore = 0;
        private DateTime _startTime;

        private BubbleGameControllerRefactor _bubbleGameController;
        public void Init(BubbleGameControllerRefactor bubbleGameController)
        {
            StopAllCoroutines();
            _bubbleGameController = bubbleGameController;
            _currentScore = _bubbleGameController.Score;
            UpdateScore();
            _startTime = DateTime.Now;
            StartCoroutine(UpdateScoreCoroutine());
            StartCoroutine(UpdateTimeCoroutine());
        }
        public void UpdateScore()
        {
            _scoreText.text = _currentScore.ToString();
        }

        IEnumerator UpdateScoreCoroutine()
        {
            yield return new WaitForSeconds(0.05f);
            if (_currentScore < _bubbleGameController.Score)
            {
                _currentScore += 5;
                UpdateScore();
            }
            else if (_currentScore > _bubbleGameController.Score)
            {
                _currentScore -= 5;
                UpdateScore();
            }
            StartCoroutine(UpdateScoreCoroutine());
        }

        IEnumerator UpdateTimeCoroutine()
        {
            yield return new WaitForSeconds(1f);
            var deltaTime = DateTime.Now - _startTime;

            string niceTime = string.Format("{00:00}:{01:00}", deltaTime.Minutes, deltaTime.Seconds);
            _timeText.text = niceTime;
            StartCoroutine(UpdateTimeCoroutine());
        }
    }
}