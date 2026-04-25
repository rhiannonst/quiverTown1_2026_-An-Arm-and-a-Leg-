using UnityEngine;

[CreateAssetMenu(fileName = "Relic", menuName = "Scriptable Objects/Relic")]
public class Relic : ScriptableObject
{
    [Header("Display")]
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Damage Modifier")]
    public TileType associatedTileType = TileType.Any;
    public float multiplier = 0f;
    public float flatAdd = 0f;
}
