using UnityEngine;
using UnityEngine.UI;

namespace LocalizationSystem
{
    [RequireComponent(typeof(Image))]
    public class LocaleSprite : LocaleComponentBase
    {
        private Image image;

        private void Start()
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                Debug.LogError("找不到Image元件，請確保本元件使用[RequireComponent(typeof(Image))]");
                return;
            }

            Localize();
        }

        /// <summary>
        /// 使用事先在Inspector設定好的鍵值進行本地化
        /// </summary>
        public override void Localize()
        {
            UpdateKey(localizationKey);
        }

        /// <summary>
        /// 重設鍵值, 並刷新顯示
        /// </summary>
        /// <param name="newLocalizationKey">新鍵值</param>
        public void UpdateKey(string newLocalizationKey)
        {
            localizationKey = newLocalizationKey;

            if (image == null)
            {
                return;
            }

            image.sprite = Localization.GetInstance().GetLocaleSprite(localizationKey);

            if (image.sprite == null)
            {
                return;
            }
            //set size
            image.rectTransform.sizeDelta = new Vector2(image.sprite.rect.width, image.sprite.rect.height);
        }
    }
}