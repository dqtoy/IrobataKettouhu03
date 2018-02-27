using System;
using System.Collections.Generic;

/// <summary>
/// Utility class that defines an extension method for generic lists which provides random shuffling
/// based on the Fisher-Yates shuffle algorithm (https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle).
///
/// This is primarily used by the server when authoritatively shuffling the player decks.
/// Fisher-Yatesシャッフルアルゴリズム（https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle）に基づいてランダムシャッフリングを提供する汎用リストの拡張メソッドを定義するユーティリティクラス。
/// これは主にプレイヤーデッキを正式にシャッフルするときにサーバーによって使用されます。
/// </summary>
public static class ListShuffle
{
    private static Random rng = new Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            var k = (rng.Next(0, n) % n);
            n--;
            var x = list[k];
            list[k] = list[n];
            list[n] = x;
        }
    }
}