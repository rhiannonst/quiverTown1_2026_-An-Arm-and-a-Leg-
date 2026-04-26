using UnityEngine;
using UnityEngine.UIElements;

public class BattleUI : MonoBehaviour
{
    UIDocument uiDoc;
    
    VisualElement root;
    
    VisualElement playerHealthBar;

    VisualElement enemyHealthBar;

    Label playerBlockText;

    Label enemyBlockText;

    public Player player;

    public BattleNPC npc;

    public EnemyGenerator enemyGenerator;

    
    public void UpdateHealthBar(Player player, BattleNPC npc)
    {
        float playerhealthPercentage = player.CurrentHealth / player.MaxHealth * 100;
        playerHealthBar.style.width = Length.Percent(playerhealthPercentage);

        float npcHealthPercentage = npc.CurrentHealth/ npc.MaxHealth * 100;
        enemyHealthBar.style.width = Length.Percent(npcHealthPercentage);
    }

    public void UpdateBlockText(Player player, BattleNPC npc)
    {
        playerBlockText.text = player.CurrentBlock.ToString();
        enemyBlockText.text = npc.CurrentBlock.ToString();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        playerHealthBar = root.Q<VisualElement>("playerHealthBar");
        enemyHealthBar = root.Q<VisualElement>("enemyHealthBar");
        playerBlockText = root.Q<Label>("playerBlockText");
        enemyBlockText = root.Q<Label>("enemyBlockText");
    }

    // Update is called once per frame
    void Update()
    {   
        //check HealthBar every tick
        UpdateHealthBar(player,enemyGenerator.CurrentEnemy);
        UpdateBlockText(player,enemyGenerator.CurrentEnemy);
    }
}
