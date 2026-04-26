using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable Objects/NPC")]
public class NPC : ScriptableObject
{
    public string Name{get; private set;}

    public float MaxHealth{get; private set;}

    public EnemyMove[] MoveList{get; private set;}

    public float Block{get; private set;}
}
