using UnityEngine;

// 스프레드시트 한 행 = 이 구조체 한 개
using System;

[Serializable] // 필요 시 JsonUtility 등 활용 가능
public struct CardDef
{
    public int CardID;
    public string CardNameKey;
    public int Tier;
    public int Type;
    public int SubType;
    public bool IsUniqueCard;
    public bool IsSellableCard;
    public int UsableClass;     
    public int Map_Restriction;
    public int BasePrice;
    public int BaseCost;
    public string DescriptionKey;
    public string ImagePathKey;
}
