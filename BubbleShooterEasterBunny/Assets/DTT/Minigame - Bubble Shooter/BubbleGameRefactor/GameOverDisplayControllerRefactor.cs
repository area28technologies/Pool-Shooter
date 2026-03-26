using TMPro;
using UnityEngine;

namespace DTT.BubbleShooter
{
    public class GameOverDisplayControllerRefactor : MonoBehaviour
    {
        [Header("Child Component")]
        [SerializeField] TMP_Text _scoreText;

        public void Init(int score)
        {
            _scoreText.text = score.ToString();
        }
    }
}