using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject
{
    public float MaxHealth{get; private set;}
    public string Name{get; private set;}
    public List<Relic> RelicList = new List<Relic>();
    public float BaseBlock{get; private set;}
    
    //then a constructor for the data Layer
    public Player(float maxHealth, string name, float baseBlock){
        MaxHealth = maxHealth;
        Name = name;    
        BaseBlock = baseBlock;
    }
}
