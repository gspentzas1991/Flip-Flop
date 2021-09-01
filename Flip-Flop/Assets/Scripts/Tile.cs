using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Tile : MonoBehaviour
{
    public TileShape shape;
    /// <summary>
    /// The position of the Tile inside the puzzle
    /// </summary>
    public Vector2Int position;
    /// <summary>
    /// Shows if the tile is part of a match
    /// </summary>
    public bool isMatched;
    private bool isMoving;
    private float tileMoveSpeed = 6f;
    /// <summary>
    /// The ammount of seconds to delay rendering the tile, in order for the tile explosion to finish
    /// </summary>
    private float renderDelay = 0.2f;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(RenderAfterDelay());
    }

    private IEnumerator RenderAfterDelay()
    {
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(renderDelay);
        spriteRenderer.enabled = true;
    }

    private void Update()
    {
        //If the tile is matched and not moving, destroy it
        if (isMatched && !isMoving)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// We try to find if the Tile is the central match of tile match
    /// </summary>
    public void FindMatch()
    {
        var puzzle = GameManager.Instance.puzzle;
        //For a tile match we need at least one match on the left, and one match on the right of the tile
        //Horizontal match check
        if (position.x>0 && position.x< puzzle.GetLength(1)-1)
        {
            var leftTile = puzzle[position.x - 1, position.y];
            var rightTile = puzzle[position.x + 1, position.y];
            if (leftTile.shape == shape && rightTile.shape == shape)
            {
                leftTile.isMatched = true;
                rightTile.isMatched = true;
                isMatched = true;
            }
        }
        //Vertical match check
        if (position.y > 0 && position.y < puzzle.GetLength(0)-1)
        {
            var bottomTile = puzzle[position.x, position.y-1];
            var topTile = puzzle[position.x, position.y+1];
            if (bottomTile.shape == shape && topTile.shape == shape)
            {
                bottomTile.isMatched = true;
                topTile.isMatched = true;
                isMatched = true;
            }
        }
    }

    /// <summary>
    /// Moves the tile towards the position. Keeps repeating until the tile reaches the position
    /// </summary>
    public IEnumerator MoveGameObject(Vector3 newPosition)
    {

        if (gameObject.activeInHierarchy)
        {
            yield return null;
        }
        isMoving = true;
        transform.position = Vector3.MoveTowards(transform.position, newPosition,Time.deltaTime * tileMoveSpeed);
        yield return new WaitForEndOfFrame();
        if (transform.position != newPosition)
        {        
            StartCoroutine(MoveGameObject(newPosition));
        }
        else
        {
            isMoving = false;
            GameManager.Instance.TileFinishedMoving(this);
        }
    }
}
