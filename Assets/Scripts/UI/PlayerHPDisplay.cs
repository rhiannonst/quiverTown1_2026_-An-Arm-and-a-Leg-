using TMPro;
using UnityEngine;

public class PlayerHPDisplay : MonoBehaviour
{
    public Player player;
    public TMP_Text label;

    void Update()
    {
        if (player == null || label == null || this == null) return;
        label.text = $"{player.CurrentHealth}/{player.MaxHealth}";
    }
}
