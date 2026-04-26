public class BattleNPC
{
    NPC npcSO;

    public string Name;
    public float MaxHealth;
    public float CurrentHealth;
    public EnemyMove[] MoveList;
    public float CurrentBlock;

    public BattleNPC(NPC npcSO)
    {
        Name = npcSO.Name;
        MaxHealth = npcSO.MaxHealth;
        CurrentHealth = npcSO.MaxHealth;
        MoveList = npcSO.MoveList;
        CurrentBlock = npcSO.Block;
    }

    public void TakeDamage(float damage)
    {
        // case 1: block > damage
        if (CurrentBlock > 0 && CurrentBlock > damage)
            {
                CurrentBlock -= damage;
            }
        else // case 2: block is < damage 
        {
            damage -= CurrentBlock;
            CurrentBlock = 0;
            if (damage > 0)
            {
                CurrentHealth = CurrentHealth-damage;
            }
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
}