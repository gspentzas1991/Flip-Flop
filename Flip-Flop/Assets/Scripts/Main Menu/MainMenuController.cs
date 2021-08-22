using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    GameSettingsManager gameSettingsManager;
    private void Start()
    {
        gameSettingsManager = GameObject.FindGameObjectWithTag("GameSettings").GetComponent<GameSettingsManager>();    
    }

    public void StartGame()
    {
        gameSettingsManager.SetGameMode(GameMode.NormalGame);
        SceneManager.LoadScene(1);
    }

    public void StartFreePlay()
    {
        gameSettingsManager.SetGameMode(GameMode.FreePlay);
        SceneManager.LoadScene(1);
    }
}
