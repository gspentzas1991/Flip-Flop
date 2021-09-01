using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataManager
{
    public static void SetHighScore(float Value)
    {
        PlayerPrefs.SetFloat("highscore", Value);
    }

    public static float GetHighScore()
    {
        return PlayerPrefs.GetFloat("highscore");
    }
}
