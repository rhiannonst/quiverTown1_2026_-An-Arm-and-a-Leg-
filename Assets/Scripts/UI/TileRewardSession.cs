using System;
using UnityEngine;

public static class TileRewardSession
{
    public static GameObject[] CurrentOptions { get; private set; }
    public static Relic BonusRelic { get; private set; }
    private static Action<GameObject> onTilePicked;

    public static bool HasActiveReward => CurrentOptions != null && onTilePicked != null;

    public static void Begin(GameObject[] options, Relic bonusRelic, Action<GameObject> onPicked)
    {
        CurrentOptions = options;
        BonusRelic = bonusRelic;
        onTilePicked = onPicked;
    }

    public static void Pick(GameObject tilePrefab)
    {
        Action<GameObject> callback = onTilePicked;
        Clear();
        callback?.Invoke(tilePrefab);
    }

    public static void Clear()
    {
        CurrentOptions = null;
        BonusRelic = null;
        onTilePicked = null;
    }
}
