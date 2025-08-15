using UnityEngine;

using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

public sealed class GoogleSheetCsvCatalogService : ICardCatalogSource
{
    readonly string _csvUrl;
    public GoogleSheetCsvCatalogService(string csvUrl) => _csvUrl = csvUrl;

    public async Task<IReadOnlyList<CardDef>> LoadAsync(CancellationToken ct)
    {
        using var req = UnityWebRequest.Get(_csvUrl);
#if UNITY_2020_3_OR_NEWER
        var op = req.SendWebRequest();
        while (!op.isDone)
        {
            if (ct.IsCancellationRequested) { req.Abort(); break; }
            await Task.Yield();
        }
#else
        var op = req.SendWebRequest();
        while (!op.isDone) { if (ct.IsCancellationRequested) { req.Abort(); break; } await Task.Yield(); }
#endif
        if (req.result != UnityWebRequest.Result.Success)
            throw new System.Exception($"CSV download failed: {req.responseCode} {req.error}");

        return Parse(req.downloadHandler.text);
    }

    static IReadOnlyList<CardDef> Parse(string csv)
    {
        var rows = CsvLite.SplitRows(csv);
        var list = new List<CardDef>(rows.Count > 0 ? rows.Count - 1 : 0);
        if (rows.Count <= 1) return list;

        // 0행 = 헤더
        for (int r = 1; r < rows.Count; r++)
        {
            var line = rows[r].TrimEnd();
            if (string.IsNullOrEmpty(line)) continue;

            var c = CsvLite.SplitCols(line);
            if (c.Count < 13) continue; // 컬럼 13개 전제

            try
            {
                list.Add(new CardDef
                {
                    CardID = ToInt(c[0]),
                    CardNameKey = c[1],
                    Tier = ToInt(c[2]),
                    Type = ToInt(c[3]),
                    SubType = ToInt(c[4]),
                    IsUniqueCard = ToBool(c[5]),
                    IsSellableCard = ToBool(c[6]),
                    UsableClass = ToInt(c[7]),
                    Map_Restriction = ToInt(c[8]),
                    BasePrice = ToInt(c[9]),
                    BaseCost = ToInt(c[10]),
                    DescriptionKey = c[11],
                    ImagePathKey = c[12]
                });
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Card row parse failed (row={r}): {ex.Message}\n{line}");
            }
        }
        return list;
    }

    static int ToInt(string s) => int.Parse((s ?? "0").Trim(), CultureInfo.InvariantCulture);
    static bool ToBool(string s)
    {
        s = (s ?? "").Trim().ToLowerInvariant();
        return s == "true" || s == "1" || s == "y";
    }
}
