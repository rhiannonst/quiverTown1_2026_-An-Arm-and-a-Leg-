

class BattleManager : MonoBehaviour
{
    events OnTakeDamage;
    PlayerBehaviour playerBehaviour = new PlayerBehaviour();

    // event handler
    public void onDamageTaken(float DamageAmount)
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

    public void Update() //this is improper syntax psuedo code done by event mentor as example.
    {
        if (hits(player, enemy))
        {
            OnTakeDamage(enemy);
        }
    }

    public void Start()
    {
        // subscriber (catcher)
        playerBehaviour.TakeDamage += onDamageTaken;
    }

    private void OnEnable()
    {
        // Subscribe to the damage event
        RegisterPlayer();
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        UnRegisterPlayer();
    }
}