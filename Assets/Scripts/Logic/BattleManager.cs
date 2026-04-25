using UnityEngine;
using System;

class BattleManager : MonoBehaviour
{
    events OnTakeDamage;
    PlayerBehaviour playerBehaviour = new PlayerBehaviour();

    // event handler
    public void onDamageTaken(float DamageAmount) //public event Action<float> OnTakeDamage
    {
        //thinking this is where it's the do the damage?
        //playerBehaviour.?
    }



    public void RegisterPlayer(PlayerBehaviour pb)
    {
        OnTakeDamage += pb.OnTakeDamage;
    }
    public void DeregisterPlayer(PlayerBehavior pb)
    {
        // Unsubscribe to prevent memory leaks
        OnTakeDamage -= pb.OnTakeDamage;
    }
    public void Start()
    {
        // subscriber (catcher)
        playerBehaviour.TakeDamage += onDamageTaken;
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