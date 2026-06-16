using UnityEngine;

public class BattleNPC
{
    public string Name;
    public float MaxHealth;
    public float CurrentHealth;
    public float CurrentBlock;

    public int minAttack;
    public int maxAttack;
    public int minHeal;
    public int maxHeal;
    public EnemyAction[] actionPool{get;private set;}
    public EnemyAction IntentAction { get; private set; }
    public int IntentValue { get; private set; }

    public string IntentDescription
    {
        get
        {
            switch (IntentAction)
            {
                case EnemyAction.Attack: return $"{Name} will attack for {IntentValue}";
                case EnemyAction.Block:  return $"{Name} will block for {IntentValue}";
                case EnemyAction.Heal:   return $"{Name} will heal for {IntentValue}";
                case EnemyAction.Idle:   return $"{Name} is idle";
                default:                 return "";
            }
        }
    }

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
        actionPool = npcSO.actionPool != null && npcSO.actionPool.Length > 0
            ? npcSO.actionPool
            : new[] { EnemyAction.Attack, EnemyAction.Block, EnemyAction.Heal, EnemyAction.Idle };

        RollIntent();
    }

    public void RollIntent()
    {
        IntentAction = actionPool[Random.Range(0, actionPool.Length)];

        switch (IntentAction)
        {
            case EnemyAction.Attack:
            case EnemyAction.Block:
                IntentValue = Random.Range(minAttack, maxAttack + 1);
                break;
            case EnemyAction.Heal:
                IntentValue = Random.Range(minHeal, maxHeal + 1);
                break;
            case EnemyAction.Idle:
                IntentValue = 0;
                break;
        }
    }

    public void ExecuteIntent(Player player)
    {
        switch (IntentAction)
        {
            case EnemyAction.Attack:
                Debug.Log($"[Enemy] {Name} attacks for {IntentValue}");
                player.TakeDamage(IntentValue);
                break;

            case EnemyAction.Block:
                CurrentBlock += IntentValue;
                Debug.Log($"[Enemy] {Name} blocks for {IntentValue}");
                break;

            case EnemyAction.Heal:
                float healed = Mathf.Min(IntentValue, MaxHealth - CurrentHealth);
                CurrentHealth += healed;
                Debug.Log($"[Enemy] {Name} heals for {healed}");
                break;

            case EnemyAction.Idle:
                Debug.Log($"[Enemy] {Name} is idle");
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
