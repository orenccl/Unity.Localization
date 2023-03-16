using UnityEngine;

[ExecuteInEditMode]
public abstract class LocaleTextBase<T> : LocaleComponentBase where T : MonoBehaviour
{
    [SerializeField]
    [Tooltip("要代入字串中的參數，會取代掉以<i>表示的多國字串")]
    protected string[] args = new string[0];

    protected T text;

    protected abstract void UpdateText(string str);

    private void Start()
    {
        text = GetComponent<T>();
        if (text == null)
        {
            Debug.LogError("找不到文字元件，請在繼承的子元件上加上[RequireComponent(typeof(T))]來確保文字元件存在!");
            return;
        }

        Localize();
    }

    /// <summary>
    /// 使用事先在Inspector設定好的鍵值與參數進行本地化
    /// </summary>
    public override void Localize()
    {
        if (text == null)
        {
            return;
        }
        // 依照繼承類別的不同，自行去更新文字
        UpdateText(Localization.GetInstance().GetLocaleText(localizationKey, args));
    }

    /// <summary>
    /// 重設鍵值與參數, 並刷新顯示
    /// </summary>
    /// <param name="newLocalizationKey">新鍵值</param>
    /// <param name="newArgs">新參數</param>
    public void Relocalize(string newLocalizationKey, string[] newArgs)
    {
        localizationKey = newLocalizationKey;
        args = newArgs;
        Localize();
    }
}
