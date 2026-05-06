using System.Collections;
using TMPro;
using UnityEngine;
using FMODUnity;

public class EnemyGenerator : MonoBehaviour
{
    public float difficultyMod = 1f;
    public float difficultyMultiplier = 2f;
    public NPC[] enemyPool;
    public GameObject enemyPrefab;
    public TMP_Text intentLabel;
    public TMP_Text stageLabel;
    public EventReference spawnSFX;

    [Tooltip("Speed of the zip animation (higher = faster).")]
    public float zipSpeed = 8f;
    [Tooltip("Impulse force applied to the dead enemy when a new one arrives.")]
    public float deadEnemyKickForce = 12f;

    private static readonly Vector3 BattleSpot = new Vector3(0f, 0f, -4f);

    public BattleNPC CurrentEnemy { get; private set; }
    public int Stage { get; private set; } = 1;

    private GameObject currentEnemyInstance;

    void Start()
    {
        SpawnNext();
    }

    public void SpawnNext()
    {
        RuntimeManager.PlayOneShot(spawnSFX);
        // Release the dead enemy — kick it off screen
        RagdollCurrentEnemy();
        SpawnNewEnemy();
    }

    public void RagdollCurrentEnemy()
    {
        if (currentEnemyInstance != null)
        {
            Rigidbody rb = currentEnemyInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.mass = 0.1f;
                rb.constraints = RigidbodyConstraints.None;
                Vector3 kickDir = Vector3.back + Vector3.up * 0.5f;
                rb.AddForce(kickDir * deadEnemyKickForce, ForceMode.Impulse);
            }

            currentEnemyInstance = null;
        }
    }

    public void handleDifficultyMod(BattleNPC npc)
    {   
        
        int intMod = Mathf.RoundToInt(difficultyMod);

        npc.MaxHealth += difficultyMod;
        npc.CurrentHealth += difficultyMod;

        npc.maxAttack += intMod;
        npc.minAttack += intMod;

        npc.maxHeal += intMod;
        npc.minHeal += intMod;
    }
    
    private void SpawnNewEnemy()
    {
        // currently randomly spawns an enemy
        NPC npc = enemyPool[Random.Range(0, enemyPool.Length)];
        CurrentEnemy = new BattleNPC(npc);
        handleDifficultyMod(CurrentEnemy);
        Debug.Log($"[EnemyGenerator] Spawned: {CurrentEnemy.Name} (Stage {Stage})");

        GameObject prefabToSpawn = npc.Prefab != null ? npc.Prefab : enemyPrefab;
        if (prefabToSpawn != null)
        {
            // Spawn at the prefab's own saved position/rotation
            currentEnemyInstance = Instantiate(prefabToSpawn, prefabToSpawn.transform.position, prefabToSpawn.transform.rotation);

            Rigidbody newRb = currentEnemyInstance.GetComponent<Rigidbody>();
            if (newRb != null)
            {
                newRb.isKinematic = true; // kinematic during zip so nothing disrupts it
                newRb.constraints = RigidbodyConstraints.FreezeAll;
            }

            StartCoroutine(ZipToBattleSpot(currentEnemyInstance));
        }

        RefreshIntentLabel();
        RefreshStageLabel();
    }

    private IEnumerator ZipToBattleSpot(GameObject instance)
    {
        Vector3 start = instance.transform.position;

        while (instance != null)
        {
            instance.transform.position = Vector3.MoveTowards(
                instance.transform.position, BattleSpot, zipSpeed * Time.deltaTime);

            if (Vector3.Distance(instance.transform.position, BattleSpot) < 0.01f)
                break;

            yield return null;
        }

        if (instance == null) yield break;

        instance.transform.position = BattleSpot;

        // Settled — unfreeze physics but lock rotation and position so it stays stable
        Rigidbody rb = instance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void AdvanceStage()
    {
        Stage++;
        difficultyMod *= difficultyMultiplier;
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
