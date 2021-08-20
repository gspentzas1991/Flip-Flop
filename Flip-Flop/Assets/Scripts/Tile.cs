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
    public Vector2Int tilePosition;
    /// <summary>
    /// Shows if the tile is part of a match
    /// </summary>
    public bool isMatched;

    private void Update()
    {
        FindMatch();
    }

    /// <summary>
    /// We try to find if the Tile is the central match of tile match
    /// </summary>
    private void FindMatch()
    {
        var puzzle = GameManager.Instance.puzzle;
        //For a tile match we need at least one match on the left, and one match on the right of the tile
        //Horizontal match check
        if (tilePosition.x>0 && tilePosition.x< puzzle.GetLength(1)-1)
        {
            var leftTile = puzzle[tilePosition.x - 1, tilePosition.y];
            var rightTile = puzzle[tilePosition.x + 1, tilePosition.y];
            if (leftTile.shape == shape && rightTile.shape == shape)
            {
                leftTile.isMatched = true;
                rightTile.isMatched = true;
                isMatched = true;
            }
        }
        //Vertical match check
        if (tilePosition.y > 0 && tilePosition.y < puzzle.GetLength(0)-1)
        {
            var bottomTile = puzzle[tilePosition.y - 1, tilePosition.y];
            var topTile = puzzle[tilePosition.y + 1, tilePosition.y];
            if (bottomTile.shape == shape && topTile.shape == shape)
            {
                bottomTile.isMatched = true;
                topTile.isMatched = true;
                isMatched = true;
            }
        }
    }
}
