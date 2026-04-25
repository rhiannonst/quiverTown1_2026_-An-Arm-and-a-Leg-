using UnityEngine;

[CreateAssetMenu(fileName = "Relic", menuName = "Scriptable Objects/Relic")]
public class Relic : ScriptableObject
{
    public enum Trigger
    {
        CombatStart,
        CombatEnd,
        TurnStart,
        TurnEnd,
        TileMatched,
        DamageDealt,
        DamageTaken
    }

    public enum EffectType
    {
        GainBlock,
        DealDamage,
        Heal,
        ModifyDamage
    }

    public enum AssociatedTileType
    {
        Any,
        Head,
        Arm,
        Leg,
        Torso,
        Spine,
        Heart
    }

    [Header("Display")]
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Behavior")]
    public Trigger trigger;
    public AssociatedTileType associatedTileType = AssociatedTileType.Any;
    public EffectType effectType;
    public float amount;
}
