using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public abstract class LocaleTMPTextBase<T> : LocaleComponentBase where T : TMP_Text
{

    [SerializeField]
    [Tooltip("要代入字串中的參數，會取代掉以<i>表示的多國字串")]
    protected string[] args = new string[0];
    [SerializeField]
    protected TMP_FontAsset defaultFontAsset;
    [SerializeField]
    protected SerializableDictionary<Language, TMP_FontAsset> overrideFontAsset;

    private Dictionary<Language, TMP_FontAsset> overriteFontAssetDictionary;

    protected T text;

    private void Start()
    {
        text = GetComponent<T>();
        if (text == null)
        {
            Debug.LogError("找不到文字元件，請在繼承的子元件上加上[RequireComponent(typeof(T))]來確保文字元件存在!");
            return;
        }

        overriteFontAssetDictionary = overrideFontAsset.ToDictionary();
        Localize();
    }

    /// <summary>
    /// 使用事先在Inspector設定好的鍵值與參數進行本地化
    /// </summary>
    public override void Localize()
    {
        if (text == null)
        {
            return;
        }
        // 依照繼承類別的不同，自行去更新文字
        text.text = Localization.GetInstance().GetLocaleText(localizationKey, args);
        text.SetAllDirty();

        // TODO: 拆成function
        Language currentLanguage = Localization.GetInstance().GetCurrentLanguage();
        if (overriteFontAssetDictionary.ContainsKey(currentLanguage) == false)
        {
            if (defaultFontAsset)
            {
                text.font = defaultFontAsset;
            }
            return;
        }

        TMP_FontAsset overriteFontAsset = overriteFontAssetDictionary[currentLanguage];
        text.font = overriteFontAsset;
    }

    /// <summary>
    /// 重設鍵值與參數, 並刷新顯示
    /// </summary>
    /// <param name="newLocalizationKey">新鍵值</param>
    /// <param name="newArgs">新參數</param>
    public void Relocalize(string newLocalizationKey, string[] newArgs)
    {
        localizationKey = newLocalizationKey;
        args = newArgs;
        Localize();
    }
}
