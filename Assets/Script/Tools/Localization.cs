using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Localization : MonoBehaviour
{
    private const string localeTextUrl = "Data/Localization/";
    private const string localeImageUrl = "Images/Localization/";

    // Singleton
    private static Localization instance = null;
    public static Localization GetInstance()
    {
        if (instance == null)
        {
            // 初始化
            instance = new GameObject("Localization").AddComponent<Localization>();
            DontDestroyOnLoad(instance.gameObject);

            instance.LoadAllLocaleTextData();
        }

        return instance;
    }

    // 目前使用的語系
    SystemLanguage currentLanguage = SystemLanguage.English;
    /// <summary>
    /// 讀取目前使用語系
    /// </summary>
    public SystemLanguage GetCurrentLanguage()
    {
        return currentLanguage;
    }

    /// <summary>
    /// 設定目前使用語系並通知刷新
    /// </summary>
    /// <param name="language">要使用語系</param>
    public void SetCurrentLanguage(SystemLanguage language)
    {
        if (allLocaleTextData.ContainsKey(language) == false)
        {
            Debug.LogError($"Language: {language} 沒有任何靜態資料，請檢查是否有設定，且設定需與Language enum一致!");
            return;
        }

        currentLanguage = language;
        StartCoroutine(OnLanguageChange());
    }

    // 所有語系的資料
    private readonly Dictionary<SystemLanguage, Dictionary<string, string>> allLocaleTextData = new Dictionary<SystemLanguage, Dictionary<string, string>>();

    /// <summary>
    /// 載入語系表
    /// </summary>
    /// <param name="fileName"></param>
    public void LoadAllLocaleTextData(string fileName = localeTextUrl)
    {
        allLocaleTextData.Clear();

        foreach (TextAsset asset in Resources.LoadAll<TextAsset>(fileName))
        {
            LoadCSVTextAsset(asset);
        }
    }

    /// <summary>
    /// 取得目前語系的資料
    /// </summary>
    /// <returns>目前語系的資料</returns>
    private Dictionary<string, string> GetCurrentLocaleTextData()
    {
        if (allLocaleTextData.ContainsKey(currentLanguage) == false)
        {
            Debug.LogError($"目前沒有載入任何 {currentLanguage} 的本地語言資料");
        }

        return allLocaleTextData[currentLanguage];
    }

    /// <summary>
    /// 載入語系表
    /// </summary>
    /// <param name="fileName"></param>
    public void LoadCSVTextAsset(TextAsset asset)
    {
        CSVReader csv = new CSVReader();
        csv.LoadCSV(asset);

        // TODO: 防呆報錯
        // col 0 存放key
        for (int col = 1; col < csv.Columns; ++col)
        {
            SystemLanguage language = StringToLanguageEnum(csv[col, 0]);

            if (!allLocaleTextData.ContainsKey(language))
            {
                allLocaleTextData[language] = new Dictionary<string, string>();
            }

            // row 0 存放對應的多國語言
            for (int row = 1; row < csv.Rows; ++row)
            {
                allLocaleTextData[language][csv[0, row]] = csv[col, row];
            }
        }
    }

    /// <summary>
    /// 檢驗是否含有此key
    /// </summary>
    /// <param name="localizationKey">本地化key</param>
    /// <returns>是否含有此key</returns>
    public bool IsContainKey(string localizationKey)
    {
        Dictionary<string, string> currentLocaleData = GetCurrentLocaleTextData();
        if (currentLocaleData == null)
        {
            return false;
        }

        return currentLocaleData.ContainsKey(localizationKey);
    }

    /// <summary>
    /// 獲取以某字串為開頭的key的數量
    /// </summary>
    /// <param name="prefix">開頭字串</param>
    /// <returns>有多少數量</returns>
    public int GetPrefixKeyCount(string prefix)
    {
        Dictionary<string, string> currentLocaleData = GetCurrentLocaleTextData();
        if (currentLocaleData == null)
        {
            return 0;
        }

        string[] resultkeys = currentLocaleData.Keys.Where(k => k.StartsWith(prefix)).ToArray();
        return resultkeys.Length;
    }

    /// <summary>
    /// 獲取本地化Text
    /// </summary>
    /// <param name="localizationKey">本地化鍵值</param>
    /// <param name="Args">要帶入字串的參數，會取代以 <i> 形式表示的字串，參數可以是數字也可以是鍵值</param>
    /// <returns>本地化Text</returns>
    public string GetLocaleText(string localizationKey, string[] args = null)
    {
        string sString = TryGetTextFromCurrentLocaleData(localizationKey);

        if (args == null)
        {
            return sString;
        }

        for (int i = 0; i < args.Length; i++)
        {
            sString = sString.Replace($"<{i}>", args[i]);
        }

        return sString;
    }

    /// <summary>
    /// 嘗試從目前設定語系中獲取localizationKey對應的值，查找不到時回傳localizationKey
    /// </summary>
    /// <param name="localizationKey">本地化鍵值</param>
    /// <returns>localizationKey對應的值，查找不到時回傳localizationKey</returns>
    private string TryGetTextFromCurrentLocaleData(string localizationKey)
    {
        Dictionary<string, string> currentLocaleData = GetCurrentLocaleTextData();
        // 查找不到的話回傳原本的
        if (currentLocaleData == null || currentLocaleData.ContainsKey(localizationKey) == false)
        {
            return localizationKey;
        }
        // 回傳對應的值
        return currentLocaleData[localizationKey];
    }

    /// <summary>
    /// 獲取本地化Sprite
    /// </summary>
    /// <param name="localizationKey">本地化鍵值</param>
    /// <returns>Sprite</returns>
    public Sprite GetLocaleSprite(string localizationKey)
    {
        // Sprite 命名格式如下 {localizationKey}_en_US、{localizationKey}_zh_TW
        string spriteUrl = $"{localeImageUrl}{localizationKey}_{currentLanguage}";
        // Unity會自動判斷是否載入過，不會重複載入
        Sprite sprite = Resources.Load<Sprite>(spriteUrl);
        if (sprite == null)
        {
            Debug.LogError($"找不到sprite, 請檢查路徑{spriteUrl}是否存在, 並且需把Texture type設定為sprite");
        }
        return sprite;
    }

    /// <summary>
    /// 初次載入語言，或語言變更時呼叫
    /// </summary>
    private IEnumerator OnLanguageChange()
    {
        // 找到所有的本地化元件
        LocaleComponentBase[] localeComponents = Object.FindObjectsOfType<LocaleComponentBase>(true);

        // 依序通知所有本地化元件更新
        foreach (LocaleComponentBase localeComponent in localeComponents)
        {
            localeComponent.Localize();
            yield return null; // 在每次迭代中暫停一幀
        }
    }

    /// <summary>
    /// 將字串轉換為Language enum格式
    /// </summary>
    /// <param name="str">要轉換的字串，需跟enum同名</param>
    /// <returns>Language enum或拋出系統錯誤</returns>
    private SystemLanguage StringToLanguageEnum(string str)
    {
        // 當enum解析失敗會拋出系統錯誤，表示靜態表語言參數錯誤
        return (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), str);
    }
}
