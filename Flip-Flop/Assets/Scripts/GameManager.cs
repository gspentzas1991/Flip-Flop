using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Tile[,] puzzle;
    [SerializeField] private TileGenerator tileGenerator;
    private bool searchForMatches;

    //might need refactoring
    private float rotationDirection;
    private bool isRotating;
    private Quaternion targetRotation;

    void Start()
    {
        InitializeSingleton();
        puzzle = tileGenerator.GeneratePuzzle();
        tileGenerator.GenerateCursorTile();

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
        //Goes through the puzzleTiles, marking any matched tiles as isMatched
        if (searchForMatches)
        {
            foreach (var tile in puzzle)
            {
                tile.FindMatch();
            }
            searchForMatches = false;
        }
        if (!isRotating)
        {
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


        float angle = Quaternion.Angle(tileGenerator.transform.rotation, targetRotation);

        //Checks to see if the target rotation has been reached, and stops the rotation if so
        if (Mathf.Abs(angle) < 1e-3f)
        {
            isRotating = false;
        }
        else
        {
            //rotate the puzzle towards the rotation target
            var rotGoal = Quaternion.Euler(tileGenerator.transform.eulerAngles + new Vector3(0, 0, rotationDirection));
            tileGenerator.transform.rotation = Quaternion.Slerp(tileGenerator.transform.rotation, rotGoal, 1f * Time.deltaTime);
        }
    }
}
