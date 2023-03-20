using LocalizationSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoUpdateTextKeyAndArgs : MonoBehaviour
{
    [SerializeField]
    private InputField key;
    [SerializeField]
    private InputField args;
    [SerializeField]
    LocaleText[] localeTextToBeUpdate = new LocaleText[0];
    [SerializeField]
    LocaleTextMeshProUGUI[] localeTMPTextToBeUpdate = new LocaleTextMeshProUGUI[0];

    public void UpdateTextKeyAndArgs()
    {
        if (key == null || args == null)
        {
            Debug.LogError("請先綁定key InputField、args InputField");
            return;
        }

        string[] argsStringArray = args.text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (LocaleText LocaleText in localeTextToBeUpdate)
        {
            LocaleText.UpdateKeyAndArgs(key.text, argsStringArray);
        }

        foreach (LocaleTextMeshProUGUI LocaleTMPText in localeTMPTextToBeUpdate)
        {
            LocaleTMPText.UpdateKeyAndArgs(key.text, argsStringArray);
        }
    }
}
