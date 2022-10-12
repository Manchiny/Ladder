using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Assets.Scripts.Social.AbstractSocialAdapter;

namespace Assets.Scripts.UI
{
    public class LeaderboardWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private RectTransform _viewsContainer;
        [Space]
        [SerializeField] private LeaderboardPlayerView _playerViewPrefab;

        public override string LockKey => "LeaderboardWindow";

        public override bool AnimatedClose => true;

        public static LeaderboardWindow Show(List<LeaderboardData> data) =>
                        Game.Windows.ScreenChange<LeaderboardWindow>(false, w => w.Init(data));

        protected void Init(List<LeaderboardData> data)
        {
            SetText();
            data.ForEach(user => CreatePlayerView(user));
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
