using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject
{
    public float MaxHealth{get; private set;}
    public string Name{get; private set;}
    public List<Relic> RelicList = new List<Relic>();
    public float Block{get; private set;}
}
