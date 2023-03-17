using LocalizationSystem;
using UnityEngine;
using UnityEngine.UI;

public class UpdateImageKeyExample : MonoBehaviour
{
    [SerializeField]
    private InputField key;

    [SerializeField]
    LocaleSprite[] localeSpritesToBeUpdate = new LocaleSprite[0];

    public void UpdateImageKey()
    {
        if (key == null)
        {
            Debug.LogError("請先綁定key InputField");
            return;
        }

        foreach (LocaleSprite localeSprite in localeSpritesToBeUpdate)
        {
            localeSprite.UpdateKey(key.text);
        }
    }
}
