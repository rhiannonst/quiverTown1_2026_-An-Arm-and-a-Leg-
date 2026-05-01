using UnityEngine;
using System.Collections.Generic;
using TMPro;
using FMODUnity;
using System.Collections;

public class BattleScheduler : MonoBehaviour
{
    public EnemyGenerator enemyGenerator;
    public Board board;
    public LevelHandler levelHandler;
    public DamagePopup damagePopup;
    public RelicHandler relicHandler;
    public TileRewardManager tileRewardManager;
    public TMP_Text turnLabel;

    public BattleUI battleUI = new BattleUI();

    [SerializeField] public EventReference EnemyDie_sfx;
    [SerializeField] public EventReference PlayerDie_sfx;
    [SerializeField] public EventReference PlayerAttack_sfx;
    [SerializeField] public EventReference PlayerHeal_sfx;
    [SerializeField] public EventReference PlayerArmored_sfx;
    [SerializeField] public EventReference DamageBlocked_sfx;
    [SerializeField] public EventReference PlayerHit_sfx;

    public int EnemyTurn { get; private set; } = 1;
    public int TotalTurns { get; private set; } = 0;

    private BattleNPC Enemy => enemyGenerator.CurrentEnemy;

    void Awake()
    {
        if (relicHandler == null)
        {
            relicHandler = FindAnyObjectByType<RelicHandler>();
        }

        if (tileRewardManager == null)
        {
            tileRewardManager = FindAnyObjectByType<TileRewardManager>();
        }
    }

    void Start()
    {
        
    }
    
    // To-do: we need to implement some simple implemnations during the yield return to play.
    public IEnumerator handleTurnSequence()
    {
        List<Player.MatchResult> chainMatches = board.player.chainMatches;
        TurnResult finalPopupResult = new TurnResult();

        // block value for both player and enemy fade in and out

        foreach (Player.MatchResult match in chainMatches)
        {
            // UnityEngine.Debug.Log($"[BattleScheduler] Player performs {match.tileType} x{match.count}");
            TurnResult matchResult = ResolveMatchAndApplyRelics(match);
            finalPopupResult.Add(matchResult);
            ApplyTurnResult(matchResult);
            yield return new WaitForSeconds(1.5f);

            if (Enemy.IsDead())
            {
                damagePopup?.SetResult(finalPopupResult);
                HandleEnemyDefeated();
                yield break;
            }
        }

        yield return StartCoroutine(handleEnemyTurnAndClean(chainMatches, finalPopupResult));

    }

    // handles clean up after a turn sequence finishes
    public IEnumerator handleEnemyTurnAndClean(List<Player.MatchResult> chainMatches,TurnResult turnResult)
    {
        damagePopup?.SetResult(turnResult);
        chainMatches.Clear();
        yield return new WaitForSeconds(1.5f);
        Enemy.ExecuteIntent(board.player);
        yield return StartCoroutine(battleUI.blockNumberTransition(board,Enemy));
        
        Enemy.RollIntent();
        enemyGenerator.RefreshIntentLabel();
        EnemyTurn++;
        TotalTurns++;
        RefreshTurnLabel();

        if (damagePopup != null) damagePopup.Clear();

        CheckDeaths();
    }

    private void RefreshTurnLabel()
    {
        if (turnLabel != null) turnLabel.text = $"Turn {EnemyTurn}";
    }

    private void HandleEnemyDefeated()
    {
        RuntimeManager.PlayOneShot(EnemyDie_sfx);
        UnityEngine.Debug.Log($"[BattleScheduler] {Enemy.Name} has died.");
        enemyGenerator.RagdollCurrentEnemy();

        board.player.chainMatches.Clear();
        if (damagePopup != null) damagePopup.Clear();

        if (tileRewardManager != null && tileRewardManager.OfferTileReward(AdvanceAfterReward, enemyGenerator.Stage))
        {
            return;
        }

        AdvanceAfterReward();
    }

    private void AdvanceAfterReward()
    {
        enemyGenerator.AdvanceStage();
        EnemyTurn = 1;
        RefreshTurnLabel();
    }

    private void CheckDeaths()
    {
        if (board.player.CurrentHealth <= 0)
        {
            UnityEngine.Debug.Log("[BattleScheduler] Player has died.");
            RuntimeManager.PlayOneShot(PlayerDie_sfx);
            board.player.handleDeath();
            //if (levelHandler != null) levelHandler.GameOver(); //redundant call
        }
    }

    public TurnResult ResolveMatchAndApplyRelics(Player.MatchResult match)
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
        if (baseValue <= 0 || board == null || board.player == null)
        {
            return baseValue;
        }

        var relicMod = RelicHandler.GetRelicOutputMod(board.player.RelicList, tileType);
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
