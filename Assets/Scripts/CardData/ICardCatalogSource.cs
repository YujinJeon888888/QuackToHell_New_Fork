using UnityEngine;

// 비동기 로더 인터페이스: 어디서든 교체 가능(GoogleSheet, StreamingAssets, Server API 등)
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface ICardCatalogSource
{
    Task<IReadOnlyList<CardDef>> LoadAsync(CancellationToken ct);
}
