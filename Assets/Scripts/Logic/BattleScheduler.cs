using UnityEngine;
using System.Collections.Generic;
using TMPro;
using FMODUnity;

public class BattleScheduler : MonoBehaviour
{
    public EnemyGenerator enemyGenerator;
    public Board board;
    public LevelHandler levelHandler;
    public DamagePopup damagePopup;
    public RelicHandler relicHandler;
    public TMP_Text turnLabel;

    [SerializeField] public EventReference EnemyDie_sfx;
    [SerializeField] public EventReference PlayerDie_sfx;
    [SerializeField] public EventReference PlayerAttack_sfx;
    [SerializeField] public EventReference PlayerHeal_sfx;
    [SerializeField] public EventReference PlayerArmored_sfx;
    [SerializeField] public EventReference DamageBlocked_sfx;
    [SerializeField] public EventReference PlayerHit_sfx;

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
            UnityEngine.Debug.Log($"[BattleScheduler] Player performs {match.tileType} x{match.count}");
            TurnResult matchResult = ResolveMatch(match);
            ApplyTurnResult(matchResult);
            damagePopup?.AddResult(matchResult);

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

        board.player.ResetBlock();
        Enemy.ResetBlock();

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
        UnityEngine.Debug.Log($"[BattleScheduler] Turn result — Dmg:{result.TotalDamage} Blk:{result.TotalBlock} Heal:{result.TotalHeal}");

        if (result.TotalBlock > 0)
        {
            RuntimeManager.PlayOneShot(PlayerArmored_sfx);
            board.player.AddBlock(result.TotalBlock);
        }
            

        if (result.TotalDamage > 0)
        {
            RuntimeManager.PlayOneShot(PlayerAttack_sfx);
            Enemy.TakeDamage(result.TotalDamage);
        }
            

        if (result.TotalHeal > 0)
        {
            RuntimeManager.PlayOneShot(PlayerHeal_sfx);
            board.player.handleHeal(result.TotalHeal);
        }
            
    }
}
