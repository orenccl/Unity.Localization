using UnityEngine;

public abstract class LocaleComponentBase : MonoBehaviour
{
    [SerializeField]
    protected string key;

    /// <summary>
    /// Localize on start.
    /// </summary>
    void Start()
    {
        OnLocalize();
    }

    /// <summary>
    /// 當Localization變換語系時呼叫
    /// </summary>
    public abstract void OnLocalize();
}
