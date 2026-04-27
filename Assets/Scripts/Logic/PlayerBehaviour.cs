using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class Player : MonoBehaviour
{
    public float MaxHealth;
    public float CurrentHealth;
    public float CurrentBlock;
    public string Name;
    public List<Relic> RelicList;
    public LevelHandler LevelHandler;

    public List<MatchResult> chainMatches = new List<MatchResult>();

    public EventReference PlayerHit_sfx;
    public EventReference LoseShield_sfx;
    public void TakeDamage(float damage)
    {
        if (CurrentBlock >= damage)
        {
            RuntimeManager.PlayOneShot(LoseShield_sfx);
            CurrentBlock -= damage;
        }
        else
        {
            RuntimeManager.PlayOneShot(PlayerHit_sfx);
            damage -= CurrentBlock;
            CurrentBlock = 0;
            CurrentHealth -= damage;
            Debug.Log($"Player took {damage} damage. Health is now {CurrentHealth}");
            if (CurrentHealth <= 0)
                handleDeath();
        }
    }

    public void AddBlock(float blockValue)
    {
        CurrentBlock += blockValue;
    }

    public void ResetBlock()
    {
        CurrentBlock = 0;
    }

    public void handleHeal(float healAmt)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + healAmt, MaxHealth);
    }

    public void handleDeath()
    {
        Debug.Log("Player has died.");
        LevelHandler.GameOver();
    }

    public void ReceiveMatchResults(List<MatchResult> results)
    {
        foreach (MatchResult result in results)
            chainMatches.Add(result);
    }

    [System.Serializable]
    public class MatchResult
    {
        public TileType tileType;
        public int count;
        public float totalDamage;
        public float totalBlock;
        public float totalHeal;

        public MatchResult(TileType tileType)
        {
            this.tileType = tileType;
        }
    }
}
