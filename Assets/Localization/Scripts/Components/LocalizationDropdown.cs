using LocalizationSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class LocalizationDropdown : MonoBehaviour
{
    private Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        if (dropdown == null)
        {
            Debug.LogError("找不到選單元件，請加上[RequireComponent(typeof(Dropdown))]來確保選單元件存在!");
            return;
        }

        // 清空，確保不會有非預期的值
        dropdown.options.Clear();

        // 取得支援語言清單及目前使用中語言
        List<SupportLanguage> supportLanguageList = Localization.GetInstance().GetSupportLanguageList();
        SystemLanguage currentLanguage = Localization.GetInstance().GetCurrentLanguage();

        // 用Language enum的Name當作key值去查找本地顯示內容
        for (int i = 0; i < supportLanguageList.Count; ++i)
        {
            // 添加到選單上顯示
            dropdown.options.Add(new Dropdown.OptionData(supportLanguageList[i].name));
            // 顯示值更新為目前使用中語言
            if (supportLanguageList[i].language == currentLanguage)
            {
                dropdown.SetValueWithoutNotify(i);
                dropdown.RefreshShownValue();
            }
        }

        // 綁定onValueChanged事件回呼
        dropdown.onValueChanged.AddListener(
            delegate
            {
                DropdownValueChanged(dropdown);
            }
        );
    }

    // onValueChanged事件回呼
    private void DropdownValueChanged(Dropdown dropdown)
    {
        // 更新多國語言設定
        SystemLanguage language = Localization.GetInstance().GetSupportLanguageList()[dropdown.value].language;
        Localization.GetInstance().SetCurrentLanguage(language, this);
    }

}
