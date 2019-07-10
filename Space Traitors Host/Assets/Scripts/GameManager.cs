using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //For hardcode debugging, when wanting to test without server functionality
    private const int NUM_PLAYERS = 4; 
    private static readonly string[] CHARACTER_TYPES = { "Brute", "Butler", "Chef", "Engineer", "Singer", "Techie" };

    public readonly int MAX_POWER = 100;

    public static GameManager instance = null;

    public Player[] players;

    public int aiPower;

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

        // For debugging when not utilising server
        players = new Player[NUM_PLAYERS];
        for (int playerID = 0; playerID < NUM_PLAYERS; playerID++)
        {
            InitialisePlayer(playerID, string.Format("Player{0}", playerID), CHARACTER_TYPES[playerID]);
        }
    }

    public void InitialisePlayer(int playerID, string playerName, string characterType)
    {
        players[playerID] = new Player(playerID, playerName, characterType);
    }

    #endregion
}
