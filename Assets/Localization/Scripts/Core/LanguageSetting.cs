using System;
using UnityEngine;

namespace LocalizationSystem
{
    [Serializable]
    public struct SupportLanguage
    {
        [SerializeField]
        [Tooltip("該系統語言原文名稱")]
        public string name;
        [SerializeField]
        public SystemLanguage language;
    }

    [CreateAssetMenu(fileName = "LanguageSetting", menuName = "ScriptableObject/LanguageSetting")]
    public class LanguageSetting : ScriptableObject
    {
        [Tooltip("相對於Resources資料夾底下")]
        public string localeTextUrl = "Localization/Text/";
        [Tooltip("相對於Resources資料夾底下")]
        public string localeImageUrl = "Localization/Image/";
        [Tooltip("有添加的語言才會顯示在多國語言選單中")]
        public SupportLanguage[] supportLanguageList;
    }
}