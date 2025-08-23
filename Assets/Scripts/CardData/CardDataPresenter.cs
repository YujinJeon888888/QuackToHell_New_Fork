using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

public sealed class CardDataPresenter
{
    readonly CardDataModel _model = new();
    Task _readyTask;

    // 외부에서 호출: 로비 씬에서 1회만
    public Task PreloadAsync(string cardCsvUrl, string stringCsvUrl, string resourceCsvUrl, CancellationToken ct = default)
    {
        if (_readyTask != null) return _readyTask;
        _readyTask = PreloadImplAsync(cardCsvUrl, stringCsvUrl, resourceCsvUrl, ct);
        return _readyTask;
    }

    public Task WhenReadyAsync() => _readyTask ?? Task.CompletedTask;

    async Task PreloadImplAsync(string cardUrl, string strUrl, string resUrl, CancellationToken ct)
    {
        // 세 시트를 병렬 다운로드(모두 HTTP, 메인스레드 가능)
        var cardT = GetTextAsync(cardUrl, ct);
        var strT = GetTextAsync(strUrl, ct);
        var resT = GetTextAsync(resUrl, ct);

        var cardCsv = await cardT; var strCsv = await strT; var resCsv = await resT;

        _model.LoadCards(ParseCardTable(cardCsv));
        _model.LoadStrings(ParseStringTable(strCsv));
        _model.LoadResources(ParseResourceTable(resCsv));

        Debug.Log($"[CardData] Loaded: Cards={_model.Cards.Count}, Strings={_model.Cards.Count} (cards count shown), Resources OK");
    }

    // Public APIs for clients
    public bool TryGetCard(int cardId, out CardDef def) => _model.TryGetCard(cardId, out def);

    public bool TryGetCardDisplay(int cardId, string locale, out CardDisplay disp)
    {
        disp = default;
        if (!_model.TryGetCard(cardId, out var d)) return false;
        disp = new CardDisplay
        {
            CardID = d.CardID,
            Name = _model.Localize(d.CardNameKey.ToString(), locale),
            Description = _model.Localize(d.DescriptionKey.ToString(), locale),
            ImagePath = _model.ResolvePath(d.ImagePathKey.ToString()),
            Tier = d.Tier,
            Type = d.Type,
            BasePrice = d.BasePrice,
            BaseCost = d.BaseCost
        };
        return true;
    }

    public IReadOnlyDictionary<int, CardDef> Cards => _model.Cards; // 전체 사전 (읽기전용)
    public int CardCount => _model.Cards.Count;                      // 개수만 필요할 때

    // CSV Parsers (헤더 기반, 문자열/숫자 모두 허용)
    static IEnumerable<CardDef> ParseCardTable(string csv)
    {
        var rows = CardDataModel.SplitRows(csv);
        var list = new List<CardDef>(); if (rows.Count == 0) return list;

        var h = CardDataModel.SplitCols(rows[0]);
        int IDX(string name) { for (int i = 0; i < h.Count; i++) if (h[i].Trim().Equals(name, System.StringComparison.OrdinalIgnoreCase)) return i; return -1; }

        int iID = IDX("CardID"), iName = IDX("CardNameKey"), iTier = IDX("Tier"), iType = IDX("Type"),
            iSub = (IDX("SubType") >= 0 ? IDX("SubType") : IDX("SubType (사용X)")),
            iUni = IDX("IsUniqueCard"), iSell = IDX("IsSellableCard"),
            iClass = IDX("UsableClass"), iMap = IDX("Map_Restriction"),
            iPrice = IDX("BasePrice"), iCost = IDX("BaseCost"),
            iDesc = IDX("DescriptionKey"), iImg = IDX("ImagePathKey"),
            iAmount = IDX("AmountOfCardItem");

        for (int r = 1; r < rows.Count; r++)
        {
            var c = CardDataModel.SplitCols(rows[r]);
            if (c.Count == 0) continue;
            // 첫 열이 숫자가 아니면(타입 안내 행 등) 스킵
            if (!int.TryParse((iID >= 0 && iID < c.Count ? c[iID].Trim() : ""), out _)) continue;

            list.Add(new CardDef
            {
                CardID = CardDataModel.ToInt(S(c, iID)),
                CardNameKey = S(c, iName),
                Tier = CardDataModel.ToTier(S(c, iTier)),
                Type = CardDataModel.ToType(S(c, iType)),
                SubType = CardDataModel.ToInt(S(c, iSub)),
                IsUniqueCard = CardDataModel.ToBool(S(c, iUni)),
                IsSellableCard = CardDataModel.ToBool(S(c, iSell)),
                UsableClass = CardDataModel.ToInt(S(c, iClass)),
                Map_Restriction = CardDataModel.ToInt(S(c, iMap)),
                BasePrice = CardDataModel.ToInt(S(c, iPrice)),
                BaseCost = CardDataModel.ToInt(S(c, iCost)),
                DescriptionKey = S(c, iDesc),
                ImagePathKey = S(c, iImg),
                AmountOfCardItem = CardDataModel.ToInt(S(c, iAmount)),
            });
        }
        return list;
        static string S(List<string> c, int i) => (i >= 0 && i < c.Count) ? (c[i]?.Trim() ?? "") : "";
    }

    static IEnumerable<StringRow> ParseStringTable(string csv)
    {
        var rows = CardDataModel.SplitRows(csv);
        var list = new List<StringRow>(); if (rows.Count == 0) return list;
        var h = CardDataModel.SplitCols(rows[0]);

        int IDX(params string[] names)
        {
            for (int i = 0; i < h.Count; i++)
                foreach (var n in names)
                    if (h[i].Trim().Equals(n, System.StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }
        int iKey = IDX("Key", "StrID", "StringID"), iKR = IDX("KR", "KO", "Korean"), iEN = IDX("EN", "English");

        for (int r = 1; r < rows.Count; r++)
        {
            var c = CardDataModel.SplitCols(rows[r]);
            var key = (iKey >= 0 && iKey < c.Count) ? c[iKey].Trim() : "";
            if (string.IsNullOrEmpty(key)) continue;
            list.Add(new StringRow { Key = key, KR = (iKR >= 0 && iKR < c.Count ? c[iKR].Trim() : ""), EN = (iEN >= 0 && iEN < c.Count ? c[iEN].Trim() : "") });
        }
        return list;
    }

    static IEnumerable<ResourceRow> ParseResourceTable(string csv)
    {
        var rows = CardDataModel.SplitRows(csv);
        var list = new List<ResourceRow>(); if (rows.Count == 0) return list;
        var h = CardDataModel.SplitCols(rows[0]);

        int IDX(params string[] names)
        {
            for (int i = 0; i < h.Count; i++)
                foreach (var n in names)
                    if (h[i].Trim().Equals(n, System.StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }
        int iKey = IDX("Key", "ResID", "ImagePathKey"), iPath = IDX("Path", "ResourcePath", "SpritePath");

        for (int r = 1; r < rows.Count; r++)
        {
            var c = CardDataModel.SplitCols(rows[r]);
            var key = (iKey >= 0 && iKey < c.Count) ? c[iKey].Trim() : "";
            if (string.IsNullOrEmpty(key)) continue;
            list.Add(new ResourceRow { Key = key, Path = (iPath >= 0 && iPath < c.Count ? c[iPath].Trim() : "") });
        }
        return list;
    }

    // 공통 HTTP (메인 스레드에서 await; Awake 말고 Start에서 호출 권장)
    static async Task<string> GetTextAsync(string url, CancellationToken ct)
    {
        using var req = UnityWebRequest.Get(url);
        var op = req.SendWebRequest();
        while (!op.isDone) { if (ct.IsCancellationRequested) { req.Abort(); break; } await Task.Yield(); }
        if (req.result != UnityWebRequest.Result.Success) throw new System.Exception($"GET {url} -> {req.responseCode} {req.error}");
        return req.downloadHandler?.text ?? "";
    }
}
