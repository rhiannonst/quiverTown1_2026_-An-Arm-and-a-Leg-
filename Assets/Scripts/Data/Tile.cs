using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile")]
public class Tile : ScriptableObject
{
    public enum TileType
    {
        Head,
        Arm,
        Leg,
        Torso,
        Spine,
        Heart
    }

    public enum Status
    {
        None,
        Poison,
        Reshuffle,
    }

    [Header("Identity")]
    public string tileName;
    public TileType tileType;
    public Status status;
    public Sprite sprite;

    [Header("Combat Values")]
    public float damage;
    public float heal;
    public float block;
}
