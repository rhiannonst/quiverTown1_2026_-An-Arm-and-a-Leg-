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

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    private FindMatches findMatches;
    private Board board;
    private TileInstance otherTile;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    void Start()
    {
        board = FindAnyObjectByType<Board>();
        findMatches = FindAnyObjectByType<FindMatches>();
    }

    void Update()
    {
        targetX = column;
        targetY = row;

        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allTileInstances[column, row] != gameObject)
            {
                board.allTileInstances[column, row] = gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allTileInstances[column, row] != gameObject)
            {
                board.allTileInstances[column, row] = gameObject;
            }
            findMatches.FindAllMatches();
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
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        float dx = finalTouchPosition.x - firstTouchPosition.x;
        float dy = finalTouchPosition.y - firstTouchPosition.y;

        if (Mathf.Abs(dy) > swipeResist || Mathf.Abs(dx) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(dy, dx) * 180 / Mathf.PI;
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

        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            otherTile = board.allTileInstances[column + 1, row].GetComponent<TileInstance>();
            otherTile.column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            otherTile = board.allTileInstances[column, row + 1].GetComponent<TileInstance>();
            otherTile.row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            otherTile = board.allTileInstances[column - 1, row].GetComponent<TileInstance>();
            otherTile.column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
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

        if (otherTile != null)
        {
            if (!isMatched && !otherTile.isMatched)
            {
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
                board.DestroyMatches();
            }
        }
    }
}
