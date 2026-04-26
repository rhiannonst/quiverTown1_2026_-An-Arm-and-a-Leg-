using UnityEngine;
using System;
using System.Collections.Generic; // Required for List

using System.Security.Cryptography.X509Certificates;

public class PlayerBehaviour : MonoBehaviour
{
    private Player playerSO;
    //public event Action TakeDamage;

    // This is the event others will call
    public Action<float> OnTakeDamage;

    public float MaxHealth;
    public float CurrentHealth;
    public string Name;
    public List<Relic> RelicList;
    int BaseBlock; //dunno what this is meant to be... so it is a int.
     
    //then a constructor for the data Layer
    public PlayerBehaviour(){
        MaxHealth = playerSO.MaxHealth;
        CurrentHealth = playerSO.MaxHealth;
        Name = playerSO.name;    
        RelicList = new List<Relic>();
        ComboHistory = new List<ComboAction>();
        BaseBlock = 0;//baseBlock;
    }


    // public event Action<PlayerBehaviour> OnTakeDamage(Player){
        
    // }
    public struct ComboAction
    {
        public TileType TileType;
        public float Value;

        public ComboAction(TileType tileType, float value)
        {
            TileType = tileType;
            Value = value;
        }
    }

    public List<ComboAction> ComboHistory;

    public void TakeDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage. Health is now {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            // Handle death
            Debug.Log($"Player took {damageAmount} damage. They have now died.");
        }
    }

    // public float HandletakeDamage(PlayerBehaviour player) //public event Action<float> OnTakeDamage
    // {
    //     // publisher
    //    TakeDamage.Invoke();
    //    return 0;
    // }

    public void HandleTakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }

    // internal void OnTakeDamage()
    // {
    //     throw new NotImplementedException();
    // }

    /*Most of the time these events take on the form of public event Action<[classtype], bool> [PropertyName]Changed; 
    or public event Action SomethingHappened;. 
    In these cases, there are two benefits. 
    First, I get a type for the issuing class. If MyClass declares and is the only class firing the event, I get an explicit instance of MyClass to work with in the event handler. 
    Secondly, for simple events such as property change events, the meaning of the parameters is obvious and stated in the name of the event handler and I don't have to create a myriad of classes for these kinds of events.*/
}