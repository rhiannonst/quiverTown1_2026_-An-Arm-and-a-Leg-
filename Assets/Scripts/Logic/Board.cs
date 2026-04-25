using System.Collections;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;

    public GameObject tilePrefab;
    public GameObject[] tilePrefabs;
    public GameObject destroyParticle;

    public GameObject[,] allTileInstances;
    public TileInstance currentTile;

    private GameObject[,] backgroundTiles;
    private FindMatches findMatches;

    

    void Start()
    {
        findMatches = FindAnyObjectByType<FindMatches>();
        backgroundTiles = new GameObject[width, height];
        allTileInstances = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);

                // Background tile
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + ", " + j + ")";
                backgroundTiles[i, j] = backgroundTile;

                // Pick a random tile that doesn't create a match
                int tileToUse = Random.Range(0, tilePrefabs.Length);
                int maxIterations = 0;
                while (MatchesAt(i, j, tilePrefabs[tileToUse]) && maxIterations < 100)
                {
                    tileToUse = Random.Range(0, tilePrefabs.Length);
                    maxIterations++;
                }

                GameObject tileInstance = Instantiate(tilePrefabs[tileToUse], tempPosition, Quaternion.identity);
                tileInstance.GetComponent<TileInstance>().row = j;
                tileInstance.GetComponent<TileInstance>().column = i;
                tileInstance.transform.parent = this.transform;
                tileInstance.name = "(" + i + ", " + j + ")";
                allTileInstances[i, j] = tileInstance;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject tilePrefabToCheck)
    {
        TileType tileType = tilePrefabToCheck.GetComponent<TileInstance>().tileData.Type;

        // Check horizontal (two to the left)
        if (column > 1)
        {
            if (GetTileType(allTileInstances[column - 1, row]) == tileType &&
                GetTileType(allTileInstances[column - 2, row]) == tileType)
                return true;
        }

        // Check vertical (two below)
        if (row > 1)
        {
            if (GetTileType(allTileInstances[column, row - 1]) == tileType &&
                GetTileType(allTileInstances[column, row - 2]) == tileType)
                return true;
        }

        return false;
    }

    private TileType GetTileType(GameObject tileInstance)
    {
        return tileInstance.GetComponent<TileInstance>().tileData.Type;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allTileInstances[column, row].GetComponent<TileInstance>().isMatched)
        {
            GameObject particle = Instantiate(destroyParticle,
                allTileInstances[column, row].transform.position,
                Quaternion.identity);
            Destroy(particle, .5f);

            Destroy(allTileInstances[column, row]);
            allTileInstances[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTileInstances[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        // For each column, collapse tiles down into empty spaces
        for (int i = 0; i < width; i++)
        {
            int nullCount = 0;
            for (int j = 0; j < height; j++)
            {
                if (allTileInstances[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    // Move this tile down by nullCount rows
                    int newRow = j - nullCount;
                    allTileInstances[i, j].GetComponent<TileInstance>().row = newRow;
                    allTileInstances[i, newRow] = allTileInstances[i, j];
                    allTileInstances[i, j] = null;
                }
            }
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTileInstances[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int tileToUse = Random.Range(0, tilePrefabs.Length);
                    GameObject tileInstance = Instantiate(tilePrefabs[tileToUse], tempPosition, Quaternion.identity);
                    tileInstance.GetComponent<TileInstance>().row = j;
                    tileInstance.GetComponent<TileInstance>().column = i;
                    tileInstance.transform.parent = this.transform;
                    allTileInstances[i, j] = tileInstance;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTileInstances[i, j] != null)
                {
                    if (allTileInstances[i, j].GetComponent<TileInstance>().isMatched)
                        return true;
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        // Detect matches on the newly filled board
        findMatches.FindAllMatches();
        yield return new WaitForSeconds(.3f);

        // Cascade: keep destroying and refilling while matches exist
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }

        findMatches.currentMatches.Clear();
        currentTile = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }
}