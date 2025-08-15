using UnityEngine;

using System.Collections.Generic;
using System.Text;

public static class CsvLite
{
    // CSV 전체 문자열 -> 행 리스트
    public static List<string> SplitRows(string csv)
    {
        var text = csv.Replace("\r\n", "\n").Replace("\r", "\n");
        var rows = text.Split('\n');
        return new List<string>(rows);
    }

    // CSV 한 줄 -> 열 리스트 (따옴표/콤마 처리)
    public static List<string> SplitCols(string line)
    {
        var res = new List<string>();
        if (string.IsNullOrEmpty(line)) { res.Add(""); return res; }

        bool inQuotes = false;
        var sb = new StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '\"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '\"')
                { sb.Append('\"'); i++; } // "" -> "
                else { inQuotes = !inQuotes; }
            }
            else if (c == ',' && !inQuotes)
            { res.Add(sb.ToString()); sb.Length = 0; }
            else
            { sb.Append(c); }
        }
        res.Add(sb.ToString());
        return res;
    }
}
