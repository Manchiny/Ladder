using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RSG;
using UnityEngine;

namespace Assets.Scripts.Localization
{
    public class GameLocalization
    {
        public static string[] AvailableLangs =
        {
            LOCALE.RU,
            LOCALE.EN
        };

        public static string[] RU_LANG_AVAILABLE =
        {
            LOCALE.RU,
            LOCALE.BE,
            LOCALE.TG,
            LOCALE.UA,
            LOCALE.UK,
            LOCALE.KA,
            LOCALE.KK,
            LOCALE.KY,
            LOCALE.TK,
            LOCALE.UZ,
            LOCALE.TT,
            LOCALE.BA,
            LOCALE.HY,
            LOCALE.AZ
        };

        private readonly Regex _paramRegex = new Regex(@"@\d");

        public GameLocalization()
        {
        }

        public static string Locale { get; set; } = LOCALE.EN;
        public static bool IsLocaleRU => Locale == LOCALE.RU;

        public Dictionary<string, string> LocalizePairs { get; private set; } = new Dictionary<string, string>();

        public void Load(string locale, LocalizationDatabase database)
        {
            Dictionary<string, string> pairs = new();

            foreach (var key in database.LocalizationKeys)
            {
                foreach (var value in key.Values)
                {
                    if (locale == Utils.To2LetterISOCode(value.Language))
                    {
                        pairs.Add(key.Key, value.Value);
                        break;
                    }
                }
            }

            LocalizePairs = pairs;
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

        public static string GetValidLocale()
        {
            if (AvailableLangs.Contains(Locale, StringComparer.OrdinalIgnoreCase))
                return Locale;

            return LOCALE.EN;
        }

        public bool ContainsKey(string key) => LocalizePairs.ContainsKey(key);

        public static string GetLocaleByCapabilities(string lang = null)
        {
            var locale = lang ?? Utils.To2LetterISOCode(Application.systemLanguage).ToUpper();

            if (RU_LANG_AVAILABLE.Contains(locale))
                return LOCALE.RU;

            return locale;
        }
    }
}
