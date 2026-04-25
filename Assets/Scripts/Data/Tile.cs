using System.Dynamic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile")]
public class Tile : ScriptableObject
{
    public enum Type
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
        Poison,
        Reshuffle,
    }
    
    public string Name{get; private set;}
    public float Damage{get; private set;}
    public float Heal{get; private set;}
    public float Block{get; private set;}

    public Tile(string name, float damage, float heal, float block)
    {
        Name = name;
        Damage = damage;
        Heal = heal;
        Block = block;
    }
}
