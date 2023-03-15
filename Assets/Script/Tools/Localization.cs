using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Language
{
    EN,
    TW,
    CN,
}

static public class Localization
{
    static Dictionary<Language, Dictionary<string, string>> dictionarys = new Dictionary<Language, Dictionary<string, string>>();

    static private Dictionary<string, string> GetWorkingDictionary()
    {
        return dictionarys[_language];
    }

    /// <summary>
    /// 將字串轉換為Language enum格式
    /// </summary>
    /// <param name="str">要轉換的字串，需跟enum同名</param>
    /// <returns>Language enum或拋出系統錯誤</returns>
    static private Language StringToLanguageEnum(string str)
    {
        // 當enum解析失敗會拋出系統錯誤，表示靜態表語言參數錯誤
        return (Language)System.Enum.Parse(typeof(Language), str);
    }

    /// <summary>
    /// 載入語系表
    /// </summary>
    /// <param name="fileName"></param>
    static public void LoadDictionary(string fileName = "Data/Localization")
    {
        dictionarys.Clear();
        TextAsset[] asset = Resources.LoadAll<TextAsset>(fileName);

        for (int i = 0; i < asset.Length; i++)
        {
            LoadDictionary(asset[i]);
        }

        TextAsset[] test = Resources.LoadAll<TextAsset>(fileName);
        for (int i = 0; i < test.Length; i++)
        {
            Debug.LogWarning(test[i]);
        }

        language = Language.CN;
    }

    /// <summary>
    /// 載入語系表
    /// </summary>
    /// <param name="fileName"></param>
    static public void LoadDictionary(TextAsset asset)
    {
        CSVReader csv = new CSVReader();
        csv.LoadCSV(asset);

        for (int i = 1; i < csv.columns; i++)
        {

            if (!dictionarys.ContainsKey(StringToLanguageEnum(csv[i, 0])))
            {
                dictionarys[StringToLanguageEnum(csv[i, 0])] = new Dictionary<string, string>();
            }

            for (int j = 1; j < csv.rows; j++)
            {
                dictionarys[StringToLanguageEnum(csv[i, 0])][csv[0, j]] = csv[i, j];
            }
        }
    }

    static Language _language = Language.CN;
    /// <summary>
    /// 讀取設定目前使用語系
    /// </summary>
    static public Language language
    {
        set
        {
            if (dictionarys.ContainsKey(value))
            {
                _language = value;
            }
            else
            {
                Debug.Log("使用不存在的語言！");
            }

            // Notify to update all locale component
            foreach (LocaleComponentBase localeComponent in Object.FindObjectsOfType<LocaleComponentBase>())
            {
                localeComponent.OnLocalize();
            }
        }
        get
        {
            return _language;
        }
    }

    /// <summary>
    /// 檢驗是否含有此ＫＥＹ？
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public bool IsContainKey(string key)
    {
        return GetWorkingDictionary().ContainsKey(key);
    }

    /// <summary>
    /// 檢驗是否有以某字首為開頭的key
    /// </summary>
    /// <param name="prefix"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    static public bool IsContainKeyOfPrefix(string prefix, out int count)
    {
        var resultkeys = GetWorkingDictionary().Keys.Where(k => k.StartsWith(prefix)).ToArray();

        count = resultkeys.Length;

        return count > 0;
    }

    /// <summary>
    /// 取得值
    /// </summary>
    static public string GetText(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        if (dictionarys.Count == 0) LoadDictionary();
        Dictionary<string, string> workDictionary = GetWorkingDictionary();
        if (workDictionary == null || workDictionary.ContainsKey(key) == false)
        {
            return key;
        }

        return workDictionary[key];
    }

    static public string GetText(string key, string[] Args)
    {
        if (string.IsNullOrEmpty(key)) return null;
        if (dictionarys.Count == 0) LoadDictionary();
        Dictionary<string, string> workDictionary = GetWorkingDictionary();
        if (workDictionary == null || workDictionary.ContainsKey(key) == false)
        {
            return key;
        }

        string sString = GetText(key);

        for (int i = 0; i < Args.Length; i++)
        {
            sString = sString.Replace($"{{{i}}}", GetText(Args[i]));
        }

        return sString;
    }

    static public void EditorInitMe()
    {
        LoadDictionary();
    }
}
