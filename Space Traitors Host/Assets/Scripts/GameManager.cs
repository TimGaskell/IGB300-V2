using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //To use to establish if testing is to be offline or online. Should always be reverted to true before building to publish
    public bool serverActive = false;

    //For hardcode debugging, when wanting to test without server functionality
    private static readonly string[] CHARACTER_TYPES = { "Brute", "Butler", "Chef", "Engineer", "Singer", "Techie" };

    public readonly int MAX_POWER = 100;

    public readonly int MIN_PLAYERS = 2;
    public readonly int MAX_PLAYERS = 6;

    public static GameManager instance = null;

    public int numPlayers;
    public List<Player> players;
    public List<int> playerOrder;
    //The active player is to identify which player is currently meant to be doing something. This is not related to the player ID and is
    //instead the index in the player order list
    public int activePlayer = 0;

    public int aiPower;
    public int aiPowerChange;

    public List<Ability> corruptionAbilities;

    private void Update()
    {

    }

    /// <summary>
    /// 
    /// Return the information about the player which is currently active
    /// 
    /// </summary>
    /// <returns></returns>
    public Player ActivePlayer()
    {
        return players.Find(x => x.playerID == playerOrder[activePlayer]);
    }

    #region Scene Transition Handling
    private void OnEnable()
    {
        SceneManager.sceneLoaded += NewSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= NewSceneLoaded;
    }

    private void NewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (serverActive)
        {
            throw new NotImplementedException("Server Functionality not Implemented");
            if (scene.name == "Main Menu")
            {

            }
            else if (scene.name == "LobbyV2")
            {

            }
            else if (scene.name == "Character SelectionV2")
            {

            }
            else if (scene.name == "Game Level")
            {

            }
        }
        else
        {
            if (scene.name == "Main Menu")
            {
                InitialiseGame();
            }
            else if (scene.name == "LobbyV2")
            {
                numPlayers = 0;
                ResetPlayers();
            }
            else if (scene.name == "Character SelectionV2")
            {
                //Character Selection should be done in the reverse order to the way the game is played, so should start at the end of the player order list
                activePlayer = numPlayers - 1;
                RandomiseOrder();
            }
            else if (scene.name == "Game Level")
            {
                StartGame();
            }
        }
    }

    #endregion

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

    /// <summary>
    /// 
    /// Initialise the game when it first opened or returns to the main menu
    /// 
    /// </summary>
    private void InitialiseGame()
    {
        ResetPlayers();
        playerOrder = new List<int>();

        //Instantiate the corruption abilities
        corruptionAbilities = new List<Ability>();
        corruptionAbilities.Add(new SensorScan());
        corruptionAbilities.Add(new CodeInspection());
        corruptionAbilities.Add(new Sabotage());
        corruptionAbilities.Add(new PowerUp());
    }

    /// <summary>
    /// 
    /// Initialise the server when the game is started
    /// 
    /// </summary>
    private void InitialiseServer()
    {
        
    }

    /// <summary>
    /// 
    /// Start the game when all players have chosen their characters and the players start the game
    /// 
    /// </summary>
    private void StartGame()
    {
        activePlayer = 0;
    }

    #endregion

    #region Spec Challenge

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
            throw new DivideByZeroException("Target Score cannot be zero in a Spec Challenge.");
        }

        return Math.Min(100, 50 + (playerScore - targetScore) * (50 / targetScore));
    }

    #endregion

    #region Character and Name Selection

    /// <summary>
    /// 
    /// Adds a new player to the players list
    /// 
    /// </summary>
    /// <param name="playerID">The connection ID of the player</param>
    /// <param name="playerName">The custom name of the player</param>
    public void GeneratePlayer(int playerID, string playerName)
    {
        players.Add(new Player(playerID, playerName));
    }

    /// <summary>
    /// 
    /// Resets the player list to be a new list
    /// 
    /// </summary>
    public void ResetPlayers()
    {
        players = new List<Player>();
    }

    /// <summary>
    /// 
    /// Determines a random order for the players to play the game in. Places these values in an ordered list of player IDs.
    /// 
    /// </summary>
    private void RandomiseOrder()
    {
        int randomPlayer;

        for (int playerID = 0; playerID < numPlayers; playerID++)
        {
            //do-while loop checks if the ordered list already contains the random player ID. If it does, then obtains a new random ID.
            do
            {
                randomPlayer = UnityEngine.Random.Range(0, numPlayers);

            } while (playerOrder.Contains(randomPlayer));

            playerOrder.Add(randomPlayer);
        }
    }

    /// <summary>
    /// 
    /// Returns if the character has already been selected in the character list. If it has returns true, otherwise false
    /// 
    /// </summary>
    /// <param name="characterType">The type of character to be checked</param>
    /// <returns>Whether the character has already been selected or not</returns>
    public bool CheckCharacterSelected(string characterType)
    {
        return players.Exists(x => x.CharacterType == characterType);
    }

    public void SelectCharacter(string characterType)
    {
        players[playerOrder[activePlayer]].CharacterType = characterType;
        //Character Selection is in the reverse player order, so works backward through the player order list
        activePlayer--;
    }

    #endregion

    #region Traitor Handling



    #endregion
}
