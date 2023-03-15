using UnityEngine;

[ExecuteInEditMode]
public abstract class LocaleTextBase<T> : LocaleComponentBase where T : MonoBehaviour
{
    public string[] args = new string[0];

    protected T text;

    private void OnEnable()
    {
        text = GetComponent<T>();
        if (text == null)
        {
            Debug.LogError("Can't get Text component!");
            return;
        }
    }

    public override void OnLocalize()
    {
        if (text == null)
        {
            return;
        }

        UpdateText(Localization.GetText(key, args));
    }

    protected abstract void UpdateText(string str);
}
