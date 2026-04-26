using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerBehaviour : MonoBehaviour
{
    public float MaxHealth;
    public float CurrentHealth;
    public string Name;
    public List<Relic> RelicList;
    public int BaseBlock;

    public List<MatchResult> chainMatches = new List<MatchResult>();

    public void TakeDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage. Health is now {CurrentHealth}");

        if (CurrentHealth <= 0)
            Debug.Log($"Player took {damageAmount} damage. They have now died.");
    }

    public void HandleTakeDamage(float damage)
    {
        CurrentHealth -= damage;
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
        public Tile tileData;

        public MatchResult(TileType tileType, int count, Tile tileData)
        {
            this.tileType = tileType;
            this.count = count;
            this.tileData = tileData;
        }
    }
}
