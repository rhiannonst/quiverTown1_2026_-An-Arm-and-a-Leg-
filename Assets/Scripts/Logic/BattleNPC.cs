using UnityEngine;

public class BattleNPC
{
    public string Name;
    public float MaxHealth;
    public float CurrentHealth;
    public float CurrentBlock;

    private readonly int minAttack;
    private readonly int maxAttack;
    private readonly int minHeal;
    private readonly int maxHeal;

    public enum EnemyAction { Attack, Heal, Block }

    public BattleNPC(NPC npcSO)
    {
        Name = npcSO.Name;
        MaxHealth = npcSO.MaxHealth;
        CurrentHealth = npcSO.MaxHealth;
        CurrentBlock = npcSO.Block;
        minAttack = npcSO.MinAttack;
        maxAttack = npcSO.MaxAttack;
        minHeal = npcSO.MinHeal;
        maxHeal = npcSO.MaxHeal;
    }

    public void TakeTurn(Player player)
    {
        EnemyAction action = (EnemyAction)Random.Range(0, 3);

        switch (action)
        {
            case EnemyAction.Attack:
                int dmg = Random.Range(minAttack, maxAttack + 1);
                Debug.Log($"[Enemy] {Name} attacks for {dmg}");
                player.TakeDamage(dmg);
                break;

            case EnemyAction.Heal:
                int healAmt = Random.Range(minHeal, maxHeal + 1);
                float healed = Mathf.Min(healAmt, MaxHealth - CurrentHealth);
                CurrentHealth += healed;
                Debug.Log($"[Enemy] {Name} heals for {healed}");
                break;

            case EnemyAction.Block:
                int blockAmt = Random.Range(minAttack, maxAttack + 1);
                CurrentBlock += blockAmt;
                Debug.Log($"[Enemy] {Name} blocks for {blockAmt}");
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        if (CurrentBlock >= damage)
        {
            CurrentBlock -= damage;
        }
        else
        {
            damage -= CurrentBlock;
            CurrentBlock = 0;
            CurrentHealth -= damage;
        }
    }

    public void AddBlock(float blockValue) => CurrentBlock += blockValue;
    public void ResetBlock() => CurrentBlock = 0;
    public bool IsDead() => CurrentHealth <= 0;
}
