using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShuffleButton : MonoBehaviour
{
    public Board board;
    public BattleScheduler battleScheduler;
    public Button button;
    public TMP_Text label;
    public string buttonText = "Shuffle";
    public int cooldownTurns = 5;

    private int lastUsedTurn = -9999;

    void Update()
    {
        if (board == null || battleScheduler == null) return;
        int remaining = TurnsRemaining();
        bool ready = remaining <= 0 && board.currentState == GameState.move;

        if (button != null)
            button.interactable = ready;

        if (label != null)
            label.text = remaining > 0 ? $"{buttonText} ({remaining} turns)" : buttonText;
    }

    public void OnShufflePressed()
    {
        if (board.currentState != GameState.move) return;
        if (TurnsRemaining() > 0) return;

        lastUsedTurn = battleScheduler.TotalTurns;
        ShuffleBoard();
    }

    public int TurnsRemaining()
    {
        int elapsed = battleScheduler.TotalTurns - lastUsedTurn;
        return Mathf.Max(0, cooldownTurns - elapsed);
    }

    private void ShuffleBoard()
    {
        // Collect all live tiles
        var tiles = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < board.width; i++)
            for (int j = 0; j < board.height; j++)
                if (board.allTileInstances[i, j] != null)
                    tiles.Add(board.allTileInstances[i, j]);

        // Fisher-Yates shuffle
        for (int i = tiles.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (tiles[i], tiles[rand]) = (tiles[rand], tiles[i]);
        }

        // Reassign grid positions — TileInstance.Update lerps each tile to its new spot
        int idx = 0;
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allTileInstances[i, j] != null)
                {
                    board.allTileInstances[i, j] = tiles[idx];
                    TileInstance ti = tiles[idx].GetComponent<TileInstance>();
                    ti.column = i;
                    ti.row = j;
                    idx++;
                }
            }
        }
    }
}
