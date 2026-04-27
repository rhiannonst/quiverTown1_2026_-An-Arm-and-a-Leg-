using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NukeButton : MonoBehaviour
{
    public Board board;
    public BattleScheduler battleScheduler;
    public Button button;
    public TMP_Text label;
    public string buttonText = "Nuke";
    public int cooldownTurns = 10;

    private int lastUsedTurn = int.MinValue;

    void Update()
    {
        int remaining = TurnsRemaining();
        bool ready = remaining <= 0 && board.currentState == GameState.move;

        if (button != null)
            button.interactable = ready;

        if (label != null)
            label.text = remaining > 0 ? $"{buttonText} ({remaining} turns)" : buttonText;
    }

    public void OnNukePressed()
    {
        if (board.currentState != GameState.move) return;
        if (TurnsRemaining() > 0) return;

        lastUsedTurn = battleScheduler.TotalTurns;
        board.NukeBoard();
    }

    public int TurnsRemaining()
    {
        int elapsed = battleScheduler.TotalTurns - lastUsedTurn;
        return Mathf.Max(0, cooldownTurns - elapsed);
    }
}
