using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class LocaleText : LocaleTextComponentBase<Text>
{
    protected override void UpdateText(string str)
    {
        text.text = str;
        text.SetAllDirty();
    }
}
