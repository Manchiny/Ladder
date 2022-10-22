using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Social.AbstractSocialAdapter;

namespace Assets.Scripts.UI
{
    public class LeaderboardWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private RectTransform _viewsContainer;
        [Space]
        [SerializeField] private LeaderboardPlayerView _playerViewPrefab;
        [SerializeField] private Image _loader;

        public override string LockKey => "LeaderboardWindow";

        public override bool AnimatedClose => true;

        public static LeaderboardWindow Show() =>
                        Game.Windows.ScreenChange<LeaderboardWindow>(false, w => w.Init());

        protected void Init()
        {
            SetText();

            Game.Social.GetLeaderboardData(DefaultLeaderBoardName)
                .Then(data => data.ForEach(user => CreatePlayerView(user)))
                .Then(_ => _loader.gameObject.SetActive(false));
        }

        protected override void SetText()
        {
            _titleText.text = "leaderboard".Localize();
        }

        private void CreatePlayerView(LeaderboardData user)
        {
            LeaderboardPlayerView view = Instantiate(_playerViewPrefab, _viewsContainer);
            view.Init(user.UserName, user.ScoreValue);
        }
    }
}
