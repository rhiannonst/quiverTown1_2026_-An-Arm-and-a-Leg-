public class EnemyMoveHandler
{
    EnemyMove moveSo;
    public string Name;
    public int CurrentTimer;
    public int MaxTimer;
    public float Damage;
    public float Block;

    public EnemyMoveHandler(EnemyMove moveSo)
    {
        Name = moveSo.Name;
        CurrentTimer = moveSo.MaxTimer;
        MaxTimer = moveSo.MaxTimer;
        Damage = moveSo.Damage;
        Block = moveSo.Block;
    }

    public void handleCountDown()
    {
        CurrentTimer -= 1;
    }
}