using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LocaleTextMeshProUGUI : LocaleTextBase<TextMeshProUGUI>
{
    protected override void UpdateText(string str)
    {
        text.text = str;
        text.SetAllDirty();
    }
}