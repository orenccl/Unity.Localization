using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CSVReader
{
    // 給外部查資料用
    private List<string> fields = new List<string>();

    public int Columns { get; private set; }
    public int Rows { get; private set; }
    public string this[int column, int row]
    {
        get { return fields[row * Columns + column]; }
    }

    // 內部分析資料用
    byte[] buffer;
    int offset = 0;

    bool IsDone { get { return (buffer == null || offset >= buffer.Length) ? true : false; } }
    bool IsNewLineChar(char c) { return (c == '\n' || c == '\r') ? true : false; }
    bool IsComma(char c) { return (c == ',') ? true : false; }
    bool IsQuote(char c) { return (c == '"') ? true : false; }
    bool IsLastIndex(string str, int i) { return i >= str.Length - 1 ? true : false; }

    /// <summary>
    ///  讀取一行
    /// </summary>
    /// <param name="skipEmptyLine"></param>
    /// <returns></returns>
    string ReadLine(bool skipEmptyLine)
    {
        int max = buffer.Length;
        // skip empty lines
        if (skipEmptyLine == true)
        {
            while (offset < max && buffer[offset] < 32) offset++;
        }

        // return one line
        int offsetTag = offset;
        string newLine = "";
        while (IsDone == false)
        {
            if (IsNewLineChar((char)buffer[offset]))
            {
                newLine = Encoding.UTF8.GetString(buffer, offsetTag, offset - offsetTag);
                offset++;
                return newLine;
            }
            else offset++;
        }
        return null;
    }

    /// <summary>
    /// 把每一行資料讀出
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public List<string> LoadCSV(TextAsset asset)
    {
        fields = new List<string>();
        Columns = 0;
        Rows = 0;
        buffer = asset.bytes;
        offset = 0;
        // 用來判定文字裡有沒有特殊符號：'"'
        bool insideQuotes = false;
        bool findField = false;
        List<char> field = new List<char>();
        while (IsDone == false)
        {
            string str = ReadLine(insideQuotes == true ? false : true);
            if (str == null) break;
            // 在雙引號內沒一次讀完=>被換了行
            if (insideQuotes == true) str = "\n" + str;
            str = str.Replace("\\n", "\n");
            for (int i = 0; i < str.Length; i++)
            {
                if (IsComma(str[i]))
                {
                    if (insideQuotes == false)
                    {
                        // 不在雙引號中，遇到 "," 表示一個欄位的結束
                        findField = true;
                    }
                    // 否則在雙引號中，遇到 "," 表示一個欄位內的內容
                    else field.Add(str[i]);
                }
                else if (IsQuote(str[i]))
                {
                    if (insideQuotes == false)
                    {
                        // 遇到雙引號，表示一個欄位內的開始
                        insideQuotes = true;
                        if (Columns == 0)
                        {
                            Debug.LogWarning("警告：表頭使用了特殊文字(,'\"')，載入結果將可能錯誤！");
                        }
                    }
                    else
                    {
                        // 遇到字串尾，表示一個欄位內的某特殊值或結束了...
                        if (IsLastIndex(str, i))
                        {
                            findField = true;
                        }
                        else
                        {
                            // 再遇到雙引號，表示一個欄位內的某特殊值或結束了...
                            if (IsQuote(str[i + 1]) == false)
                            {
                                findField = true;
                                // 萬一雙引號下一個是逗號，不可以再加field，跳掉這個逗號
                                if (IsComma(str[i + 1])) i++;
                            }
                            else
                            {
                                // 連雙引號只留一個單＂雙引號＂
                                field.Add(str[i++]);
                            }
                        }
                    }
                }
                else
                {
                    field.Add(str[i]);
                    if (IsLastIndex(str, i) && insideQuotes == false)
                    {
                        findField = true;
                    }
                }

                if (findField == true)
                {
                    string aField = new string(field.ToArray());
                    fields.Add(aField);
                    field.Clear();
                    findField = false;
                    insideQuotes = false;
                    if (IsLastIndex(str, i) == true && Columns == 0) Columns = fields.Count;
                }
            }
            // 補尾巴跳掉的空欄位
            if (Columns != 0 && insideQuotes == false)
            {
                while (fields.Count % Columns != 0) fields.Add("");
            }
        }
        if (Columns != 0) Rows = fields.Count / Columns;
        return fields;
    }
}
