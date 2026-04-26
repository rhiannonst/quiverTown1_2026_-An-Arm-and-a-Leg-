using UnityEngine;

public class RelicHandler : MonoBehaviour
{
    public (float multiplier, float flatAdd) GetRelicOutputMod(Player player, TileType tileType)
    {
        float multiplier = 1f;
        float flatAdd = 0f;

        if (player == null || player.RelicList == null)
        {
            return (multiplier, flatAdd);
        }

        foreach (Relic relic in player.RelicList)
        {
            if (CanModifyDamage(relic, tileType))
            {
                multiplier += relic.multiplier;
                flatAdd += relic.flatAdd;
            }
        }

        return (multiplier, flatAdd);
    }

    private bool CanModifyDamage(Relic relic, TileType tileType)
    {
        return relic != null
            && (relic.associatedTileType == TileType.Any
                || relic.associatedTileType == tileType);
    }
}
