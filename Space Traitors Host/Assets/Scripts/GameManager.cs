using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int testVar;

    #region Game Initialisation

    private void Awake()
    {
        //Singleton Setup
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        InitialiseGame();
    }

    private void InitialiseGame()
    {
        //For initialising the Game when it is first opened
    }

    private void InitialiseServer()
    {
        //For initialising the server when the game is started
    }

    private void StartGame()
    {
        //For starting the game when game is started after all players connected
    }

    #endregion
}
