using UnityEngine;
using System.Collections.Generic;

public class BattleScheduler : MonoBehaviour
{
    public EnemyGenerator enemyGenerator;
    public Board board;
    public LevelHandler levelHandler;
    public DamagePopup damagePopup;

    private BattleNPC Enemy => enemyGenerator.CurrentEnemy;

    public void ResolveTurn()
    {
        List<Player.MatchResult> chainMatches = board.player.chainMatches;

        foreach (Player.MatchResult match in chainMatches)
        {
            Debug.Log($"[BattleScheduler] Player performs {match.tileType} x{match.count}");
            TurnResult matchResult = ResolveTile(match.tileData, match.count);
            ApplyTurnResult(matchResult);

            if (Enemy.IsDead())
            {
                Debug.Log($"[BattleScheduler] {Enemy.Name} has died.");
                enemyGenerator.SpawnNext();
                board.player.chainMatches.Clear();
                if (damagePopup != null) damagePopup.Clear();
                return;
            }
        }

        board.player.chainMatches.Clear();

        Enemy.ExecuteIntent(board.player);
        Enemy.RollIntent();
        enemyGenerator.RefreshIntentLabel();

        if (damagePopup != null) damagePopup.Clear();

        CheckDeaths();
    }

    private void CheckDeaths()
    {
        if (board.player.CurrentHealth <= 0)
        {
            Debug.Log("[BattleScheduler] Player has died.");
            board.player.handleDeath();
            if (levelHandler != null) levelHandler.GameOver();
        }
    }

    private TurnResult ResolveTile(Tile tile, int count)
    {
        TurnResult result = new TurnResult();

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
            Enemy.TakeDamage(result.TotalDamage);

        if (result.TotalHeal > 0)
            board.player.handleHeal(result.TotalHeal);
    }
}
