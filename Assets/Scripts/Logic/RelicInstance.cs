public class RelicInstance
{
    public Relic Data { get; }
    public int TriggerCount { get; private set; }
    public int Counter { get; private set; }
    public bool HasTriggeredThisTurn { get; private set; }
    public bool HasTriggeredThisCombat { get; private set; }

    public RelicInstance(Relic data)
    {
        Data = data;
    }

    public bool HasTriggered()
    {
        return TriggerCount > 0;
    }

    public void MarkTriggered()
    {
        TriggerCount++;
        HasTriggeredThisTurn = true;
        HasTriggeredThisCombat = true;
    }

    public void IncrementCounter(int amount = 1)
    {
        Counter += amount;
    }

    public bool CounterAtLeast(int amount)
    {
        return Counter >= amount;
    }

    public void ResetCounter()
    {
        Counter = 0;
    }

    public void ResetTurnState()
    {
        HasTriggeredThisTurn = false;
    }

    public void ResetCombatState()
    {
        HasTriggeredThisCombat = false;
        HasTriggeredThisTurn = false;
    }
}
