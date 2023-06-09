# Unity.Localization
Localization system

![image](https://github.com/orenccl/Unity.Localization/blob/main/Preview/Preview.gif)

## 功能
* Demo場景(展示UI操作多國功能、展示程式碼操作多國功能)
* LanguageSetting可自由調整多國檔案路徑及支持語言清單
* 實作Text、TextMeshPro、TextMeshProUGUI、Sprite元件
* TextMeshPro、TextMeshProUGUI可隨多國切換字體
* 多國文字可代換參數 e.g. 攻擊力: <0> -> 攻擊力: 3000

## 資料夾結構
- Localization (插件資料夾)
    - Scripts
        - Core (核心功能)
            - Localization.cs (載入多國語言表、查找多國鍵值等...)
            - LocaleComponentBase.cs (所有多國實作元件的Parent)
            - LocaleTMPTextBase.cs (多國實作TMP_Text的Parent)
            - LanguageSetting.cs (設定支持語言清單、資源路徑)
        - Components (實作元件)
            - LocaleText.cs (多國Text)
            - LocaleTextMeshPro.cs (多國TextMeshPro)
            - LocaleTextMeshProUGUI.cs (多國TextMeshProUGUI)
            - LocaleSprite.cs (多國Sprite)
            - LocalizationDropdown.cs (多國選單)
    - Prefabs (將Components實際套用，可參考Prefab是如何設定)
        - LocaleText.prefab
        - LocaleTextMeshPro.prefab
        - LocaleTextMeshProUGUI.prefab
        - LocaleSprite.prefab
        - LocalizationDropdown.prefab
    - Demo
        - Scripts
            - DemoUpdateTextKeyAndArgs.cs (更新文字鍵與參數並刷新顯示的範例)
            - DemoUpdateImageKey.cs (更新圖片鍵並刷新顯示的範例)
        - Scene
            - Demo.unity (完整示範場景)
            
- Resources (資源存放資料夾)
    - Localization
        - LanguageSetting.asset (必須要有，設定支持語言清單與存放路徑)
        - Demo (Demo演示用素材)
            - Fonts (Demo用繁簡字體)
                - 略
            - Image (Demo用圖片)
                - Language_English.png (英文圖片)
                - Language_ChineseTraditional.png (繁體圖片)
                - Language_ChineseSimplified.png (簡體圖片)
                - 略
            - Text (Demo用多國語言表)
                - LocalizationText.csv (多國語言表範例)

## API
### Localization
* 取得Singleton，並在初次使用時初始化
`public static Localization GetInstance()`

* 取得支持語系清單
`public List<SupportLanguage> GetSupportLanguageList()`

* 讀取目前使用語系
`public SystemLanguage GetCurrentLanguage()`

* 設定目前使用語系並通知刷新
`public void SetCurrentLanguage(SystemLanguage language, MonoBehaviour setter)`

* 檢驗是否含有此key
`public bool IsContainKey(string localizationKey)`
        
* 獲取以某字串為開頭的key的數量
`public int GetPrefixKeyCount(string prefix)`
        
* 獲取本地化Text
`public string GetLocaleText(string localizationKey, string[] args = null)`
        
* 獲取本地化Sprite
`public Sprite GetLocaleSprite(string localizationKey)`

### LocaleComponentBase
* 使用事先在Inspector設定好的鍵進行本地化
`public abstract void Localize()`

* 重設鍵, 並刷新顯示
`public void UpdateKey(string newLocalizationKey)`

### LocaleText/LocaleTMPTextBase (礙於Unity架構，普通Text與TMPText無法整合成同一Class，但API皆相同)
* 重設參數, 並刷新顯示
`public void UpdateArgs(string[] newArgs)`
        
* 重設鍵與參數, 並刷新顯示
`public void UpdateKeyAndArgs(string newLocalizationKey, string[] newArgs)`

## 使用方式
1. 需在Resources/Localization右鍵選單ScriptableObject建立LanguageSetting，並設定要支持的語言與素材存放路徑

2. 使用Json建立多國語言表，並放在相對應語言的路徑，可查看Demo範例。

3. 將多國圖片放在相對應語言的路徑，可查看Demo範例。
