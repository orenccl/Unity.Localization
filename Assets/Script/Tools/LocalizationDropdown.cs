using System;
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

        // 用Language enum的Name當作key值去查找本地顯示內容
        foreach (string languageName in Enum.GetNames(typeof(Language)))
        {
            // 添加到選單上顯示
            string str = Localization.GetInstance().GetLocaleText(languageName);
            dropdown.options.Add(new Dropdown.OptionData(str));
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
        // 更新本地語系
        Localization.GetInstance().SetCurrentLanguage((Language)dropdown.value);
    }

}
