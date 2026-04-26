using System.Collections;
using UnityEngine;
using FMOD;
using FMODUnity;
using System.ComponentModel;

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
    public Vector2 boardSize = new Vector2(7f, 7f);
    [Tooltip("0–1 fill factor. 1 = tiles touch edge-to-edge, <1 = gap between tiles.")]
    public float gemScale = 1f;

    public GameObject tilePrefab;
    public GameObject[] tilePrefabs;
    public TileBag tileBag = new TileBag();

    [Tooltip("Base speed multiplier for all sequence delays. 1 = normal, 2 = twice as fast.")]
    public float sequenceSpeed = 1f;
    [Tooltip("Added to sequenceSpeed with each cascade chain.")]
    public float chainSpeedIncrement = 0.5f;

    public GameObject[,] allTileInstances;
    public TileInstance currentTile;

    private GameObject[,] backgroundTiles;
    private FindMatches findMatches;

    public BattleScheduler battleScheduler;
    public DamagePopup damagePopup;

    public TurnResult turnResult;
    [SerializeField] private EventReference MatchSuccessSound;
    [SerializeField] private EventReference BoardFillSound;

    public Player player;

    public Vector2 CellSize => new Vector2(boardSize.x / width, boardSize.y / height);

    public Vector3 GridOrigin => new Vector3(
        transform.position.x - boardSize.x / 2f + CellSize.x / 2f,
        transform.position.y - boardSize.y / 2f + CellSize.y / 2f,
        transform.position.z
    );

    // call this before destroying
    public TurnResult TallyMatches()
    {
        TurnResult result = default;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var go = allTileInstances[i, j];
                if (go != null && go.GetComponent<TileInstance>().isMatched)
                {
                    Tile tileData = go.GetComponent<TileInstance>().tileData;
                }
            }
        }
        return result;
    }

    
    void Start()
    {
        findMatches = FindAnyObjectByType<FindMatches>();
        if (!tileBag.IsInitialized)
        {
            tileBag.Initialize(tilePrefabs);
        }

        backgroundTiles = new GameObject[width, height];
        allTileInstances = new GameObject[width, height];
        SetUp();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void SpawnPopups(System.Collections.Generic.List<Player.MatchResult> results)
    {
        if (damagePopup == null) return;
        var turnResult = new TurnResult();
        foreach (var r in results)
        {
            turnResult.TotalDamage += r.totalDamage;
            turnResult.TotalBlock  += r.totalBlock;
            turnResult.TotalHeal   += r.totalHeal;
        }
        damagePopup.AddResult(turnResult);
    }

    public void AddTilesToBag(GameObject tilePrefabToAdd, int count = 1)
    {
        tileBag.AddTile(tilePrefabToAdd, count);
    }

    public void ClearTilesForReward()
    {
        StopAllCoroutines();
        currentState = GameState.wait;
        currentTile = null;
        ClearMatchState();
        ClearTileInstances();
    }

    public void RedrawFromCurrentDeckAvoidingMatches()
    {
        tileBag.Refill();
        RedrawTilesAvoidingMatches();
        currentState = GameState.move;
    }

    private void SetUp()
    {
        Vector3 origin = GridOrigin;
        Vector2 cell = CellSize;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 tempPosition = new Vector3(origin.x + i * cell.x, origin.y + j * cell.y + offSet, origin.z);

                // Background tile
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.localScale = new Vector3(cell.x, cell.y, backgroundTile.transform.localScale.z);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + ", " + j + ")";
                backgroundTiles[i, j] = backgroundTile;

                GameObject tilePrefabToUse = tileBag.DrawAvoiding(candidate => MatchesAt(i, j, candidate));
                if (tilePrefabToUse == null)
                {
                    UnityEngine.Debug.LogError("Board has no tile prefabs available to draw from.", this);
                    return;
                }

                GameObject tileInstance = Instantiate(tilePrefabToUse, tempPosition, Quaternion.identity);
                Vector3 prefabScale = tileInstance.transform.localScale;
                tileInstance.transform.localScale = new Vector3(cell.x * gemScale, cell.y * gemScale, prefabScale.z);
                tileInstance.GetComponent<TileInstance>().row = j;
                tileInstance.GetComponent<TileInstance>().column = i;
                tileInstance.transform.parent = this.transform;
                tileInstance.name = "(" + i + ", " + j + ")";
                allTileInstances[i, j] = tileInstance;
            }
        }
        LogBoard();
    }

    private void ClearMatchState()
    {
        if (findMatches != null)
        {
            findMatches.currentMatches.Clear();
            findMatches.matchResults.Clear();
        }
    }

    private void ClearTileInstances()
    {
        if (allTileInstances == null) return;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTileInstances[i, j] != null)
                {
                    TileInstance tileInstance = allTileInstances[i, j].GetComponent<TileInstance>();
                    if (tileInstance != null)
                    {
                        tileInstance.isMatched = true;
                    }

                    Destroy(allTileInstances[i, j]);
                    allTileInstances[i, j] = null;
                }
            }
        }
    }

    private void RedrawTilesAvoidingMatches()
    {
        RuntimeManager.PlayOneShot(BoardFillSound);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 cell = CellSize;
                Vector3 origin = GridOrigin;
                Vector3 tempPosition = new Vector3(origin.x + i * cell.x, origin.y + j * cell.y + offSet, origin.z);
                GameObject tilePrefabToUse = tileBag.DrawAvoiding(candidate => MatchesAt(i, j, candidate));
                if (tilePrefabToUse == null)
                {
                    UnityEngine.Debug.LogError("Board has no tile prefabs available to draw from.", this);
                    return;
                }

                GameObject tileInstance = Instantiate(tilePrefabToUse, tempPosition, Quaternion.identity);
                Vector3 prefabScale = tileInstance.transform.localScale;
                tileInstance.transform.localScale = new Vector3(cell.x * gemScale, cell.y * gemScale, prefabScale.z);
                tileInstance.GetComponent<TileInstance>().row = j;
                tileInstance.GetComponent<TileInstance>().column = i;
                tileInstance.transform.parent = transform;
                tileInstance.name = "(" + i + ", " + j + ")";
                allTileInstances[i, j] = tileInstance;
            }
        }
    }

    private void LogBoard()
    {
        System.Text.StringBuilder sb = new();
        for (int j = height - 1; j >= 0; j--)
        {
            System.Text.StringBuilder row = new();
            row.Append('[');
            for (int i = 0; i < width; i++)
            {
                string label = allTileInstances[i, j] != null
                    ? allTileInstances[i, j].GetComponent<TileInstance>().tileData.Type.ToString()
                    : "null";
                row.Append(label);
                if (i < width - 1) row.Append(", ");
            }
            row.Append(']');
            sb.AppendLine(row.ToString());
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
            Destroy(allTileInstances[column, row]);
            allTileInstances[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        player?.ReceiveMatchResults(findMatches.matchResults);
        SpawnPopups(findMatches.matchResults);

        RuntimeManager.PlayOneShot(MatchSuccessSound);

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
        findMatches.matchResults.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private void CollapseColumns()
    {
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
                    int newRow = j - nullCount;
                    allTileInstances[i, j].GetComponent<TileInstance>().row = newRow;
                    allTileInstances[i, newRow] = allTileInstances[i, j];
                    allTileInstances[i, j] = null;
                }
            }
        }
    }

    private IEnumerator DecreaseRowCo()
    {
        CollapseColumns();
        yield return new WaitForSeconds(.4f / sequenceSpeed);
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
                    Vector2 cell = CellSize;
                    Vector3 origin = GridOrigin;
                    Vector3 tempPosition = new Vector3(origin.x + i * cell.x, origin.y + j * cell.y + offSet, origin.z);
                    GameObject tilePrefabToUse = tileBag.Draw();
                    if (tilePrefabToUse == null)
                    {
                        UnityEngine.Debug.LogError("Board has no tile prefabs available to draw from.", this);
                        return;
                    }

                    GameObject tileInstance = Instantiate(tilePrefabToUse, tempPosition, Quaternion.identity);
                    Vector3 prefabScale = tileInstance.transform.localScale;
                    tileInstance.transform.localScale = new Vector3(cell.x * gemScale, cell.y * gemScale, prefabScale.z);
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
        float startSpeed = sequenceSpeed;
        float speed = sequenceSpeed;

        RefillBoard();
        yield return new WaitForSeconds(.5f / speed);

        // Detect matches on the newly filled board
        findMatches.FindAllMatches();
        yield return new WaitForSeconds(.25f / speed); // must exceed FindAllMatchesCo's internal .2f / speed

        // Cascade: handle destroy → collapse → refill → re-detect inline.
        // Never call DestroyMatches() here — it spawns new coroutines and
        // creates an exponentially growing parallel chain each iteration.
        while (MatchesOnBoard())
        {
            speed += chainSpeedIncrement;
            sequenceSpeed = speed; // keep in sync so FindMatches reads the updated value

            yield return new WaitForSeconds(.25f / speed);

            player?.ReceiveMatchResults(findMatches.matchResults);
            SpawnPopups(findMatches.matchResults);
            RuntimeManager.PlayOneShot(MatchSuccessSound);

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (allTileInstances[i, j] != null)
                        DestroyMatchesAt(i, j);
            findMatches.currentMatches.Clear();
            findMatches.matchResults.Clear();

            CollapseColumns();
            yield return new WaitForSeconds(.2f / speed);

            RefillBoard();
            yield return new WaitForSeconds(.25f / speed);

            findMatches.FindAllMatches();
            yield return new WaitForSeconds(.25f / speed); // must exceed FindAllMatchesCo's internal .2f / speed
        }

        sequenceSpeed = startSpeed;
        findMatches.currentMatches.Clear();
        currentTile = null;
        battleScheduler?.ResolveTurn();
        yield return new WaitForSeconds(.25f / sequenceSpeed);
        currentState = GameState.move;
    }
}
