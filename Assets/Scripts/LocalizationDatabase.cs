using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Localization
{
    [CreateAssetMenu]
    public class LocalizationDatabase : ScriptableObject
    {
        [SerializeField] private List<LocalizationKey> _localizationKeys;

        public IReadOnlyList<LocalizationKey> LocalizationKeys => _localizationKeys;
    }

    [Serializable]
    public class LocalizationKey
    {
        [SerializeField] private string _key;
        [SerializeField] private List<LocalizationValue> _values;

        public string Key => _key;
        public IReadOnlyList<LocalizationValue> Values => _values;
    }

    [Serializable]
    public class LocalizationValue
    {
        [SerializeField] private SystemLanguage _language;
        [SerializeField] private string _value;

        public SystemLanguage Language => _language;
        public string Value => _value;
    }
}

