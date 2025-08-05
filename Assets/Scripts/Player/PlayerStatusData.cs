using System;
using UnityEngine;

/// <summary>  
/// 플레이어 데이터 구조  
/// </summary>  

public enum PlayerJob
{
    None,
}

[System.Serializable]
public struct PlayerStatusData
{
    public const int MaxCredibility = 100;
    public const int MaxSpellpower = 100;

    public string Nickname;
    public PlayerJob Job;
    public int Credibility;
    public int Spellpower;
    public int Gold;
    public float MoveSpeed;
}