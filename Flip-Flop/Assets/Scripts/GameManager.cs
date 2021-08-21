using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Tile[,] puzzle;
    [SerializeField] private TileGenerator tileGenerator;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image powerMeterImage;
    private Tile cursorTile;
    /// <summary>
    /// How much the score and power meter should increase after destroying a tile
    /// </summary>
    private float tileBreakValue=5f;
    private float score;
    private float powerMeter = 0f;
    private float maxPowerMeter = 100f;

    //might need refactoring
    private float rotationDirection;
    private bool isRotating;
    private Quaternion targetRotation;
    private float rotationSpeed = 1f;

    //need refactoring
    public bool tilesAreMoving;
    private List<Tile> movingTiles = new List<Tile>();

    void Start()
    {
        InitializeSingleton();
        puzzle = tileGenerator.GeneratePuzzle();
        cursorTile = tileGenerator.GenerateCursorTile();
    }

    /// <summary>
    /// Initializes the instance singleton
    /// </summary>
    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        //Gets the rotation of the puzzle
        var puzzleRotation = tileGenerator.transform.rotation.eulerAngles;
        puzzleRotation.z = Mathf.Round(tileGenerator.transform.rotation.eulerAngles.z);
        //If we're not rotating, we're expecting an input
        if (!isRotating)
        {
            //we don't allow input if tiles are moving
            if (tilesAreMoving)
            {
                return;
            }
            //PowerMove input
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                if (powerMeter == maxPowerMeter)
                {
                    UseBombPower();
                }
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                var targetTilePosition = DirectionCalculator.Calculate(cursorTile.position, Direction.Up, puzzleRotation.z);
                //if the movement would 
                if (!IsPositionWithinPuzzle(targetTilePosition))
                {
                    return;
                }
                var targetTile = puzzle[targetTilePosition.x, targetTilePosition.y];
                MoveCursor(targetTile);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                var targetTilePosition = DirectionCalculator.Calculate(cursorTile.position, Direction.Down, puzzleRotation.z);
                if (!IsPositionWithinPuzzle(targetTilePosition))
                {
                    return;
                }
                var targetTile = puzzle[targetTilePosition.x, targetTilePosition.y];
                MoveCursor(targetTile);
            }
            //Gets the rotation target based on input
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                rotationDirection = 90;
                isRotating = true;
                targetRotation = Quaternion.Euler(tileGenerator.transform.eulerAngles + new Vector3(0, 0, rotationDirection));
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                rotationDirection = -90;
                isRotating = true;
                targetRotation = Quaternion.Euler(tileGenerator.transform.eulerAngles + new Vector3(0, 0, rotationDirection));
            }
        }
        else
        {
            //calculates the angle between the generator's current rotation and targetRotation
            float angle = Quaternion.Angle(tileGenerator.transform.rotation, targetRotation);

            //Checks to see if the target rotation has been reached, and stops the rotation if so
            if (Mathf.Abs(angle) < 1)
            {
                isRotating = false;
                tileGenerator.transform.rotation = targetRotation;
                puzzleRotation = tileGenerator.transform.rotation.eulerAngles;
                puzzleRotation.z = Mathf.Round(puzzleRotation.z);
                //var targetTilePosition = DirectionCalculator.Calculate(cursorTile.position, Direction.Up, puzzleRotation.z);
                //if (!IsPositionWithinPuzzle(targetTilePosition))
                //{
                //    return;
                //}
                //MoveCursor(puzzle[targetTilePosition.x,targetTilePosition.y]);
            }
            else
            {
                //rotate the puzzle towards the rotation target
                var rotGoal = Quaternion.Euler(tileGenerator.transform.eulerAngles + new Vector3(0, 0, rotationDirection));
                tileGenerator.transform.rotation = Quaternion.Slerp(tileGenerator.transform.rotation, rotGoal, rotationSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Replaces the cursor with the target tile. After the movement the tiles should search for matches again
    /// </summary>
    private void MoveCursor(Tile targetTile)
    {
        tilesAreMoving = true;
        //swap the tiles in the puzzle
        puzzle[cursorTile.position.x, cursorTile.position.y] = targetTile;
        puzzle[targetTile.position.x, targetTile.position.y] = cursorTile;
        //update the tile position
        var cursorTileStartingPuzzlePosition = cursorTile.position;
        cursorTile.position = targetTile.position;
        targetTile.position = cursorTileStartingPuzzlePosition;
        //moves the tile gameobject
        movingTiles.Add(cursorTile);
        movingTiles.Add(targetTile);
        StartCoroutine(cursorTile.MoveGameObject(targetTile.transform.position));
        StartCoroutine(targetTile.MoveGameObject(cursorTile.transform.position));
    }

    /// <summary>
    /// Checks if a position is within the puzzle dimensions
    /// </summary>
    private bool IsPositionWithinPuzzle(Vector2Int positionToCheck)
    {
        if (positionToCheck.x < 0 || positionToCheck.y < 0 || positionToCheck.x >= puzzle.GetLength(1) || positionToCheck.y >= puzzle.GetLength(0))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Removes the tile from the movingTiles list and generates a new one in its place, and if all tiles finished moving, updates the 
    /// relevant flags
    /// </summary>
    public void TileFinishedMoving(Tile tile)
    {
        for (int i = 0; i < movingTiles.Count; i++)
        {
            if (movingTiles[i]==tile)
            {
                movingTiles.RemoveAt(i);
            }
        }
        if (movingTiles.Count == 0)
        {
            tilesAreMoving = false;
            SearchTilesForMatches();
        }
    }

    /// <summary>
    /// Iterates through the puzzle to find tile matches
    /// </summary>
    private void SearchTilesForMatches()
    {
        bool generatedNewTiles = false;
        foreach (var tile in puzzle)
        {
            tile.FindMatch();
        }
        //Destroy all matched tiles and generate new tiles in their positions
        foreach (var tile in puzzle)
        {
            if (tile.isMatched)
            {
                ReplaceTile(tile);
                generatedNewTiles = true;
                IncreaseScore(tileBreakValue);
                IncreasePowerMeter(tileBreakValue);
            }
        }
        //if new tiles where generated, then search again for matches
        if (generatedNewTiles)
        {
            SearchTilesForMatches();
        }
    }

    /// <summary>
    /// Increases the score fo
    /// </summary>
    private void IncreaseScore(float increaseValue)
    {
        score += increaseValue;
        scoreText.text = $"Score : {score}";
        UpdatePowerMeter();
    }

    private void IncreasePowerMeter(float increaseValue)
    {
        powerMeter += increaseValue;
        if (powerMeter > maxPowerMeter)
        {
            powerMeter = maxPowerMeter;
        }
    }

    /// <summary>
    /// Uses the bomb power, which destroys 5 tiles
    /// </summary>
    private void UseBombPower()
    {
        powerMeter = 0f;
        UpdatePowerMeter();
        List<Tile> tilesToBeBombed = new List<Tile>();
        while (tilesToBeBombed.Count < 5)
        {
            var tile = puzzle[Random.Range(0, puzzle.GetLength(1)), Random.Range(0, puzzle.GetLength(0))];
            if (tilesToBeBombed.Contains(tile) || tile == cursorTile)
            {
                continue;
            }
            tilesToBeBombed.Add(tile);
        }
        foreach (var tile in tilesToBeBombed) 
        {
            ReplaceTile(tile);
            IncreaseScore(tileBreakValue);
        }
        SearchTilesForMatches();
    }

    /// <summary>
    /// Destroys tile and creates a new one in its place
    /// </summary>
    private void ReplaceTile(Tile tile)
    {
        puzzle[tile.position.x, tile.position.y] = tileGenerator.GenerateRandomTile(tile.position, true);
        puzzle[tile.position.x, tile.position.y].transform.position = tile.transform.position;
        puzzle[tile.position.x, tile.position.y].transform.rotation = tile.transform.rotation;
        Destroy(tile.gameObject);
    }

    /// <summary>
    /// Updates the power meter UI
    /// </summary>
    private void UpdatePowerMeter()
    {
        powerMeterImage.fillAmount = powerMeter / 100;
    }
}
