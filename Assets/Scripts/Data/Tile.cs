using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile")]
public class Tile : ScriptableObject
{
    [Header("Identity")]
    public string Name;
    public TileType Type;
    public TileStatus Status;
    public Sprite _Sprite;

    [Header("Combat Values")]
    public float Damage;
    public float Heal;
    public float Block;
}
