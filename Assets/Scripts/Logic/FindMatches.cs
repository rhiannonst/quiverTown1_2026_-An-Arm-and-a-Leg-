using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    public List<PlayerBehaviour.MatchResult> matchResults = new List<PlayerBehaviour.MatchResult>();
    private bool isFinding = false;

    void Start()
    {
        board = FindAnyObjectByType<Board>();
    }

    public void FindAllMatches()
    {
        if (!isFinding)
        {
            StartCoroutine(FindAllMatchesCo());
        }
    }

    private IEnumerator FindAllMatchesCo()
    {
        isFinding = true;
        yield return new WaitForSeconds(.2f / board.sequenceSpeed);

        // Clear stale flags before scanning — this method only sets true, never false,
        // so without this reset any previously-matched tile that survived would keep
        // MatchesOnBoard() returning true forever.
        for (int i = 0; i < board.width; i++)
            for (int j = 0; j < board.height; j++)
                if (board.allTileInstances[i, j] != null)
                    board.allTileInstances[i, j].GetComponent<TileInstance>().isMatched = false;

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentTile = board.allTileInstances[i, j];
                if (currentTile == null) continue;

                TileInstance currentTileInstance = currentTile.GetComponent<TileInstance>();

                // Check horizontal match (left-center-right)
                if (i > 0 && i < board.width - 1)
                {
                    GameObject leftTile = board.allTileInstances[i - 1, j];
                    GameObject rightTile = board.allTileInstances[i + 1, j];

                    if (leftTile != null && rightTile != null)
                    {
                        if (SameTileType(leftTile, currentTileInstance) &&
                            SameTileType(rightTile, currentTileInstance))
                        {
                            AddToListAndMatch(leftTile);
                            AddToListAndMatch(currentTile);
                            AddToListAndMatch(rightTile);
                        }
                    }
                }

                // Check vertical match (down-center-up)
                if (j > 0 && j < board.height - 1)
                {
                    GameObject downTile = board.allTileInstances[i, j - 1];
                    GameObject upTile = board.allTileInstances[i, j + 1];

                    if (downTile != null && upTile != null)
                    {
                        if (SameTileType(downTile, currentTileInstance) &&
                            SameTileType(upTile, currentTileInstance))
                        {
                            AddToListAndMatch(downTile);
                            AddToListAndMatch(currentTile);
                            AddToListAndMatch(upTile);
                        }
                    }
                }
            }
        }

        // feed match results into player
        matchResults.Clear();
        Dictionary<TileType, int> typeCounts = new Dictionary<TileType, int>();
        foreach (GameObject tile in currentMatches)
        {
            TileType t = tile.GetComponent<TileInstance>().tileData.Type;
            if (!typeCounts.ContainsKey(t)) typeCounts[t] = 0;
            typeCounts[t]++;
        }
        foreach (var kvp in typeCounts)
            matchResults.Add(new PlayerBehaviour.MatchResult(kvp.Key, kvp.Value));

        isFinding = false;
    }

    private bool SameTileType(GameObject tile, TileInstance otherTileInstance)
    {
        return tile.GetComponent<TileInstance>().tileData.Type == otherTileInstance.tileData.Type;
    }

    private void AddToListAndMatch(GameObject tile)
    {
        if (!currentMatches.Contains(tile))
        {
            currentMatches.Add(tile);
        }
        tile.GetComponent<TileInstance>().isMatched = true;
    }
}