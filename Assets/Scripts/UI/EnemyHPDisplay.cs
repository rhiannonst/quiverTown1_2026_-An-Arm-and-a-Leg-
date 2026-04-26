using TMPro;
using UnityEngine;

public class EnemyHPDisplay : MonoBehaviour
{
    public EnemyGenerator enemyGenerator;
    public TMP_Text label;

    void Update()
    {
        if (enemyGenerator == null || label == null || this == null) return;

        BattleNPC enemy = enemyGenerator.CurrentEnemy;
        if (enemy == null)
        {
            label.text = "No Enemy";
            return;
        }

        label.text = $"{enemy.Name}: {enemy.CurrentHealth}/{enemy.MaxHealth}";
    }
}
