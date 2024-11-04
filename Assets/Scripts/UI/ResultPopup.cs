using TMPro;
using UnityEngine;

namespace BubbleShooter
{
    public class ResultPopup : Popup
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _bestScoreText;

        public void Setup(string scoreText, string bestScoreText)
        {
            _scoreText.text = scoreText;
            _bestScoreText.text = bestScoreText;
        }
    }
}