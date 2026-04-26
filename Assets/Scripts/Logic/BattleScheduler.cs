using UnityEngine;
using System;
using System.Security.Cryptography;

public class BattleScheduler : MonoBehaviour
{
    public event Action<float> OnTakeDamage;
    public BattlePlayer player;
    public BattleNPC enemy;

    TurnResult currentTurnResult;
    
    public Board board;
    [SerializeField]
    public BattleNPC[] enemyList;
    public BattleNPC currentEnemy;
    private int currentIndex = 0;

    // helper function
    public void cycleEnemyList()
    {
        currentIndex = (currentIndex + 1) % enemyList.Length;
        currentEnemy = enemyList[currentIndex];
    }

    public void AccumulateWave(TurnResult waveResult)
    {
        currentTurnResult.Add(waveResult);
        Debug.Log($"[Wave] +Dmg:{waveResult.TotalDamage} +Blk:{waveResult.TotalBlock} +Heal:{waveResult.TotalHeal}");
    }

    // helper function 
    public TurnResult ResolveTile(Tile tile, int chainValue)
    {
        TurnResult result = default;

        switch (tile.Type)
        {
            case TileType.Head:
            case TileType.Arm:
            case TileType.Leg:
                result.TotalDamage = tile.Damage * chainValue;
                break;

            case TileType.Torso:
            case TileType.Spine:
                result.TotalBlock = tile.Block * chainValue;
                break;

            case TileType.Heart:
                result.TotalHeal = tile.Heal * chainValue;
                break;
        }

        return result;
    }

    public void ResolveTurn()
    {
        Debug.Log($"Turn resolved: Damage={currentTurnResult.TotalDamage} " +
                $"Block={currentTurnResult.TotalBlock} Heal={currentTurnResult.TotalHeal}");
        ApplyTurnResult(currentTurnResult);
        currentTurnResult.Reset();
    }

    public void ApplyTurnResult(TurnResult result)
    {
        // Order matters: block first, then deal damage, then heal
        if (result.TotalBlock > 0)
            player.AddBlock(result.TotalBlock);

        if (result.TotalDamage > 0)
            currentEnemy.TakeDamage(result.TotalDamage);

        if (result.TotalHeal > 0)
            player.handleHeal(result.TotalHeal);
    }
    
    // handle Battle
    public void TriggerBattle()
    {
        //trigger first enemy
        currentEnemy = enemyList[0];
        board = new Board();
        
    }

    public void Start()
    {
        // subscriber (catcher)
        //playerBehaviour.TakeDamage += onDamageTaken;
        
        //playerBehaviour.TakeDamage(onDamageTaken());
        //PlayerBehaviour.TakeDamage(playerBehaviour.OnTakeDamage());
        //player.TakeDamage(damageAmount);
    }

    public void Update() //this is improper syntax psuedo code done by event mentor as example.
    {
        /* if (hits(player, enemy))
        {
            OnTakeDamage(enemy);
        } */
    }


    //private void OnEnable()
    //{
    //    // Subscribe to the damage event
    //    RegisterPlayer();
    //}

    //private void OnDisable()
    //{
    //    // Unsubscribe to prevent memory leaks
    //    UnRegisterPlayer();
    //}
}