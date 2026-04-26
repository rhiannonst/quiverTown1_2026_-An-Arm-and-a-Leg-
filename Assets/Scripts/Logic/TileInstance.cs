using System.Collections;
using UnityEngine;

public class TileInstance : MonoBehaviour
{
    [Header("Tile Data")]
    public Tile tileData;

    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    [Header("Input")]
    public float dragResist = 1f;

    private FindMatches findMatches;
    private Board board;
    private TileInstance otherTile;
    private Vector2 firstClickPosition;
    private Vector2 finalClickPosition;
    private Vector2 tempPosition;
    private float dragAngle = 0;

    void Start()
    {
        board = FindAnyObjectByType<Board>();
        findMatches = FindAnyObjectByType<FindMatches>();
    }

    void Update()
    {
        targetX = column;
        targetY = row;

        // Lerp horizontal
        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            // Guard: matched tiles are about to be destroyed — don't overwrite the null
            // that DestroyMatchesAt just set, since Unity defers Destroy() to end-of-frame.
            if (!isMatched && board.allTileInstances[column, row] != gameObject)
            {
                board.allTileInstances[column, row] = gameObject;
            }
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        // Lerp vertical
        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (!isMatched && board.allTileInstances[column, row] != gameObject)
            {
                board.allTileInstances[column, row] = gameObject;
            }
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        float dx = finalClickPosition.x - firstClickPosition.x;
        float dy = finalClickPosition.y - firstClickPosition.y;

        if (Mathf.Abs(dy) > dragResist || Mathf.Abs(dx) > dragResist)
        {
            dragAngle = Mathf.Atan2(dy, dx) * 180 / Mathf.PI;
            if (MovePieces())
            {
                board.currentState = GameState.wait;
                board.currentTile = this;
                StartCoroutine(CheckMoveCo());
            }
            else
            {
                board.currentState = GameState.move;
            }
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    bool MovePieces()
    {
        previousRow = row;
        previousColumn = column;
        otherTile = null;

        // Right
        if (dragAngle > -45 && dragAngle <= 45 && column < board.width - 1)
        {
            otherTile = board.allTileInstances[column + 1, row].GetComponent<TileInstance>();
            otherTile.column -= 1;
            column += 1;
        }
        // Up
        else if (dragAngle > 45 && dragAngle <= 135 && row < board.height - 1)
        {
            otherTile = board.allTileInstances[column, row + 1].GetComponent<TileInstance>();
            otherTile.row -= 1;
            row += 1;
        }
        // Left
        else if ((dragAngle > 135 || dragAngle <= -135) && column > 0)
        {
            otherTile = board.allTileInstances[column - 1, row].GetComponent<TileInstance>();
            otherTile.column += 1;
            column -= 1;
        }
        // Down
        else if (dragAngle < -45 && dragAngle >= -135 && row > 0)
        {
            otherTile = board.allTileInstances[column, row - 1].GetComponent<TileInstance>();
            otherTile.row += 1;
            row -= 1;
        }

        return otherTile != null;
    }

    private IEnumerator CheckMoveCo()
    {
        if (otherTile == null)
        {
            board.currentTile = null;
            board.currentState = GameState.move;
            yield break;
        }

        yield return new WaitForSeconds(.5f);

        // Run match detection after tiles have settled
        findMatches.FindAllMatches();
        yield return new WaitForSeconds(.3f);

        if (otherTile != null)
        {
            if (!isMatched && !otherTile.isMatched)
            {
                // No match — swap back
                otherTile.row = row;
                otherTile.column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentTile = null;
                board.currentState = GameState.move;
            }
            else
            {
                // Match found — destroy
                board.DestroyMatches();
            }
        }
    }
}