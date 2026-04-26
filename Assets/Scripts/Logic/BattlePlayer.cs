using UnityEngine;
using System;
using System.Collections.Generic; // Required for List

using System.Security.Cryptography.X509Certificates;

public class BattlePlayer : MonoBehaviour
{
    //public event Action TakeDamage;
    public Player CurrentPlayer{get; private set;}
    // This is the event others will call
    public Action<float> OnTakeDamage;

    public float CurrentHealth;
    public string Name;
    public List<Relic> RelicList;
    public float CurrentBlock;
     
    //then a constructor for the data Layer
    public BattlePlayer(Player playerSO){

        CurrentHealth = playerSO.MaxHealth;
        Name = playerSO.Name;    
        RelicList = new List<Relic>();
        CurrentBlock = playerSO.Block;
    }

    public void TakeDamage(float damage)
    {
        // case 1: block > damage
        if (CurrentBlock > 0 && CurrentBlock > damage)
            {
                CurrentBlock -= damage;
            }
        else // case 2: block is < damage 
        {
            damage -= CurrentBlock;
            CurrentBlock = 0;
            if (damage > 0)
            {
                CurrentHealth = CurrentHealth-damage;
            }
        }
    }

    public void AddBlock(float blockValue)
    {
        CurrentBlock += blockValue;
    }

    public void ResetBlock()
    {
        CurrentBlock = 0;
    }

    public void handleHeal(float healAmt)
    {
        CurrentHealth = Mathf.Min(CurrentHealth+healAmt,CurrentPlayer.MaxHealth);
    }

    public void handleDeath()
    {
        Debug.Log("You Died");
    }

    /*Most of the time these events take on the form of public event Action<[classtype], bool> [PropertyName]Changed; 
    or public event Action SomethingHappened;. 
    In these cases, there are two benefits. 
    First, I get a type for the issuing class. If MyClass declares and is the only class firing the event, I get an explicit instance of MyClass to work with in the event handler. 
    Secondly, for simple events such as property change events, the meaning of the parameters is obvious and stated in the name of the event handler and I don't have to create a myriad of classes for these kinds of events.*/
}