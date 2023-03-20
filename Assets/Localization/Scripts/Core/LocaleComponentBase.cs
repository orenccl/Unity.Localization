using UnityEngine;

namespace LocalizationSystem
{
    [ExecuteInEditMode]
    public abstract class LocaleComponentBase : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("與多國資料表對應的鍵")]
        protected string localizationKey;

        /// <summary>
        /// 初始本地化
        /// </summary>
        void Start()
        {
            Localize();
        }

        /// <summary>
        /// 使用事先在Inspector設定好的鍵進行本地化
        /// </summary>
        public abstract void Localize();

        /// <summary>
        /// 重設鍵, 並刷新顯示
        /// </summary>
        /// <param name="newLocalizationKey">新鍵</param>
        public abstract void UpdateKey(string newLocalizationKey);
    }
}