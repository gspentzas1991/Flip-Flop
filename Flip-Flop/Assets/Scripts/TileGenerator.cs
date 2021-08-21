using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{

    [SerializeField] private Tile cursorPrefab;
    [SerializeField] private int puzzleWidth = 8;
    [SerializeField] private int puzzleHeight = 8;
    [SerializeField] private Tile[] availableTiles;
    /// <summary>
    /// The empty space between tiles
    /// </summary>
    [SerializeField] private float tilePadding =1.5f;
    private Tile[,] generatedPuzzle;


    /// <summary>
    /// Generates an ammount of tiles, depending on the puzzle width and height
    /// </summary>
    public Tile[,] GeneratePuzzle()
    {
        generatedPuzzle = new Tile[puzzleWidth, puzzleHeight] ;
        for (int x=0; x< puzzleWidth; x++)
        {
            for(int y = 0; y< puzzleHeight; y++)
            {
                generatedPuzzle[x,y] = GenerateRandomTile(new Vector2Int(x , y),false);
            }
        }
        return generatedPuzzle;
    }

    /// <summary>
    /// Generates a cursor object in a random position inside the puzzle
    /// </summary>
    public Tile GenerateCursorTile()
    {
        var puzzle = GameManager.Instance.puzzle;
        //Make a random tile, our cursor
        var randomX = Random.Range(0, puzzleWidth);
        var randomY = Random.Range(0, puzzleHeight);
        Destroy(puzzle[randomX, randomY].gameObject);
        puzzle[randomX, randomY] = Instantiate(cursorPrefab, new Vector2(randomX * tilePadding, randomY * tilePadding), cursorPrefab.transform.rotation);
        puzzle[randomX, randomY].tilePosition = new Vector2Int(randomX, randomY);
        puzzle[randomX, randomY].transform.parent = transform;
        return puzzle[randomX, randomY];
    }

    /// <summary>
    /// Generates a random tile at the defined position
    /// </summary>
    private Tile GenerateRandomTile(Vector2Int tilePosition, bool allowMatches)
    {
        var randomTile = availableTiles[Random.Range(0, availableTiles.Length)];
        //if we don't want to allow matches, we need to check the puzzle for matches with the new tile
        if (!allowMatches)
        {
            //we don't want the program to ever hang, so we allow only for a specific number of loops
            int loops = 0;
            //Checks that a match will not happen on the generation
            while (TileMatch(tilePosition,randomTile.shape) || loops++ >1000)
            {
                randomTile = availableTiles[Random.Range(0, availableTiles.Length)];
            }
        }

        var generatedTile = Instantiate(randomTile, new Vector2(tilePosition.x * tilePadding,tilePosition.y * tilePadding), randomTile.transform.rotation);
        generatedTile.tilePosition = tilePosition;
        generatedTile.transform.parent = transform;
        return generatedTile;
    }

    /// <summary>
    /// Checks if creating a tile of a specific shape in a specific position would create matches
    /// </summary>
    private bool TileMatch(Vector2Int tilePosition, TileShape tileShape)
    {
        //Checking middle tiles
        if (tilePosition.x>1 && tilePosition.y>1)
        {
            if (tileShape == generatedPuzzle[tilePosition.x - 1, tilePosition.y].shape && tileShape == generatedPuzzle[tilePosition.x - 2, tilePosition.y].shape)
            {
                return true;
            }
            if (tileShape == generatedPuzzle[tilePosition.x, tilePosition.y - 1].shape && tileShape == generatedPuzzle[tilePosition.x, tilePosition.y - 2].shape)
            {
                return true;
            }
        }
        //Checking edge tiles
        else
        {
            //bottom row check
            if (tilePosition.x>1)
            {
                if (tileShape == generatedPuzzle[tilePosition.x - 1, tilePosition.y].shape && tileShape == generatedPuzzle[tilePosition.x - 2, tilePosition.y].shape)
                {
                    return true;
                }
            }
            //left row check
            if (tilePosition.y > 1)
            {
                if (tileShape == generatedPuzzle[tilePosition.x, tilePosition.y-1].shape && tileShape == generatedPuzzle[tilePosition.x, tilePosition.y-2].shape)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
