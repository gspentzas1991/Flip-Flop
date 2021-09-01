using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Tile[,] puzzle;
    [SerializeField] private TileGenerator tileGenerator;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text shiftTimerText;
    [SerializeField] private Text shiftTargetText;
    [SerializeField] private Text highscoreText;
    [SerializeField] private Image powerMeterImage;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject normalLossScreen;
    [SerializeField] private GameObject freeplayLossScreen;
    [SerializeField] private Text freeplayLossScreenText;
    //References to the workers Q and E
    [SerializeField] private Worker QWorker;
    [SerializeField] private Worker EWorker;
    [SerializeField] private GameObject tileExplosion;
    private AudioSource audioSource;
    [SerializeField] private AudioClip matchClip;
    [SerializeField] private AudioClip cursorMoveClip;
    [SerializeField] private AudioClip explosionClip;
    //Dear god refactor
    [SerializeField] private Text QContinueDialogueText;
    [SerializeField] private Text EContinueDialogueText;
    [SerializeField] private Text VictoryContinueText;
    [SerializeField] private Text LossContinueText;
    private Tile cursorTile;
    /// <summary>
    /// How much the score and power meter should increase after destroying a tile
    /// </summary>
    private float tileBreakValue=5f;
    private float score;
    private float powerMeter = 0f;
    private float maxPowerMeter = 100f;
    public bool isInDialogue = true;
    private bool wonGame = false;
    private bool searchingForMatches = false;
    private bool runOutOfTime = false;
    /// <summary>
    /// The delay after which the manager will search for tile matches again
    /// </summary>
    // We use this in order for newly formed matches to appear before they get destroyed
    private float searchmatchDelay = 0.5f;

    //might need refactoring
    private float rotationDirection;
    private bool isRotating;
    private Quaternion targetRotation;
    private float rotationSpeed = 2f;

    //need refactoring
    public bool tilesAreMoving;
    private List<Tile> movingTiles = new List<Tile>();

    //might need refactoring
    private List<int> shiftTimeLimits = new List<int>() {120, 100, 100, 60 };
    private int currentShiftTime;
    private List<int> shiftTargets = new List<int>() { 50, 60, 70, 100};
    private int freePlayStartingTimer = 60;
    public int currentShift = 0;
    GameSettingsManager gameSettingsManager;
    //The ui screen that appears on loss
    private GameObject LossScreen;

    private float highScore;

    //definately refactor
    private bool turnRight;
    private bool turnLeft;

    private Gyroscope gyro;
    //The amount of gyro movement we need to register a turn
    private float gyroThreshold = 3f;
    //Holds the gyro value of the previous frame
    private float previousGyroValue;

    void Start()
    {
        #if UNITY_ANDROID
            QContinueDialogueText.text = "Tap To Continue";
            EContinueDialogueText.text = "Tap To Continue";
            VictoryContinueText.text = "Tap to return to the main menu";
            LossContinueText.text = "You run out of time\nTap to try again";
        #else
            QContinueDialogueText.text = "Press Space To Continue";
            EContinueDialogueText.text = "Press Space To Continue";
            VictoryContinueText.text = "Press space to return to the main menu";
            LossContinueText.text = "You run out of time\Press space to try again";
        #endif
        //try to set the fps to 60
        Application.targetFrameRate = 60;

        gyro = Input.gyro;
        if (gyro.enabled == false)
        {
            gyro.enabled = true;
        }
        previousGyroValue = gyro.rotationRateUnbiased.z;


        gameSettingsManager = GameObject.FindGameObjectWithTag("GameSettings").GetComponent<GameSettingsManager>();
        InitializeSingleton();
        puzzle = tileGenerator.GeneratePuzzle();
        cursorTile = tileGenerator.GenerateCursorTile();
        audioSource = GetComponent<AudioSource>();
        if (gameSettingsManager.gameMode == GameMode.NormalGame)
        {
            currentShiftTime = shiftTimeLimits[0];
            isInDialogue = true;
            LossScreen = normalLossScreen;
        }
        else if (gameSettingsManager.gameMode == GameMode.FreePlay)
        {
            highScore = DataManager.GetHighScore();
            shiftTargetText.gameObject.SetActive(false);
            highscoreText.gameObject.SetActive(true);
            UpdateHighscoreUI();
            LossScreen = freeplayLossScreen;
            isInDialogue = false;
            currentShiftTime = freePlayStartingTimer;
        }
       
        UpdateShiftTargetUI();
        UpdateShiftTimerUI();
        UpdateScoreUI();
        StartCoroutine(DecreaseShiftTimer());
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

        //Every frame we see if there was a rotation, based on the gyro rotation value of the previous frame
        if (gyro.rotationRateUnbiased.z >  gyroThreshold)
        {
            turnLeft = true;
        }
        else if (gyro.rotationRateUnbiased.z < - gyroThreshold)
        {
            turnRight = true;
        }
        //previousGyroValue = gyro.rotationRateUnbiased.z;

        if (wonGame)
        {
            //return to main menu
            if(Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)
            {
                SceneManager.LoadScene(0);
            }
        }
        if (runOutOfTime)
        {
            //return to main menu
            if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)
            {
                runOutOfTime = false;
                LossScreen.SetActive(false);
                RestartShift();
            }
        }
        if (score >= shiftTargets[currentShift] && !searchingForMatches)
        {
            StartNextShift();
        }
        //Gets the rotation of the puzzle
        var puzzleRotation = tileGenerator.transform.rotation.eulerAngles;
        puzzleRotation.z = Mathf.Round(tileGenerator.transform.rotation.eulerAngles.z);
        //in the third shift, E will be missing
        if (currentShift == 2)
        {
            EWorker.gameObject.SetActive(false);
        }
        else
        {
            EWorker.gameObject.SetActive(true);
        }
        //If we're not rotating, we're expecting an input
        if (!isRotating)
        {
            //we don't allow input in the following conditions
            if (tilesAreMoving || isInDialogue || wonGame || searchingForMatches || runOutOfTime)
            {
                return;
            }

            //PowerMove input
            if (Input.GetKeyDown(KeyCode.F)) 
            {
                if (powerMeter == maxPowerMeter)
                {
                    UseBombPower();
                }
            }
            else if (Input.GetKeyDown(KeyCode.W) || SwipeDetection.swipedUp)
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
            else if (Input.GetKeyDown(KeyCode.S) || SwipeDetection.swipedDown)
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
            else if (Input.GetKeyDown(KeyCode.Q)  || turnLeft)
            {
                QWorker.StartWorking();
                rotationDirection = -90;
                isRotating = true;
                turnLeft = false;
                targetRotation = Quaternion.Euler(tileGenerator.transform.eulerAngles + new Vector3(0, 0, rotationDirection));
            }
            else if (Input.GetKeyDown(KeyCode.E) && currentShift != 2 || turnRight)
            {
                EWorker.StartWorking();
                rotationDirection = 90;
                isRotating = true;
                turnRight = false;
                targetRotation = Quaternion.Euler(tileGenerator.transform.eulerAngles + new Vector3(0, 0, rotationDirection));
            }
        }
        else
        {
            //calculates the angle between the generator's current rotation and targetRotation
            float angle = Quaternion.Angle(tileGenerator.transform.rotation, targetRotation);

            //Checks to see if the target rotation has been reached, and stops the rotation if so
            if (Mathf.Abs(angle) < 2)
            {
                isRotating = false;
                turnLeft = false;
                turnRight = false;
                if (rotationDirection == -90)
                {
                    QWorker.StopWorking();
                }
                else if (rotationDirection == 90)
                {
                    EWorker.StopWorking();
                }
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
        audioSource.clip = cursorMoveClip;
        audioSource.Play();
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
            StartCoroutine(SearchTilesForMatches());
        }
    }

    /// <summary>
    /// Iterates through the puzzle to find tile matches
    /// </summary>
    private IEnumerator SearchTilesForMatches()
    {
        bool generatedNewTiles = false;
        searchingForMatches = true;
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
                IncreasePowerMeter(tileBreakValue*3);
                if (gameSettingsManager.gameMode == GameMode.FreePlay)
                {
                    currentShiftTime += 1;
                    UpdateShiftTimerUI();
                }
            }
        }
        //if new tiles where generated, then wait for a second and then search again for matches
        if (generatedNewTiles)
        {
            yield return new WaitForSeconds(searchmatchDelay);
            StartCoroutine(SearchTilesForMatches());
        }
        else
        {
            searchingForMatches = false;
        }
    }

    /// <summary>
    /// Increases the score UI
    /// </summary>
    private void IncreaseScore(float increaseValue)
    {
        score += increaseValue;
        UpdateScoreUI();
        UpdatePowerMeterUI();
    }

    /// <summary>
    /// Increases the currentShift, and puts us back in dialogueMode
    /// </summary>
    private void StartNextShift()
    {
        //We don't start new shifts in the freeplay mode
        if (gameSettingsManager.gameMode == GameMode.FreePlay)
        {
            return;
        }
        if (currentShift>=shiftTargets.Count-1)
        {
            WinTheGame();
        }
        else
        {
            currentShift++;
            currentShiftTime = shiftTimeLimits[currentShift];
            score = 0;
            powerMeter = 0f;
            UpdateScoreUI();
            UpdateShiftTimerUI();
            UpdateShiftTargetUI();
            UpdatePowerMeterUI();
            isInDialogue = true;
        }
    }

    /// <summary>
    /// Increases the power meter UI
    /// </summary>
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
        UpdatePowerMeterUI();
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
        //in free mode, when a bomb is used, we give 3 seconds back (instead of one second per tile)
        if (gameSettingsManager.gameMode == GameMode.FreePlay)
        {
            currentShiftTime += 3;
            UpdateShiftTimerUI();
        }
        audioSource.clip = explosionClip;
        audioSource.Play();
        StartCoroutine(SearchTilesForMatches());
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
        Instantiate(tileExplosion, tile.transform.position, tileExplosion.transform.rotation);
        audioSource.clip = matchClip;
        audioSource.Play();
    }

    /// <summary>
    /// Updates the power meter UI
    /// </summary>
    private void UpdatePowerMeterUI()
    {
        powerMeterImage.fillAmount = powerMeter / 100;
    }

    /// <summary>
    /// Deceases the shift timer every second, unless we're in dialogue
    /// </summary>
    /// <returns></returns>
    private IEnumerator DecreaseShiftTimer()
    {
        if (!isInDialogue && !wonGame)
        {
            currentShiftTime--;
            UpdateShiftTimerUI();
        }
        //we only lose the game in the normal game mode
        if (currentShiftTime<=0)
        {
            currentShiftTime = 0;
            UpdateShiftTimerUI();
            GameOver();
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(DecreaseShiftTimer());
    }

    private void WinTheGame()
    {
        wonGame = true;
        victoryScreen.SetActive(true);
    }

    private void GameOver()
    {
        runOutOfTime = true;
        if (gameSettingsManager.gameMode == GameMode.FreePlay)
        {
            UpdateLossScreenText();
            if (score > highScore)
            {
                highScore = score;
                DataManager.SetHighScore(highScore);
                UpdateHighscoreUI();
            }
        }
        LossScreen.SetActive(true);
    }

    /// <summary>
    /// Increases the score UI
    /// </summary>
    private void UpdateShiftTimerUI()
    {
        shiftTimerText.text = $"Shift Ends in {currentShiftTime}";
    }

    /// <summary>
    /// Updates the score on the loss screen text
    /// </summary>
    private void UpdateLossScreenText()
    {
        #if UNITY_ANDROID
            freeplayLossScreenText.text = $"You run out of time\nYour score was: {score}!\nTap to try again!";
        #else
            freeplayLossScreenText.text = $"You run out of time\nYour score was: {score}!\nPress space to try again!";
        #endif
    }

    /// <summary>
    /// Updates the score UI
    /// </summary>
    private void UpdateScoreUI()
    {
        scoreText.text = $"Score {score}";
    }

    /// <summary>
    /// Updates the highscore UI
    /// </summary>
    private void UpdateHighscoreUI()
    {
        highscoreText.text = $"Highscore {highScore}";
    }

    private void UpdateShiftTargetUI()
    {
        shiftTargetText.text = $"Shift Taget {shiftTargets[currentShift]}";
    }

    /// <summary>
    /// Is called from a TouchInputDetector when it detects a touch.
    /// </summary>
    /// <param name="name">The name of the detector gameobject</param>
    public void TouchInputDetection(string name)
    {
        if (name == "Q")
        {
            turnLeft = true;
        }
        else if (name == "E")
        {
            turnRight = true;
        }
        else if (name == "BombPowerTouchDetection")
        {
            if (powerMeter == maxPowerMeter)
            {
                UseBombPower();
            }
        }
    }

    private void RestartShift()
    {
        if (gameSettingsManager.gameMode == GameMode.NormalGame)
        {
            currentShiftTime = shiftTimeLimits[currentShift];
        }
        else if (gameSettingsManager.gameMode == GameMode.FreePlay)
        {
            currentShiftTime = freePlayStartingTimer;
        }
        score = 0;
        powerMeter = 0f;
        tileGenerator.transform.rotation = transform.rotation;
        UpdateScoreUI();
        UpdateShiftTimerUI();
        UpdateShiftTargetUI();
        UpdatePowerMeterUI();
    }
}
