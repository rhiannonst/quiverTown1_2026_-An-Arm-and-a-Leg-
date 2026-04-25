using UnityEngine;

[CreateAssetMenu(fileName = "Relic", menuName = "Scriptable Objects/Relic")]
public class Relic : ScriptableObject
{
    public float Name{get; private set;}
    public float Effect{get; private set;}
}
