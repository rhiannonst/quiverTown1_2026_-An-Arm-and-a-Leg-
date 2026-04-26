using TMPro;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public NPC[] enemyPool;
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public TMP_Text intentLabel;
    public TMP_Text stageLabel;

    public float angleRange = 7f;

    public BattleNPC CurrentEnemy { get; private set; }
    public int Stage { get; private set; } = 1;

    private GameObject currentEnemyInstance;

    void Start()
    {
        SpawnNext();
    }

    public void SpawnNext()
    {
        if (currentEnemyInstance != null)
        {
            Rigidbody rb = currentEnemyInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.mass = .1f;
                rb.constraints = RigidbodyConstraints.None;
            }
        }

        NPC npc = enemyPool[Random.Range(0, enemyPool.Length)];
        CurrentEnemy = new BattleNPC(npc);
        Debug.Log($"[EnemyGenerator] Spawned: {CurrentEnemy.Name} (Stage {Stage})");

        if (enemyPrefab != null)
        {
            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion baseRot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
            Quaternion tilt = Quaternion.Euler(
                Random.Range(-angleRange, angleRange),
                Random.Range(-angleRange, angleRange),
                Random.Range(-angleRange, angleRange)
            );
            currentEnemyInstance = Instantiate(enemyPrefab, pos, baseRot * tilt);

            Rigidbody newRb = currentEnemyInstance.GetComponent<Rigidbody>();
            if (newRb != null)
                newRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        RefreshIntentLabel();
        RefreshStageLabel();
    }

    public void AdvanceStage()
    {
        Stage++;
        SpawnNext();
    }

    private void RefreshStageLabel()
    {
        if (stageLabel != null) stageLabel.text = $"Stage {Stage}";
    }

    public void RefreshIntentLabel()
    {
        if (intentLabel != null && CurrentEnemy != null)
            intentLabel.text = CurrentEnemy.IntentDescription;
    }
}
