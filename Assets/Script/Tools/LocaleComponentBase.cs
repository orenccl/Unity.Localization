using UnityEngine;

public abstract class LocaleComponentBase : MonoBehaviour
{
    [SerializeField]
    [Tooltip("與多國資料表對應的鍵值")]
    protected string localizationKey;

    /// <summary>
    /// 初始本地化
    /// </summary>
    void Start()
    {
        Localize();
    }

    /// <summary>
    /// 使用事先在Inspector設定好的鍵值進行本地化
    /// </summary>
    public abstract void Localize();
}
