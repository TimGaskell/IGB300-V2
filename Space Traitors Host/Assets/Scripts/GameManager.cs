﻿using System;
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
    public int aiPowerChange;

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
        // Initialises all players in the players array
        players = new Player[NUM_PLAYERS];
        for (int playerID = 0; playerID < NUM_PLAYERS; playerID++)
        {
            players[playerID] = new Player(playerID, string.Format("Player{0}", playerID), CHARACTER_TYPES[playerID]);
        }
    }

    #endregion

    /// <summary>
    /// 
    /// Apply the spec challenge formula, comparing a player's score to a target score. Can also be used in combats.
    /// If the two scores are equal, result will be 50%. If the attacker's score if double or greater than the target
    /// score, then result will be 100%. 
    /// 
    /// Target Score cannot be equal to zero.
    /// 
    /// </summary>
    /// <param name="playerScore">The player performing the spec challenge's relevant spec score. Also the attacker's score in a combat</param>
    /// <param name="targetScore">The target score of the spec challenge, or the defender's relevant spce score</param>
    /// <returns>The chance for the player or the attacker to succeed on the spec challenge</returns>
    public float SpecChallengeChance(int playerScore, int targetScore)
    {
        if (targetScore == 0)
        {
            throw new DivideByZeroException("Target Score Cannot be zero in a Spec Challenge.");
        }

        return Math.Min(100, 50 + (playerScore - targetScore) * (50 / targetScore));
    }
}
