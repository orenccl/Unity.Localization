using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LocalizationSystem
{
    public abstract class LocaleTMPTextBase<T> : LocaleComponentBase where T : TMP_Text
    {
        [Serializable]
        protected struct LanguageFontOverride
        {
            [SerializeField]
            public SystemLanguage language;
            [SerializeField]
            public TMP_FontAsset fontAsset;
        }

        [SerializeField]
        [Tooltip("要代入字串中的參數，會取代掉以<i>表示的多國字串")]
        protected string[] args = new string[0];
        [SerializeField]
        protected TMP_FontAsset defaultFontAsset;
        [SerializeField]
        protected List<LanguageFontOverride> FontOverrideList = new List<LanguageFontOverride>();

        protected T text;

        private void Start()
        {
            text = GetComponent<T>();
            if (text == null)
            {
                Debug.LogError("找不到文字元件，請在繼承的子元件上加上[RequireComponent(typeof(T))]來確保文字元件存在!");
                return;
            }

            Localize();
        }

        /// <summary>
        /// 使用事先在Inspector設定好的鍵值與參數進行本地化
        /// </summary>
        public override void Localize()
        {
            UpdateKeyAndArgs(localizationKey, args);
            UpdateFontAsset();
        }

        /// <summary>
        /// 重設鍵值, 並刷新顯示
        /// </summary>
        /// <param name="newLocalizationKey">新鍵值</param>
        public void UpdateKey(string newLocalizationKey)
        {
            UpdateKeyAndArgs(newLocalizationKey, args);
        }

        /// <summary>
        /// 重設參數, 並刷新顯示
        /// </summary>
        /// <param name="newArgs">新參數</param>
        public void UpdateArgs(string[] newArgs)
        {
            UpdateKeyAndArgs(localizationKey, newArgs);
        }

        /// <summary>
        /// 重設鍵值與參數, 並刷新顯示
        /// </summary>
        /// <param name="newLocalizationKey">新鍵值</param>
        /// <param name="newArgs">新參數</param>
        public void UpdateKeyAndArgs(string newLocalizationKey, string[] newArgs)
        {
            localizationKey = newLocalizationKey;
            args = newArgs;

            if (text == null)
            {
                return;
            }

            text.text = Localization.GetInstance().GetLocaleText(localizationKey, args);
            text.SetAllDirty();
        }

        /// <summary>
        /// 依照語系更新字體
        /// </summary>
        private void UpdateFontAsset()
        {
            SystemLanguage currentLanguage = Localization.GetInstance().GetCurrentLanguage();
            int index = FontOverrideList.FindIndex(value => value.language == currentLanguage);
            text.font = index == -1 ? defaultFontAsset : FontOverrideList[index].fontAsset;
        }
    }
}