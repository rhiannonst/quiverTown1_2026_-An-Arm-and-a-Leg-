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

    // Debug hotkey for testing reshuffle + skip turn.
    public bool enableDebugHotkeys = true;
    public KeyCode reshuffleSkipTurnKey = KeyCode.R;

    public int EnemyTurn { get; private set; } = 1;

    private BattleNPC Enemy => enemyGenerator.CurrentEnemy;

    // this only checks the key.
    void Update()
    {
        if (enableDebugHotkeys
            && Input.GetKeyDown(reshuffleSkipTurnKey)
            && board.player.CurrentHealth > 0
            && Enemy.CurrentHealth > 0)
        {
            SkipPlayerTurnWithReshuffle();
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

    public void SkipPlayerTurnWithReshuffle()
    {
        board.Reshuffle();
        board.player.chainMatches.Clear();
        ResolveTurn();
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
            TotalDamage = match.totalDamage,
            TotalBlock = match.totalBlock,
            TotalHeal = match.totalHeal,
            MatchCount = match.count
        };
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
