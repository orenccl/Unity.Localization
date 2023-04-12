using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LocalizationSystem
{
    public class Localization
    {
        // Singleton
        private static Localization instance = null;

        // 設定檔存放位置
        private const string settingUrl = "Localization/LanguageSetting";
        // 本地文字存放位置，會被設定檔覆寫
        private string localeTextUrl = "Localization/Text/";
        // 本地圖片存放位置，會被設定檔覆寫
        private string localeImageUrl = "Localization/Image/";
        // 支持的語言清單，會被設定檔覆寫
        private List<SupportLanguage> supportLanguageList = new List<SupportLanguage>();

        // 目前使用的語系
        private SystemLanguage currentLanguage = SystemLanguage.English;
        // 多國文字資料
        private readonly Dictionary<string, string> localeTextData = new Dictionary<string, string>();

        /// <summary>
        /// 取得Singleton，並在初次使用時初始化
        /// </summary>
        /// <returns>Singleton</returns>
        public static Localization GetInstance()
        {
            if (instance == null)
            {
                // 初次被取用時初始化
                instance = new Localization();
                instance.Init();
            }

            return instance;
        }

        /// <summary>
        /// 讀取支持語系清單
        /// </summary>
        /// <returns>支持語系清單</returns>
        public List<SupportLanguage> GetSupportLanguageList()
        {
            return supportLanguageList;
        }

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
        /// <param name="setter">切換語系的物件，需繼承Monobehavior，用於Coroutine執行多國元件更新顯示</param>
        public void SetCurrentLanguage(SystemLanguage language, MonoBehaviour setter)
        {
            if (supportLanguageList.FindIndex(supportLanguage => supportLanguage.language == language) == -1)
            {
                Debug.LogError($"多國並不支援Language: {language}，請檢查 {settingUrl} 是否有設定!");
                return;
            }

            currentLanguage = language;
            // 重新載入多國資料
            LoadLocaleTextData();
            setter.StartCoroutine(OnLanguageChange());
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            // 載入設定檔
            LanguageSetting setting = Resources.Load<LanguageSetting>(settingUrl);
            if (setting == null)
            {
                Debug.LogError("請在Resources/Localization底下右鍵ScriptableObject/LanguageSetting建立設定，並命名為LanguageSetting");
                return;
            }

            // 初始化資源存放位置與支持的語言清單
            localeTextUrl = setting.localeTextUrl;
            localeImageUrl = setting.localeImageUrl;
            supportLanguageList = setting.supportLanguageList.ToList<SupportLanguage>();

            // 查找是否有支持當前系統語言，
            int index = supportLanguageList.FindIndex(supportLanguage => supportLanguage.language == Application.systemLanguage);

            // 有則設定為當前系統語言，沒有則設為supportLanguageList中第一個支持的語言
            currentLanguage = index == -1 ? supportLanguageList[0].language : supportLanguageList[index].language;

            // 載入多國語言文字資料
            LoadLocaleTextData();
        }

        /// <summary>
        /// 載入全部語系表
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadLocaleTextData()
        {
            // 重置資料
            localeTextData.Clear();

            // 載入對應語系表
            foreach (TextAsset asset in Resources.LoadAll<TextAsset>($"{localeTextUrl}{currentLanguage}"))
            {
                LoadJsonAsset(asset);
            }
        }

        private void LoadJsonAsset(TextAsset asset)
        {
            Dictionary<string, string> jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(asset.text);

            foreach (KeyValuePair<string, string> pair in jsonData)
            {
                localeTextData.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// 檢驗是否含有此key
        /// </summary>
        /// <param name="localizationKey">本地化key</param>
        /// <returns>是否含有此key</returns>
        public bool IsContainKey(string localizationKey)
        {
            return localeTextData.ContainsKey(localizationKey);
        }

        /// <summary>
        /// 獲取以某字串為開頭的key的數量
        /// </summary>
        /// <param name="prefix">開頭字串</param>
        /// <returns>有多少數量</returns>
        public int GetPrefixKeyCount(string prefix)
        {
            string[] resultkeys = localeTextData.Keys.Where(k => k.StartsWith(prefix)).ToArray();
            return resultkeys.Length;
        }

        /// <summary>
        /// 獲取本地化Text
        /// </summary>
        /// <param name="localizationKey">本地化鍵</param>
        /// <param name="Args">要帶入字串的參數，會取代以 <i> 形式表示的字串</param>
        /// <returns>本地化Text</returns>
        public string GetLocaleText(string localizationKey, string[] args = null)
        {
            string sString = TryGetTextFromCurrentLocaleData(localizationKey);
            // 沒給參數不處理
            if (args == null)
            {
                return sString;
            }
            // 有參數的話嘗試替換字串中的參數表示字元<0>、<1>...
            for (int i = 0; i < args.Length; i++)
            {
                sString = sString.Replace($"<{i}>", args[i]);
            }

            return sString;
        }

        /// <summary>
        /// 嘗試從目前設定語系中獲取localizationKey對應的值，查找不到時回傳localizationKey
        /// </summary>
        /// <param name="localizationKey">本地化鍵</param>
        /// <returns>localizationKey對應的值，查找不到時回傳localizationKey</returns>
        private string TryGetTextFromCurrentLocaleData(string localizationKey)
        {
            // 查找不到的話直接回傳Key
            if (localeTextData.ContainsKey(localizationKey) == false)
            {
                Debug.LogError($"找不到localizationKey所對應的值, 請檢查多國語言表中是否有設定該值");
                return localizationKey;
            }
            // 回傳對應的值
            return localeTextData[localizationKey];
        }

        /// <summary>
        /// 獲取本地化Sprite
        /// </summary>
        /// <param name="localizationKey">本地化鍵</param>
        /// <returns>Sprite</returns>
        public Sprite GetLocaleSprite(string localizationKey)
        {
            string spriteUrl = $"{localeImageUrl}{currentLanguage}/{localizationKey}";
            // Unity會自動判斷是否載入過，若載入過不會重複載入
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

            // 依序通知所有本地化元件更新，使用IEnumerator處理避免卡頓
            foreach (LocaleComponentBase localeComponent in localeComponents)
            {
                localeComponent.Localize();
                yield return null; // 在每次迭代中暫停一幀
            }
        }
    }
}