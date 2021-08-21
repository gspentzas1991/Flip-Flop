using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Tile[,] puzzle;
    [SerializeField] private TileGenerator tileGenerator;
    private bool searchForMatches;
    private Tile cursorTile;

    //might need refactoring
    private float rotationDirection;
    private bool isRotating;
    private Quaternion targetRotation;
    private float rotationSpeed = 1f;

    void Start()
    {
        InitializeSingleton();
        puzzle = tileGenerator.GeneratePuzzle();
        cursorTile = tileGenerator.GenerateCursorTile();

        searchForMatches = true;
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
        //Goes through the puzzleTiles, marking any matched tiles as isMatched
        if (searchForMatches)
        {
            foreach (var tile in puzzle)
            {
                tile.FindMatch();
            }
            searchForMatches = false;
        }
        //If we're not rotating, we're expecting an input
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                var targetTilePosition = DirectionCalculator.Calculate(cursorTile.tilePosition, Direction.Up, puzzleRotation.z);
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
                var targetTilePosition = DirectionCalculator.Calculate(cursorTile.tilePosition, Direction.Down, puzzleRotation.z);
                if (!IsPositionWithinPuzzle(targetTilePosition))
                {
                    return;
                }
                var targetTile = puzzle[targetTilePosition.x, targetTilePosition.y];
                MoveCursor(targetTile);
            }
            //Gets the rotation target based on input
            if (Input.GetKeyDown(KeyCode.Q))
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
            if (Mathf.Abs(angle) < 1e-3f)
            {
                isRotating = false;
                tileGenerator.transform.rotation = Quaternion.Euler(puzzleRotation);

                var targetTilePosition = DirectionCalculator.Calculate(cursorTile.tilePosition, Direction.Down, puzzleRotation.z);
                if (!IsPositionWithinPuzzle(targetTilePosition))
                {
                    return;
                }
                MoveCursor(puzzle[targetTilePosition.x,targetTilePosition.y]);
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
        var cursorStartingPosition = cursorTile.transform.position;
        cursorTile.transform.position = targetTile.transform.position;
        targetTile.transform.position = cursorStartingPosition;
        puzzle[cursorTile.tilePosition.x, cursorTile.tilePosition.y] = targetTile;
        puzzle[targetTile.tilePosition.x, targetTile.tilePosition.y] = cursorTile;
        var cursorTileStartingPuzzlePosition = cursorTile.tilePosition;
        cursorTile.tilePosition = targetTile.tilePosition;
        targetTile.tilePosition = cursorTileStartingPuzzlePosition;
        searchForMatches = true;
    }

    /// <summary>
    /// Checks if a position is within the puzzle dimensions
    /// </summary>
    private bool IsPositionWithinPuzzle(Vector2Int positionToCheck)
    {
        Debug.Log(positionToCheck);
        if (positionToCheck.x < 0 || positionToCheck.y < 0 || positionToCheck.x >= puzzle.GetLength(1) || positionToCheck.y >= puzzle.GetLength(0))
        {
            return false;
        }
        return true;
    }
}
