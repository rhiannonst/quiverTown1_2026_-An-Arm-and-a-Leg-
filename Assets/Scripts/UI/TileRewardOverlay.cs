using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TileRewardOverlay : MonoBehaviour
{
    public GameObject bonusCard;
    public TMP_Text bonusNameLabel;
    public TMP_Text bonusDescriptionLabel;
    public TileRewardCard[] cards = new TileRewardCard[3];

    void Awake()
    {
        EnsureSingleEventSystem();
    }

    void Start()
    {
        GameObject[] options = TileRewardSession.CurrentOptions;
        if (options == null || options.Length == 0)
        {
            UnityEngine.Debug.LogWarning("Pick tile overlay opened without reward options.", this);
            return;
        }

        SetBonusRelic(TileRewardSession.BonusRelic);

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;

            if (i < options.Length)
            {
                cards[i].SetTile(options[i], TileRewardSession.Pick);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetBonusRelic(Relic relic)
    {
        bool hasRelic = relic != null;
        if (bonusCard != null)
        {
            bonusCard.SetActive(hasRelic);
        }

        if (bonusNameLabel != null)
        {
            bonusNameLabel.text = hasRelic ? $"Bonus: {relic.Name}" : string.Empty;
        }

        if (bonusDescriptionLabel != null)
        {
            bonusDescriptionLabel.text = hasRelic ? relic.description : string.Empty;
        }
    }

    private void EnsureSingleEventSystem()
    {
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>();
        if (eventSystems.Length <= 1) return;

        EventSystem eventSystemToKeep = eventSystems[0];
        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem.gameObject.scene != gameObject.scene)
            {
                eventSystemToKeep = eventSystem;
                break;
            }
        }

        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem != eventSystemToKeep)
            {
                Destroy(eventSystem.gameObject);
            }
        }
    }
}
