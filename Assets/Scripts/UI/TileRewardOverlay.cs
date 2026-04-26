using UnityEngine;
using UnityEngine.EventSystems;

public class TileRewardOverlay : MonoBehaviour
{
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
