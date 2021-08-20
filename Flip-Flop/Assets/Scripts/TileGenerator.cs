using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private int puzzleWidth = 8;
    [SerializeField] private int puzzleHeight = 8;
    [SerializeField] private Tile[] availableTiles;
    /// <summary>
    /// The empty space between tiles
    /// </summary>
    [SerializeField] private float tilePadding =1.5f;


    /// <summary>
    /// Generates an ammount of tiles, depending on the puzzle width and height
    /// </summary>
    public Tile[,] GeneratePuzzle()
    {
        Tile[,] puzzle = new Tile[puzzleWidth, puzzleHeight] ;
        for (int x=0; x< puzzleWidth; x++)
        {
            for(int y = 0; y< puzzleHeight; y++)
            {
                puzzle[x,y] = GenerateRandomTile(new Vector2Int(x , y));
            }
        }
        return puzzle;
    }

    /// <summary>
    /// Generates a random tile at the defined position
    /// </summary>
    private Tile GenerateRandomTile(Vector2Int tilePosition)
    {
        var randomTile = availableTiles[Random.Range(0,availableTiles.Length)];
        var generatedTile = Instantiate(randomTile, new Vector2(tilePosition.x * tilePadding,tilePosition.y * tilePadding), randomTile.transform.rotation);
        generatedTile.tilePosition = tilePosition;
        generatedTile.transform.parent = transform;
        return generatedTile;
    }
}
