using System;

public class TurnResult
{
    public float TotalDamage;
    public float TotalBlock;
    public float TotalHeal;
    public int   MatchCount;


    public void Add(TurnResult other)
    {
        TotalDamage += other.TotalDamage;
        TotalBlock  += other.TotalBlock;
        TotalHeal   += other.TotalHeal;
        MatchCount  += other.MatchCount;
    }

    public void Reset()
    {
        TotalDamage = 0f;
        TotalBlock  = 0f;
        TotalHeal   = 0f;
        MatchCount  = 0;
    }
}