using UnityEngine;
using System.Collections.Generic;

public class RelicHandler : MonoBehaviour
{
    public (float multiplier, float flatAdd) GetRelicOutputMod(IEnumerable<Relic> relics, TileType tileType)
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

    private bool CanModifyOutput(Relic relic, TileType tileType)
    {
        return relic != null
            && (relic.associatedTileType == TileType.Any
                || relic.associatedTileType == tileType);
    }
}
