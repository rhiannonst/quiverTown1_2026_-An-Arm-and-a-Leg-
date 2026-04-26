using UnityEngine;
using System.Collections.Generic;

public class BattleScheduler : MonoBehaviour
{
    //public BattlePlayer player;
    public BattleNPC enemy;
    public Board board;

    public void ResolveTurn()
    {
        List<PlayerBehaviour.MatchResult> chainMatches = board.player.chainMatches;

        TurnResult turnResult = new TurnResult();
        foreach (PlayerBehaviour.MatchResult match in chainMatches)
        {
            Debug.Log($"[BattleScheduler] Player performs {match.tileType} x{match.count}");
            TurnResult matchResult = ResolveTile(match.tileData, match.count);
            turnResult.Add(matchResult);
        }

        ApplyTurnResult(turnResult);
        board.player.chainMatches.Clear();
    }

    private TurnResult ResolveTile(Tile tile, int count)
    {
        TurnResult result = new TurnResult();

        // This is where we would apply any relic effects that modify the tile's behavior, e.g. "Head matches deal +1 damage per tile". For now we just calculate the base effect of the tile.  

        // Determine tile actions here
        switch (tile.Type)
        {
            case TileType.Head:
            case TileType.Arm:
            case TileType.Leg:
                result.TotalDamage = tile.Damage * count; 
                break;

            case TileType.Torso:
            case TileType.Spine:
                result.TotalBlock = tile.Block * count;
                break;

            case TileType.Heart:
                result.TotalHeal = tile.Heal * count;
                break;
        }

        return result;
    }

    private void ApplyTurnResult(TurnResult result)
    {
        Debug.Log($"[BattleScheduler] Turn result — Dmg:{result.TotalDamage} Blk:{result.TotalBlock} Heal:{result.TotalHeal}");

        if (result.TotalBlock > 0)
            //player.AddBlock(result.TotalBlock);

        if (result.TotalDamage > 0)
            enemy.TakeDamage(result.TotalDamage);

        if (result.TotalHeal > 0) return;
           // player.handleHeal(result.TotalHeal);
    }
}
