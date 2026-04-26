using UnityEngine;
using System.Collections.Generic;

public class BattleScheduler : MonoBehaviour
{
    public BattleNPC enemy;
    public Board board;

    public void ResolveTurn()
    {
        List<Player.MatchResult> chainMatches = board.player.chainMatches;

        TurnResult turnResult = new TurnResult();
        foreach (Player.MatchResult match in chainMatches)
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

        // This is where we would apply any relic effects that modify the tile's behavior.
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
            board.player.AddBlock(result.TotalBlock);

        if (result.TotalDamage > 0)
            enemy.TakeDamage(result.TotalDamage);

        if (result.TotalHeal > 0)
            board.player.handleHeal(result.TotalHeal);
    }
}
