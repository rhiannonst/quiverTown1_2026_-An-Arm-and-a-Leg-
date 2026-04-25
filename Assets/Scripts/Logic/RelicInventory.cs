using System.Collections.Generic;
using UnityEngine;

public class RelicInventory : MonoBehaviour
{
    private readonly List<RelicInstance> relics = new List<RelicInstance>();

    public IReadOnlyList<RelicInstance> Relics => relics;

    public void AddRelic(Relic relic)
    {
        if (relic == null)
        {
            return;
        }

        relics.Add(new RelicInstance(relic));
    }

    public void TriggerRelics(Relic.Trigger trigger)
    {
        TriggerRelics(trigger, Relic.AssociatedTileType.Any);
    }

    public void TriggerRelics(Relic.Trigger trigger, Relic.AssociatedTileType tileType)
    {
        foreach (RelicInstance relic in relics)
        {
            if (CanTrigger(relic, trigger, tileType))
            {
                ApplyRelic(relic);
            }
        }
    }

    public void ResetTurnState()
    {
        foreach (RelicInstance relic in relics)
        {
            relic.ResetTurnState();
        }
    }

    public void ResetCombatState()
    {
        foreach (RelicInstance relic in relics)
        {
            relic.ResetCombatState();
        }
    }

    private bool CanTrigger(RelicInstance relic, Relic.Trigger trigger, Relic.AssociatedTileType tileType)
    {
        return relic.Data.trigger == trigger
            && (relic.Data.associatedTileType == Relic.AssociatedTileType.Any
                || relic.Data.associatedTileType == tileType);
    }

    private void ApplyRelic(RelicInstance relic)
    {
        relic.MarkTriggered();

        switch (relic.Data.effectType)
        {
            case Relic.EffectType.GainBlock:
            case Relic.EffectType.DealDamage:
            case Relic.EffectType.Heal:
            case Relic.EffectType.ModifyDamage:
                Debug.Log($"Relic triggered: {relic.Data.name}");
                break;
        }
    }
}
