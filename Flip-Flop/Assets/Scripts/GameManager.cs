using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Tile[,] puzzle;
    [SerializeField] private TileGenerator tileGenerator;

    void Start()
    {
        InitializeSingleton();

        puzzle = tileGenerator.GeneratePuzzle();
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
        
    }
}
