using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class LocaleImage : LocaleComponentBase
{
    static Dictionary<string, Sprite> spriteMap = new Dictionary<string, Sprite>();

    /// <summary>
    /// 
    /// </summary>
    public override void OnLocalize()
    {
        if (!string.IsNullOrEmpty(key))
        {
            var fullPath = GetFullPath(key);
            //var sprite = Resources.Load<Sprite>(fullPath);
            var sprite = GetSprite(fullPath);
            if (sprite != null)
            {
                GetComponent<Image>().sprite = sprite;
                //set size
                GetComponent<Image>().rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
            }
        }
    }

    string GetFullPath(string key)
    {
        return "Images/Localization/" + key + "_" + Localization.language;
    }

    Sprite GetSprite(string fullPath)
    {
        if (spriteMap.ContainsKey(fullPath) == false)
        {
            spriteMap[fullPath] = Resources.Load<Sprite>(fullPath);
        }
        return spriteMap[fullPath];
    }
}
