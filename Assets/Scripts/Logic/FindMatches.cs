using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindAnyObjectByType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentTile = board.allTileInstances[i, j];
                if (currentTile == null) continue;

                TileInstance currentTileInstance = currentTile.GetComponent<TileInstance>();

                if (i > 0 && i < board.width - 1)
                {
                    GameObject leftTile = board.allTileInstances[i - 1, j];
                    GameObject rightTile = board.allTileInstances[i + 1, j];

                    if (leftTile != null && rightTile != null)
                    {
                        if (SameTileType(leftTile, currentTileInstance) && SameTileType(rightTile, currentTileInstance))
                        {
                            AddToListAndMatch(leftTile);
                            AddToListAndMatch(currentTile);
                            AddToListAndMatch(rightTile);
                        }
                    }
                }

                if (j > 0 && j < board.height - 1)
                {
                    GameObject upTile = board.allTileInstances[i, j + 1];
                    GameObject downTile = board.allTileInstances[i, j - 1];

                    if (upTile != null && downTile != null)
                    {
                        if (SameTileType(upTile, currentTileInstance) && SameTileType(downTile, currentTileInstance))
                        {
                            AddToListAndMatch(upTile);
                            AddToListAndMatch(currentTile);
                            AddToListAndMatch(downTile);
                        }
                    }
                }
            }
        }
    }

    private bool SameTileType(GameObject tile, TileInstance otherTileInstance)
    {
        return tile.GetComponent<TileInstance>().tileData.tileType == otherTileInstance.tileData.tileType;
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
