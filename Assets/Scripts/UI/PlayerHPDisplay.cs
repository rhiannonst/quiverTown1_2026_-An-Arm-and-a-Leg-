using TMPro;
using UnityEngine;

public class PlayerHPDisplay : MonoBehaviour
{
    public Player player;
    public TMP_Text label;

    void Update()
    {
        if (player == null || label == null) return;
        label.text = $"Player HP: {player.CurrentHealth}/{player.MaxHealth}";
    }
}
