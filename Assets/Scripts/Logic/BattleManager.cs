using UnityEngine;
using System;
using System.Security.Cryptography;

class BattleManager : MonoBehaviour
{
    public event Action<float> OnTakeDamage;
    PlayerBehaviour playerBehaviour;

    public float damageAmount = 0; //this be how enemy gives it's damage amount.
    // event handler
    public void onDamageTaken()//float DamageAmount) //public event Action<float> OnTakeDamage
    {
        //thinking this is where it's the do the damage?
        playerBehaviour.OnTakeDamage?.Invoke(damageAmount);
    }

    public void RegisterPlayer(PlayerBehaviour pb) //the actual subscriber...
    {
        OnTakeDamage += pb.OnTakeDamage;
    }
    public void DeregisterPlayer(PlayerBehaviour pb) //the unsubscriber.
    {
        // Unsubscribe to prevent memory leaks
        OnTakeDamage -= pb.OnTakeDamage;
    }
    
    public void turn()
    {
        
    }
    public void Start()
    {
        // subscriber (catcher)
        //playerBehaviour.TakeDamage += onDamageTaken;
        
        //playerBehaviour.TakeDamage(onDamageTaken());
        //PlayerBehaviour.TakeDamage(playerBehaviour.OnTakeDamage());
        playerBehaviour.TakeDamage(damageAmount);
    }

    public void Update() //this is improper syntax psuedo code done by event mentor as example.
    {
        /* if (hits(player, enemy))
        {
            OnTakeDamage(enemy);
        } */
    }


    //private void OnEnable()
    //{
    //    // Subscribe to the damage event
    //    RegisterPlayer();
    //}

    //private void OnDisable()
    //{
    //    // Unsubscribe to prevent memory leaks
    //    UnRegisterPlayer();
    //}
}