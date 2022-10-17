using Assets.Scripts.Social;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameLoader : MonoBehaviour
    {
        [SerializeField] private AbstractSocialAdapter _yandexSocialAdapter;

        private AbstractSocialAdapter _socialAdapter;

        private IEnumerator Start()
        {

#if UNITY_WEBGL || UNITY_EDITOR
#if YANDEX_GAMES
            _socialAdapter = _yandexSocialAdapter;
#endif
#endif
            if (_socialAdapter != null)
            {
                _socialAdapter.gameObject.SetActive(true);
                yield return _socialAdapter.Init();
            }

            SceneManager.LoadScene(1);
        }
    }
}
