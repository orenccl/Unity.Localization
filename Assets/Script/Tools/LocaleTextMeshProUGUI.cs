using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LocaleTextMeshProUGUI : LocaleTextComponentBase<TextMeshProUGUI>
{
    [SerializeField]
    private SerializableDictionary<Language, TMP_FontAsset> overriteLocaleFont;
    [SerializeField]
    private TMP_FontAsset defaultFontAsset;
    private Dictionary<Language, TMP_FontAsset> overriteLocaleFontDictionary = null;

    protected override void UpdateText(string str)
    {
        text.text = str;
        text.SetAllDirty();

        if (overriteLocaleFontDictionary == null)
        {
            overriteLocaleFontDictionary = overriteLocaleFont.ToDictionary();
        }

        Language currentLanguage = Localization.GetInstance().GetCurrentLanguage();
        if (overriteLocaleFontDictionary.ContainsKey(currentLanguage) == false)
        {
            if (defaultFontAsset)
            {
                text.font = defaultFontAsset;
            }
            return;
        }

        TMP_FontAsset overriteFontAsset = overriteLocaleFontDictionary[currentLanguage];
        text.font = overriteFontAsset;
    }
}