using UnityEngine;
using System.Collections.Generic;
using TMPro;
using FMOD;
using FMODUnity;

public class BattleScheduler : MonoBehaviour
{
    public EnemyGenerator enemyGenerator;
    public Board board;
    public LevelHandler levelHandler;
    public DamagePopup damagePopup;
    public TMP_Text turnLabel;

    [SerializeField] public EventReference EnemyDie_sfx;
    [SerializeField] public EventReference PlayerDie_sfx;

    public int EnemyTurn { get; private set; } = 1;

    private BattleNPC Enemy => enemyGenerator.CurrentEnemy;

    public void ResolveTurn()
    {
        List<Player.MatchResult> chainMatches = board.player.chainMatches;

        foreach (Player.MatchResult match in chainMatches)
        {
            UnityEngine.Debug.Log($"[BattleScheduler] Player performs {match.tileType} x{match.count}");
            TurnResult matchResult = ResolveMatch(match);
            ApplyTurnResult(matchResult);

            if (Enemy.IsDead())
            {
                RuntimeManager.PlayOneShot(EnemyDie_sfx);
                UnityEngine.Debug.Log($"[BattleScheduler] {Enemy.Name} has died.");
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
            UnityEngine.Debug.Log("[BattleScheduler] Player has died.");
            RuntimeManager.PlayOneShot(PlayerDie_sfx);
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
        UnityEngine.Debug.Log($"[BattleScheduler] Turn result — Dmg:{result.TotalDamage} Blk:{result.TotalBlock} Heal:{result.TotalHeal}");

        if (result.TotalBlock > 0)
            board.player.AddBlock(result.TotalBlock);

        if (result.TotalDamage > 0)
            Enemy.TakeDamage(result.TotalDamage);

        if (result.TotalHeal > 0)
            board.player.handleHeal(result.TotalHeal);
    }
}
