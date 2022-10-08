using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Localization
{
    public class GameLocalization
    {
        public static string[] RuLangAvailable =
        {
            Locale.RU,
            Locale.BE,
            Locale.TG,
            Locale.UA,
            Locale.UK,
            Locale.KA,
            Locale.KK,
            Locale.KY,
            Locale.TK,
            Locale.UZ,
            Locale.TT,
            Locale.BA,
            Locale.HY,
            Locale.AZ
        };

        public static IReadOnlyCollection<string> AvailableLocals => _availableLocals;

        private static string[] _availableLocals =
{
            Locale.RU,
            Locale.EN
        };

        private readonly Regex _paramRegex = new Regex(@"@\d");

        public static string CurrentLocale { get; set; }
        public static bool IsLocaleRU => CurrentLocale == Locale.RU;

        public Dictionary<string, string> LocalizePairs { get; private set; } = new Dictionary<string, string>();

        public event Action LanguageChanged;

        public void LoadKeys(string locale, LocalizationDatabase database)
        {
            if (_availableLocals.Contains(locale) == false)
                locale = Locale.EN;

            Dictionary<string, string> pairs = new();

            foreach (var key in database.LocalizationKeys)
            {
                foreach (var value in key.Values)
                {
                    if (locale == Utils.To2LetterISOCode(value.Language))
                    {
                        if (pairs.TryAdd(key.Key, value.Value) == false)
                            Debug.LogError($"{key.Key} is already contains in localization keys!");

                        break;
                    }
                }
            }

            LocalizePairs = pairs;
            CurrentLocale = locale;

            Debug.Log("[LOCALIZTION]: language setted - " + locale);

            LanguageChanged?.Invoke();
        }

        public string Localize(string key, params string[] parameters)
        {
            if (key == null)
            {
                Debug.LogError("Localize key is null\n" + new Exception().StackTrace);
                return "@null";
            }

            string result = "@" + key;

            if (LocalizePairs.ContainsKey(key))
                result = LocalizePairs[key];

            if (parameters != null && parameters.Length > 0)
                for (int i = 0; i < parameters.Length; i++)
                    result = result.Replace("@" + (i + 1), parameters[i] ?? string.Empty);

            return _paramRegex
                .Replace(result, "")
                .Replace("<br>", Environment.NewLine)
                .Replace("\\n", Environment.NewLine);
        }

        public bool ContainsKey(string key) => LocalizePairs.ContainsKey(key);

        public static string GetSystemLocaleByCapabilities()
        {
            var locale = Utils.To2LetterISOCode(Application.systemLanguage).ToUpper();

            if (RuLangAvailable.Contains(locale))
                return Locale.RU;

            return Locale.EN;
        }
    }
}
