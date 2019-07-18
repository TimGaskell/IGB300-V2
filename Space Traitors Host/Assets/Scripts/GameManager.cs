using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //To use to establish if testing is to be offline or online. Should always be reverted to true before building to publish
    public bool serverActive = false;

    //Used to detect if the game has been initialised or not. This is to prevent InitialiseGame being called twice when the game begins
    //(i.e. in awake and loading into main menu). Should be set true after initialisation is complete and false after leaving the main menu
    private bool gameInit = false;

    //Used for generating default player information if loading into a scene later than the lobby
    private const int DEFAULT_NUM_PLAYERS = 4;
    private static readonly string[] DEFAULT_NAMES = { "BruteTest", "ButlerTest", "ChefTest", "EngineerTest", "SingerTest", "TechieTest" };
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

    #region Player Retrieval

    /// <summary>
    /// 
    /// Returns the player information for a player of a particular ID
    /// 
    /// </summary>
    /// <param name="playerID">The ID of the player</param>
    /// <returns>The relevant player</returns>
    public Player GetPlayer(int playerID)
    {
        return players[playerID];
    }

    /// <summary>
    /// 
    /// Returns the player information for a player based on its position in the turn order. 
    /// 
    /// </summary>
    /// <param name="orderID">The index of the player in the player order list</param>
    /// <returns>The relevant player</returns>
    public Player GetOrderedPlayer(int orderID)
    {
        return players.Find(x => x.playerID == playerOrder[orderID]);
    }

    #endregion

    #region Scene Transition Handling

    /// <summary>
    /// 
    /// Used to detect when a new scene is loaded into, since Game Manager is enabled when this happens
    /// See 
    /// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
    /// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneUnloaded.html
    /// for more information
    /// 
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OldSceneUnloaded;
        SceneManager.sceneLoaded += NewSceneLoaded;
    }

    /// <summary>
    /// 
    /// Used to detect when a scene is loaded out of, since Game Manager is disabled when this happens
    /// See 
    /// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
    /// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneUnloaded.html
    /// for more information
    /// 
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= NewSceneLoaded;
        SceneManager.sceneUnloaded -= OldSceneUnloaded;
    }

    /// <summary>
    /// 
    /// Used for detecting when a scene is unloaded. Customised to detect for the different scenes in the game.
    /// See
    /// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneUnloaded.html
    /// for more information
    /// 
    /// </summary>
    /// <param name="scene"></param>
    private void OldSceneUnloaded(Scene scene)
    {
        if (serverActive)
        {
            throw new NotImplementedException("Server Functionality not Implemented");
            if (scene.name == "Main Menu")
            {
                //Reset the game initialisation so if the main menu is returned to, redoes initialisation
                gameInit = false;
            }
            else if (scene.name == "LobbyV2")
            {

            }
            else if (scene.name == "Character SelectionV2")
            {

            }
            else if (scene.name == "Game LevelV2")
            {

            }
        }
        else
        {
            if (scene.name == "Main Menu")
            {
                //Reset the game initialisation so if the main menu is returned to, redoes initialisation
                gameInit = false;
            }
            else if (scene.name == "LobbyV2")
            {
                
            }
            else if (scene.name == "Character SelectionV2")
            {

            }
            else if (scene.name == "Game LevelV2")
            {

            }
        }
    }

    /// <summary>
    /// 
    /// Used for detecting when a new scene is loaded into. Customised to detect for the different scenes in the game
    /// See
    /// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
    /// for more information
    /// 
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void NewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (serverActive)
        {
            throw new NotImplementedException("Server Functionality not Implemented");
            if (scene.name == "Main Menu")
            {
                InitialiseGame();
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
                //For debugging if wanting to go into character selection immediately, generates a default player list without characters
                if (players.Count == 0)
                {
                    GenerateDefaultPlayers(DEFAULT_NUM_PLAYERS, false);
                }

                //Character Selection should be done in the reverse order to the way the game is played, so should start at the end of the player order list
                activePlayer = numPlayers - 1;
                RandomiseOrder();
            }
            else if (scene.name == "Game Level")
            {
                //For debugging if wanting to go into game level immediately, generates a default player list with characters
                if (players.Count == 0)
                {
                    GenerateDefaultPlayers(DEFAULT_NUM_PLAYERS, true);
                }

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
        if (!gameInit)
        {
            ResetPlayers();
            playerOrder = new List<int>();

            //Instantiate the corruption abilities
            corruptionAbilities = new List<Ability>();
            corruptionAbilities.Add(new SensorScan());
            corruptionAbilities.Add(new CodeInspection());
            corruptionAbilities.Add(new Sabotage());
            corruptionAbilities.Add(new PowerUp());

            gameInit = true;
            Debug.Log("Game Initialised");
        }        
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
    /// Resets the player list to be a new list. Used when starting a new game
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

    /// <summary>
    /// 
    /// Generates a Default Player list based on the constants defined above
    /// 
    /// </summary>
    /// <param name="NumPlayers">The number of players to generate. Should be between 1 and 6</param>
    /// <param name="giveCharacters">Whether or not to define characters for the players</param>
    private void GenerateDefaultPlayers(int NumPlayers, bool giveCharacters)
    {
        numPlayers = NumPlayers;

        for (int playerID = 0; playerID < numPlayers; playerID++)
        {
            if (giveCharacters)
            {
                players.Add(new Player(playerID, DEFAULT_NAMES[playerID], CHARACTER_TYPES[playerID]));
            }
            else
            {
                players.Add(new Player(playerID, DEFAULT_NAMES[playerID]));
            }
        }
    }

    #endregion

    #region Round Management

    /// <summary>
    /// 
    /// Increment the turn order to the next player. Starts a new round if the current player is the last player in the order
    /// 
    /// </summary>
    public void IncrementTurn()
    {
        activePlayer++;
        //If the active player reaches the maximum number of players, the round has ended and a surge will occur
        if(activePlayer == numPlayers)
        {
            activePlayer = 0;
            //Need to add traitor handling for surges in here (i.e. power increases and traitor selection)
        }
    }

    #endregion

    #region Traitor Handling



    #endregion
}
