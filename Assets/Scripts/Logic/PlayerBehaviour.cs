using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerBehaviour : MonoBehaviour
{
    private Player playerSO;

    public Action<float> OnTakeDamage;

    public float MaxHealth;
    public float CurrentHealth;
    public string Name;
    public List<Relic> RelicList;
    public int BaseBlock;

    public List<MatchResult> chainMatches = new List<MatchResult>();

    public PlayerBehaviour()
    {
        MaxHealth = playerSO.MaxHealth;
        CurrentHealth = playerSO.MaxHealth;
        Name = playerSO.name;
        RelicList = new List<Relic>();
        BaseBlock = 0;
    }

    public void TakeDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage. Health is now {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Debug.Log($"Player took {damageAmount} damage. They have now died.");
        }
    }

    public void HandleTakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }

    public void ReceiveMatchResults(List<MatchResult> results)
    {
        foreach (MatchResult result in results)
        {
            chainMatches.Add(result);
            Debug.Log($"Match: {result.tileType} x{result.count}");
        }
    }

    [System.Serializable]
    public class MatchResult
    {
        public TileType tileType;
        public int count;

        public MatchResult(TileType tileType, int count)
        {
            this.tileType = tileType;
            this.count = count;
        }
    }
}
