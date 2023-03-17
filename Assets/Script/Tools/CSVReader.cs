using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CSVReader
{
    // 載入的CSV表格資料
    private List<string> tableData = new List<string>();

    public int Columns { get; private set; }
    public int Rows { get; private set; }
    public string this[int row, int col]
    {
        get { return tableData[row * Columns + col]; }
    }

    private bool IsNewLineChar(char c) { return (c == '\n' || c == '\r'); }
    private bool IsComma(char c) { return (c == ','); }
    private bool IsQuote(char c) { return (c == '"'); }
    private bool IsLastIndex(string str, int i) { return i >= str.Length - 1; }

    /// <summary>
    /// 把每一行資料讀出
    /// </summary>
    /// <param name="asset"></param>
    public void LoadCSV(in TextAsset asset)
    {
        // 初始化
        tableData.Clear();
        Columns = 0;
        Rows = 0;

        if(asset.bytes == null)
        {
            Debug.LogError($"載入的CSV{asset.name}，沒有任何資料!");
            return;
        }


        // 是否被雙引號包圍，有特殊字元的字串會被雙引號包圍
        bool insideQuotes = false;
        bool findField = false;
        List<char> field = new List<char>();

        int offset = 0;
        while (offset < asset.bytes.Length)
        {
            // 讀取一行資料
            string str = ReadLine(asset.bytes, ref offset, insideQuotes);
            if (str == null)
            { 
                break; 
            }

            // 在雙引號內沒一次讀完=>被換了行，補上換行
            if (insideQuotes) 
            {
                str = "\n" + str;
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (IsComma(str[i]))
                {
                    // 在雙引號中，遇到 "," 表示一個欄位內的內容
                    if (insideQuotes)
                    {
                        field.Add(str[i]);
                    }
                    // 不在雙引號中，遇到 "," 表示一個欄位的結束
                    else
                    {
                        findField = true;
                    }
                }
                else if (IsQuote(str[i]))
                {
                    // 初次遇到雙引號，表示一個欄位內的開始
                    if (insideQuotes == false)
                    {
                        insideQuotes = true;
                        if (Columns == 0)
                        {
                            Debug.LogWarning("警告：表頭使用了特殊文字(,'\"')，載入結果將可能錯誤！");
                        }
                    }
                    else
                    {
                        // 遇到字串尾，表示一個欄位的結束
                        if (IsLastIndex(str, i))
                        {
                            findField = true;
                        }
                        else
                        {
                            // 再遇到單個雙引號，表示一個欄位的結束
                            if (IsQuote(str[i + 1]) == false)
                            {
                                findField = true;
                                // 萬一雙引號下一個是逗號，不可以再加到field，跳掉這個逗號
                                if (IsComma(str[i + 1]))
                                {
                                    i++;
                                }
                            }
                            // 遇到連續雙引號
                            else
                            {
                                // 只紀錄一個＂雙引號＂
                                field.Add(str[i]);
                                // 跳過其中一個單引號
                                ++i;
                            }
                        }
                    }
                }
                else
                {
                    // 將字元加入欄位內容
                    field.Add(str[i]);
                    // 字串讀取完畢且不再雙引號內，表示欄位讀取完成
                    if (IsLastIndex(str, i) && insideQuotes == false)
                    {
                        findField = true;
                    }
                }

                if (findField)
                {
                    tableData.Add(new string(field.ToArray()));

                    // 清空狀態
                    field.Clear();
                    findField = false;
                    insideQuotes = false;

                    // 讀完第一列的時候紀錄行數
                    if (IsLastIndex(str, i)  && Columns == 0)
                    {
                        Columns = tableData.Count;
                    }
                }
            }

            // 補上列結尾被略過的的空欄位
            if (Columns != 0 && insideQuotes == false)
            {
                while (tableData.Count % Columns != 0)
                {
                    tableData.Add("");
                }
            }
        }

        if(Columns == 0)
        {
            Debug.LogError("CSV解析錯誤，行數為0");
            return;
        }

        Rows = tableData.Count / Columns;
    }


    /// <summary>
    ///  讀取一行
    /// </summary>
    /// <param name="skipEmptyLine"></param>
    /// <returns></returns>
    string ReadLine(byte[] bytes, ref int offset, bool insideQuotes)
    {
        int max = bytes.Length;
        // 不在雙引號中間時，略過所有空白行
        if (insideQuotes == false)
        {
            while (offset < max && bytes[offset] < 32) offset++;
        }

        // 讀取一行並返回
        int offsetTag = offset;
        while (offset < bytes.Length)
        {
            if (IsNewLineChar((char)bytes[offset]))
            {
                string line = Encoding.UTF8.GetString(bytes, offsetTag, offset - offsetTag);
                offset++;
                // 修正換行符號修正
                return line.Replace("\\n", "\n");
            }
            else
            {
                offset++;
            }
        }
        return null;
    }
}
