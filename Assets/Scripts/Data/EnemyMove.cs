using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMove", menuName = "Scriptable Objects/EnemyMove")]
public class EnemyMove : ScriptableObject
{
    public string Name{get; private set;}
    
    public int MaxTimer{get; private set;}
    public int CurrentTimer{get; private set;}
    
    public float Damage{get; private set;}
    public float Block{get; private set;}
}
