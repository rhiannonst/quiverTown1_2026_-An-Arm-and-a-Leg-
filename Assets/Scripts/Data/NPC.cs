using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable Objects/NPC")]
public class NPC : ScriptableObject
{
    [Header("Identity")]
    public string Name;
    public Sprite Portrait;

    [Header("Stats")]
    public float MaxHealth = 30f;
    public float Block = 0f;

    [Header("Attack")]
    public int MinAttack = 1;
    public int MaxAttack = 4;
    public int MinHeal = 1;
    public int MaxHeal = 3;
}
