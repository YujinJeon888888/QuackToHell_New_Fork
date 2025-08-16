using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;

#region Data Structs (기획서 타입 반영)
public enum TierEnum { None = 0, Common = 1, Rare = 2, Special = 3 }
public enum TypeEnum { None = 0, Attack = 1, Defense = 2, Special = 3 }

[Serializable]
public struct CardDef : INetworkSerializable
{
    public int CardID;
    public string CardNameKey;
    public TierEnum Tier;      // enum
    public TypeEnum Type;      // enum
    public int SubType;        // 사용 안 하면 0
    public bool IsUniqueCard;
    public bool IsSellableCard;
    public int UsableClass;      // 3bit
    public int Map_Restriction;  // 2bit
    public int BasePrice;
    public int BaseCost;
    public string DescriptionKey;
    public string ImagePathKey;
    public int AmountOfCardItem;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref CardID);
        serializer.SerializeValue(ref CardNameKey);
        serializer.SerializeValue(ref Tier);
        serializer.SerializeValue(ref Type);
        serializer.SerializeValue(ref SubType);
        serializer.SerializeValue(ref IsUniqueCard);
        serializer.SerializeValue(ref IsSellableCard);
        serializer.SerializeValue(ref UsableClass);
        serializer.SerializeValue(ref Map_Restriction);
        serializer.SerializeValue(ref BasePrice);
        serializer.SerializeValue(ref BaseCost);
        serializer.SerializeValue(ref DescriptionKey);
        serializer.SerializeValue(ref ImagePathKey);
        serializer.SerializeValue(ref AmountOfCardItem);
    }
}

[System.Serializable]
public enum CardItemState
{
    None,
    Sold,
}

[System.Serializable]
public struct CardItemStatusData : INetworkSerializable
{
    public int CardItemID;
    public int CardID;
    public int Price;
    public int Cost;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref CardItemID);
        serializer.SerializeValue(ref CardID);
        serializer.SerializeValue(ref Price);
        serializer.SerializeValue(ref Cost);
    }
}

[Serializable] public struct StringRow { public string Key; public string KR; public string EN; }
[Serializable] public struct ResourceRow { public string Key; public string Path; }

[Serializable]
public struct CardDisplay
{
    public int CardID;
    public string Name;
    public string Description;
    public string ImagePath;
    public TierEnum Tier;
    public TypeEnum Type;
    public int BasePrice;
    public int BaseCost;
}
#endregion

public sealed class CardDataModel : NetworkBehaviour
{
    readonly Dictionary<int, CardDef> _cards = new();
    readonly Dictionary<string, StringRow> _strings = new();
    readonly Dictionary<string, ResourceRow> _resources = new();

    public void LoadCards(IEnumerable<CardDef> rows) { _cards.Clear(); foreach (var r in rows) _cards[r.CardID] = r; }
    public void LoadStrings(IEnumerable<StringRow> rows) { _strings.Clear(); foreach (var r in rows) if (!string.IsNullOrEmpty(r.Key)) _strings[r.Key] = r; }
    public void LoadResources(IEnumerable<ResourceRow> r) { _resources.Clear(); foreach (var x in r) if (!string.IsNullOrEmpty(x.Key)) _resources[x.Key] = x; }

    public bool TryGetCard(int id, out CardDef def) => _cards.TryGetValue(id, out def);
    public IReadOnlyDictionary<int, CardDef> Cards => _cards;

    public string Localize(string key, string locale)
    {
        if (!_strings.TryGetValue(key, out var row)) return key ?? "";
        return locale switch { "ko" => row.KR ?? key, "en" => row.EN ?? key, _ => row.KR ?? row.EN ?? key };
    }
    public string ResolvePath(string key) => _resources.TryGetValue(key, out var r) ? r.Path : key ?? "";

    #region CSV Lite (경량 파서 & 매핑 유틸)
    public static List<string> SplitRows(string csv)
    {
        var t = (csv ?? "").Replace("\r\n", "\n").Replace("\r", "\n"); return new List<string>(t.Split('\n'));
    }
    public static List<string> SplitCols(string line)
    {
        var res = new List<string>(); if (line == null) { res.Add(""); return res; }
        bool q = false; var sb = new StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '\"') { if (q && i + 1 < line.Length && line[i + 1] == '\"') { sb.Append('\"'); i++; } else q = !q; }
            else if (c == ',' && !q) { res.Add(sb.ToString()); sb.Length = 0; }
            else sb.Append(c);
        }
        res.Add(sb.ToString()); return res;
    }
    public static int ToInt(string s) { s = (s ?? "").Trim(); if (s == "" || s == "-") return 0; return int.Parse(s, System.Globalization.CultureInfo.InvariantCulture); }
    public static bool ToBool(string s) { s = (s ?? "").Trim().ToLowerInvariant(); return s == "true" || s == "1" || s == "y"; }
    public static TierEnum ToTier(string s)
    {
        if (int.TryParse(s, out var n)) return (TierEnum)n; s = (s ?? "").Trim().ToLowerInvariant();
        return s switch { "common" => TierEnum.Common, "rare" => TierEnum.Rare, "special" => TierEnum.Special, _ => TierEnum.None };
    }
    public static TypeEnum ToType(string s)
    {
        if (int.TryParse(s, out var n)) return (TypeEnum)n; s = (s ?? "").Trim().ToLowerInvariant();
        return s switch { "attack" => TypeEnum.Attack, "defense" => TypeEnum.Defense, "special" => TypeEnum.Special, _ => TypeEnum.None };
    }
    #endregion
}
