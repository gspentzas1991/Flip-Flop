using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public GameMode gameMode;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetGameMode(GameMode _gameMode)
    {
        gameMode = _gameMode;
    }
}
