using UnityEngine;

public class TileBehaviour
{
    public TileType Type;
    public TileStatus Status;
    public float Damage;
    public float Heal;
    public float Block;
    public string Name;
    public Sprite Sprite;

    Tile TileSO;

    public TileBehaviour(Tile tileSO)
    {
        Type = tileSO.Type;
        Status = tileSO.Status;
        Damage = tileSO.Damage;
        Heal = tileSO.Heal;
        Block = tileSO.Block;
        Name = tileSO.Name;
        Sprite = tileSO._Sprite;
    }
}