using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class LeaderboardPlayerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        public void Init(string name, int score)
        {
            _nameText.text = name;
            _scoreText.text = score.ToString();
        }
    }
}
