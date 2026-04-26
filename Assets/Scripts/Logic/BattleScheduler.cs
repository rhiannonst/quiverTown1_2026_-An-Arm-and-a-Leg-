using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BattleScheduler : MonoBehaviour
{
    public EnemyGenerator enemyGenerator;
    public Board board;
    public LevelHandler levelHandler;
    public DamagePopup damagePopup;
    public TMP_Text turnLabel;

    public int EnemyTurn { get; private set; } = 1;

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
                enemyGenerator.AdvanceStage();
                EnemyTurn = 1;
                RefreshTurnLabel();
                board.player.chainMatches.Clear();
                if (damagePopup != null) damagePopup.Clear();
                return;
            }
        }

        board.player.chainMatches.Clear();

        Enemy.ExecuteIntent(board.player);
        Enemy.RollIntent();
        enemyGenerator.RefreshIntentLabel();
        EnemyTurn++;
        RefreshTurnLabel();

        if (damagePopup != null) damagePopup.Clear();

        CheckDeaths();
    }

    private void RefreshTurnLabel()
    {
        if (turnLabel != null) turnLabel.text = $"Turn {EnemyTurn}";
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
            case TileType.Head: // attack and block
                result.TotalDamage = tile.Damage * count;
                result.TotalBlock = tile.Block * count;
                break;
            case TileType.Arm:
            case TileType.Leg: // attack only
                result.TotalDamage = tile.Damage * count;
                break;

            case TileType.Torso: // block only
                result.TotalBlock = tile.Block * count;
                break;

            case TileType.Heart: // heal
                result.TotalHeal = tile.Heal * count;
                break;
            case TileType.Spine: // spine does nothing for now
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
