using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FMODUnity;
using System.Collections;

public class BattleUI : MonoBehaviour
{
    // element ref list
    UIDocument uiDoc;
    VisualElement root;
    VisualElement playerHealthBar;
    VisualElement enemyHealthBar;
    Label playerBlockText;
    Label enemyBlockText;
    Button bagButton;
    Button helperButton;
    
    public Player player;
    public BattleNPC npc;
    public EnemyGenerator enemyGenerator;
    public List<Relic> relicList;
    public VisualElement relicInventoryContainer;
    public VisualElement helperContainer;
    public VisualElement container;
    public EventReference openBagSFX;

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

    private void PopulateRelics()
    {
        container.Clear();
        container.style.paddingTop = 12f;
        container.style.paddingBottom = 12f;
        container.style.paddingLeft = 16f;
        container.style.paddingRight = 16f;

        foreach (Relic relic in relicList)
        {
            // Card — horizontal row, white bg, rounded corners, shadow-style border
            var card = new VisualElement();
            card.style.flexDirection = FlexDirection.Row;
            card.style.alignItems = Align.Center;
            card.style.backgroundColor = new StyleColor(new Color(0.97f, 0.97f, 0.97f));
            card.style.borderTopLeftRadius = 8f;
            card.style.borderTopRightRadius = 8f;
            card.style.borderBottomLeftRadius = 8f;
            card.style.borderBottomRightRadius = 8f;
            card.style.borderBottomColor = new StyleColor(new Color(0.2f, 0.4f, 0.8f));
            card.style.borderBottomWidth = 3f;
            card.style.paddingTop = 10f;
            card.style.paddingBottom = 10f;
            card.style.paddingLeft = 12f;
            card.style.paddingRight = 12f;
            card.style.marginBottom = 10f;

            // Icon — fixed square, no stretching
            var icon = new Image { sprite = relic.icon };
            icon.style.width = 56f;
            icon.style.height = 56f;
            icon.style.flexShrink = 0;
            icon.style.marginRight = 14f;
            card.Add(icon);

            // Text column
            var textCol = new VisualElement();
            textCol.style.flexDirection = FlexDirection.Column;
            textCol.style.flexGrow = 1f;

            var name = new Label(relic.Name);
            name.style.fontSize = 16f;
            name.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
            name.style.unityFontStyleAndWeight = FontStyle.Bold;
            name.style.marginBottom = 3f;
            textCol.Add(name);

            var description = new Label(relic.description);
            description.style.fontSize = 12f;
            description.style.color = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            description.style.whiteSpace = WhiteSpace.Normal;
            textCol.Add(description);

            card.Add(textCol);

            // Multiplier badge — right-aligned
            var multiplier = new Label($"{relic.multiplier}x");
            multiplier.style.fontSize = 18f;
            multiplier.style.color = new StyleColor(new Color(0.2f, 0.4f, 0.8f));
            multiplier.style.unityFontStyleAndWeight = FontStyle.Bold;
            multiplier.style.marginLeft = 12f;
            multiplier.style.flexShrink = 0;
            card.Add(multiplier);

            container.Add(card);
        }
    }
    
    public void handleBagButtonClick()
    {
        RuntimeManager.PlayOneShot(openBagSFX);
        if (relicInventoryContainer.style.display == DisplayStyle.Flex)
        {
            relicInventoryContainer.style.display = DisplayStyle.None;
        }
        else
        {
            relicInventoryContainer.style.display = DisplayStyle.Flex;
            PopulateRelics();
        }
    }

    public void handleHelperButtonClick()
    {
        if(helperContainer.style.display == DisplayStyle.Flex)
        {
            helperContainer.style.display = DisplayStyle.None;
        }
        else
        {
            helperContainer.style.display = DisplayStyle.Flex;
        }
    }

    public float fadeDuration = 1f;
    public float elapsed = 0f;

    public IEnumerator FadeOut(Label target, float duration)
    {
        Color color = target.style.color.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            target.style.color = new StyleColor(new Color(color.r, color.g, color.b, alpha));
            yield return null; // wait one frame
        }

        target.style.color = new StyleColor(new Color(color.r, color.g, color.b, 0f)); // ensure fully invisible
    }

    IEnumerator FadeIn(Label target, float duration)
    {
        Color color = target.style.color.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            target.style.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        target.style.color = new Color(color.r, color.g, color.b, 1f);
    }

    public IEnumerator blockNumberTransition(Board board, BattleNPC enemy)
    {
        //handle fadeOut
        StartCoroutine(FadeOut(playerBlockText,fadeDuration));
        StartCoroutine(FadeOut(enemyBlockText,fadeDuration));
        yield return new WaitForSeconds(fadeDuration);
        board.player.ResetBlock();
        enemy.ResetBlock();
        yield return null;
        StartCoroutine(FadeIn(playerBlockText,fadeDuration));
        StartCoroutine(FadeIn(enemyBlockText,fadeDuration));

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
        bagButton = root.Q<Button>("bagButton");
        relicInventoryContainer = root.Q<VisualElement>("relicInventoryContainer");
        container = root.Q<VisualElement>("container");
        helperButton = root.Q<Button>("helperButton");
        helperContainer = root.Q<VisualElement>("helperContainer");
        relicList = player.RelicList;
        bagButton.clicked += handleBagButtonClick;
        helperButton.clicked += handleHelperButtonClick;
    }

    // Update is called once per frame
    void Update()
    {   
        //check HealthBar every tick
        UpdateHealthBar(player,enemyGenerator.CurrentEnemy);
        UpdateBlockText(player,enemyGenerator.CurrentEnemy);

        //handle relicList display
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(relicInventoryContainer.style.display == DisplayStyle.Flex)
            {
                relicInventoryContainer.style.display = DisplayStyle.None;
            }
            else
            {
                relicInventoryContainer.style.display = DisplayStyle.Flex;
                PopulateRelics();
            }
        }

        //handle helper display
        if (Input.GetKeyDown(KeyCode.H))
        {
            if(helperContainer.style.display == DisplayStyle.Flex)
            {
                helperContainer.style.display = DisplayStyle.None;
            }
            else
            {
                helperContainer.style.display = DisplayStyle.Flex;
            }
        }
    }
}
