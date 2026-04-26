using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BattleScheduler : MonoBehaviour
{
    public EnemyGenerator enemyGenerator;
    public Board board;
    public LevelHandler levelHandler;
    public DamagePopup damagePopup;
    public RelicHandler relicHandler;
    public TMP_Text turnLabel;

    public int EnemyTurn { get; private set; } = 1;

    private BattleNPC Enemy => enemyGenerator.CurrentEnemy;

    void Awake()
    {
        if (relicHandler == null)
        {
            relicHandler = FindAnyObjectByType<RelicHandler>();
        }
    }

    public void ResolveTurn()
    {
        List<Player.MatchResult> chainMatches = board.player.chainMatches;

        foreach (Player.MatchResult match in chainMatches)
        {
            Debug.Log($"[BattleScheduler] Player performs {match.tileType} x{match.count}");
            TurnResult matchResult = ResolveMatch(match);
            ApplyTurnResult(matchResult);
            damagePopup?.AddResult(matchResult);

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

    private TurnResult ResolveMatch(Player.MatchResult match)
    {
        return new TurnResult
        {
            TotalDamage = ApplyRelicsToOutput(match.totalDamage, match.tileType),
            TotalBlock = ApplyRelicsToOutput(match.totalBlock, match.tileType),
            TotalHeal = ApplyRelicsToOutput(match.totalHeal, match.tileType),
            MatchCount = match.count
        };
    }

    private float ApplyRelicsToOutput(float baseValue, TileType tileType)
    {
        if (baseValue <= 0 || relicHandler == null || board == null || board.player == null)
        {
            return baseValue;
        }

        var relicMod = relicHandler.GetRelicOutputMod(board.player.RelicList, tileType);
        return Mathf.Round((baseValue * relicMod.multiplier) + relicMod.flatAdd);
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
