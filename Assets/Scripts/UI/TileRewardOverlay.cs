using UnityEngine;

public class TileRewardOverlay : MonoBehaviour
{
    public TileRewardCard[] cards = new TileRewardCard[3];

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
}
