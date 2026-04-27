using UnityEngine;
using System.Collections.Generic;

public class RelicHandler : MonoBehaviour
{
    public Relic[] relicPool;

    public bool TryAddRandomRelic(Player player)
    {
        Relic chosenRelic = GetRandomAvailableRelic(player);
        return TryAddRelic(player, chosenRelic);
    }

    public Relic GetRandomAvailableRelic(Player player)
    {
        if (player == null || relicPool == null || relicPool.Length == 0)
        {
            UnityEngine.Debug.LogWarning("Cannot choose random relic because no player or relic pool is available.", this);
            return null;
        }

        if (player.RelicList == null)
        {
            player.RelicList = new List<Relic>();
        }

        List<Relic> availableRelics = new List<Relic>();
        foreach (Relic relic in relicPool)
        {
            if (relic != null && !player.RelicList.Contains(relic))
            {
                availableRelics.Add(relic);
            }
        }

        if (availableRelics.Count == 0)
        {
            UnityEngine.Debug.LogWarning("Cannot choose random relic because the player already has every relic in the pool.", this);
            return null;
        }

        return availableRelics[UnityEngine.Random.Range(0, availableRelics.Count)];
    }

    public bool TryAddRelic(Player player, Relic relic)
    {
        if (player == null || relic == null)
        {
            return false;
        }

        if (player.RelicList == null)
        {
            player.RelicList = new List<Relic>();
        }

        if (player.RelicList.Contains(relic))
        {
            return false;
        }

        player.RelicList.Add(relic);
        UnityEngine.Debug.Log($"[RelicHandler] Added relic: {relic.Name}");
        return true;
    }

    public static (float multiplier, float flatAdd) GetRelicOutputMod(IEnumerable<Relic> relics, TileType tileType)
    {
        float multiplier = 1f;
        float flatAdd = 0f;

        if (relics == null)
        {
            return (multiplier, flatAdd);
        }

        foreach (Relic relic in relics)
        {
            if (CanModifyOutput(relic, tileType))
            {
                multiplier += relic.multiplier;
                flatAdd += relic.flatAdd;
            }
        }

        return (multiplier, flatAdd);
    }

    private static bool CanModifyOutput(Relic relic, TileType tileType)
    {
        return relic != null
            && (relic.associatedTileType == TileType.Any
                || relic.associatedTileType == tileType);
    }
}
