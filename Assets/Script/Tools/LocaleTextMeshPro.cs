using TMPro;
using UnityEngine;

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
