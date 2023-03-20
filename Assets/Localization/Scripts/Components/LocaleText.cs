using UnityEngine;
using UnityEngine.UI;

namespace LocalizationSystem
{
    [RequireComponent(typeof(Text))]
    public class LocaleText : LocaleComponentBase
    {
        [SerializeField]
        [Tooltip("要代入字串中的參數，會取代掉以<i>表示的多國字串")]
        private string[] args = new string[0];

        private Text text;

        private void Start()
        {
            text = GetComponent<Text>();
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

            //　取得多國文字並刷新顯示
            text.text = Localization.GetInstance().GetLocaleText(localizationKey, args);
            text.SetAllDirty();
        }
    }
}