using Assets.Scripts;
using RSG;
using System.Collections;
using UnityEngine;

public static class Utils
{
    private static MonoBehaviour _mb = null;

    public static void SetMainContainer(MonoBehaviour mainContainer)
    {
        _mb = mainContainer;
    }

    public static Promise WaitSeconds(float time)
    {
        var promise = new Promise();

        if (time == 0f)
            promise.ResolveOnce();
        else
        {
            if (_mb != null)
                _mb.StartCoroutine(timer());
            else
                Debug.LogError("_mb is null");
        }

        return promise;

        IEnumerator timer()
        {
            yield return new WaitForSeconds(time);
            promise.ResolveOnce();
        }
    }

    public static Promise WaitForFixedUpdate()
    {
        var promise = new Promise();


        if (_mb != null)
            _mb.StartCoroutine(timer());
        else
            Debug.LogError("_mb is null");

        return promise;

        IEnumerator timer()
        {
            yield return new WaitForFixedUpdate();
            promise.ResolveOnce();
        }
    }

    public static Promise WaitForEndOfFrame()
    {
        var promise = new Promise();

        if (_mb != null)
            _mb.StartCoroutine(timer());
        else
            Debug.LogError("_mb is null");

        return promise;

        IEnumerator timer()
        {
            yield return new WaitForEndOfFrame();
            promise.ResolveOnce();
        }
    }

    public static void ResolveOnce(this Promise p)
    {
        if (p.CurState == PromiseState.Pending)
            p.Resolve();
    }

    /// <summary>
    /// Helps to convert Unity's Application.systemLanguage to a
    /// 2 letter ISO country code. There is unfortunately not more
    /// countries available as Unity's enum does not enclose all
    /// countries.
    /// </summary>
    /// <returns>The 2-letter ISO code from system language.</returns>
    public static string To2LetterISOCode(this SystemLanguage lang)
    {
        string result = "EN";

        switch (lang)
        {
            case SystemLanguage.Afrikaans: result = "AF"; break;
            case SystemLanguage.Arabic: result = "AR"; break;
            case SystemLanguage.Basque: result = "EU"; break;
            case SystemLanguage.Belarusian: result = "BY"; break;
            case SystemLanguage.Bulgarian: result = "BG"; break;
            case SystemLanguage.Catalan: result = "CA"; break;
            case SystemLanguage.Chinese: result = "ZH"; break;
            case SystemLanguage.Czech: result = "CS"; break;
            case SystemLanguage.Danish: result = "DA"; break;
            case SystemLanguage.Dutch: result = "NL"; break;
            case SystemLanguage.English: result = "EN"; break;
            case SystemLanguage.Estonian: result = "ET"; break;
            case SystemLanguage.Faroese: result = "FO"; break;
            case SystemLanguage.Finnish: result = "FI"; break;
            case SystemLanguage.French: result = "FR"; break;
            case SystemLanguage.German: result = "DE"; break;
            case SystemLanguage.Greek: result = "EL"; break;
            case SystemLanguage.Hebrew: result = "IW"; break;
            case SystemLanguage.Hungarian: result = "HU"; break;
            case SystemLanguage.Icelandic: result = "IS"; break;
            case SystemLanguage.Indonesian: result = "IN"; break;
            case SystemLanguage.Italian: result = "IT"; break;
            case SystemLanguage.Japanese: result = "JA"; break;
            case SystemLanguage.Korean: result = "KO"; break;
            case SystemLanguage.Latvian: result = "LV"; break;
            case SystemLanguage.Lithuanian: result = "LT"; break;
            case SystemLanguage.Norwegian: result = "NO"; break;
            case SystemLanguage.Polish: result = "PL"; break;
            case SystemLanguage.Portuguese: result = "PT"; break;
            case SystemLanguage.Romanian: result = "RO"; break;
            case SystemLanguage.Russian: result = "RU"; break;
            case SystemLanguage.SerboCroatian: result = "SH"; break;
            case SystemLanguage.Slovak: result = "SK"; break;
            case SystemLanguage.Slovenian: result = "SL"; break;
            case SystemLanguage.Spanish: result = "ES"; break;
            case SystemLanguage.Swedish: result = "SV"; break;
            case SystemLanguage.Thai: result = "TH"; break;
            case SystemLanguage.Turkish: result = "TR"; break;
            case SystemLanguage.Ukrainian: result = "UK"; break;
            case SystemLanguage.Unknown: result = "EN"; break;
            case SystemLanguage.Vietnamese: result = "VI"; break;
        }
        //		Debug.Log ("Lang: " + res);
        return result;
    }

    public static string Localize(this string str, params string[] parameters)
    {
        return Game.Localize(str, parameters);
    }

    public static bool IsNullOrEmpty(this string str)
    {
        if (str == null || str.Length == 0)
            return true;

        return false;
    }
}
