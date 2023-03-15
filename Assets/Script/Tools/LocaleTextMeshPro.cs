using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshPro))]
public class LocaleTextMeshPro : LocaleTextBase<TextMeshPro>
{
    protected override void UpdateText(string str)
    {
        text.text = str;
        text.SetAllDirty();
    }
}
