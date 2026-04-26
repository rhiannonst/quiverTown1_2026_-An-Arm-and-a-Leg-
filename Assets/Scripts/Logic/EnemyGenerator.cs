using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public NPC[] enemyPool;

    public BattleNPC CurrentEnemy { get; private set; }

    void Start()
    {
        SpawnNext();
    }

    public void SpawnNext()
    {
        NPC npc = enemyPool[Random.Range(0, enemyPool.Length)];
        CurrentEnemy = new BattleNPC(npc);
        Debug.Log($"[EnemyGenerator] Spawned: {CurrentEnemy.Name}");
    }
}
