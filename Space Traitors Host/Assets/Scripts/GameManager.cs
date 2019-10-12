using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Names of scenes for scene management
    public const string MainMenuScene = "Server MainMenu";

    public const string LobbyScene = "LobbyLan Host";
    public const string CharacterScene = "Server Character Selection";
    public const string MainGameScene = "Server GameLevel";

    public const string NoServerLobbyScene = "NOSERVER Lobby";
    public const string NoServerCharacterScene = "NOSERVER Character Selection";
    public const string NoServerMainGameScene = "NOSERVER Game Level";

    //To use to establish if testing is to be offline or online. Should always be reverted to true before building to publish
    public bool serverActive => true;

    //Used to detect if the game has been initialised or not. This is to prevent InitialiseGame being called twice when the game begins
    //(i.e. in awake and loading into main menu). Should be set true after initialisation is complete and false after leaving the main menu
    private bool gameInit = false;

    //A default player ID for dummy players (i.e. when a target is not needed). -1 should never be a valid player ID so it used here
    public const int DEFAULT_PLAYER_ID = -1;

    //Amount of damage to deal during a combat, whether it be between players or from an AI Attack
    public const int COMBAT_DAMAGE = -1;

    //Used for generating default player information if loading into a scene later than the lobby
    private const int DEFAULT_NUM_PLAYERS = 2;
    private static readonly string[] DEFAULT_NAMES = { "ButlerTest", "EngineerTest", "SingerTest", "TechieTest", "BruteTest", "ChefTest" };
    private static readonly Character.CharacterTypes[] CHARACTER_TYPES = { Character.CharacterTypes.Butler, Character.CharacterTypes.Engineer,
        Character.CharacterTypes.Singer, Character.CharacterTypes.Techie, Character.CharacterTypes.Brute, Character.CharacterTypes.Chef };

    public enum SpecScores { Default, Brawn, Skill, Tech, Charm };

    public readonly int MAX_POWER = 100;

    public readonly int MIN_PLAYERS = 2;
    public readonly int MAX_PLAYERS = 4;

    public static GameManager instance = null;
    public int ServerSize = 0;

    public int numPlayers;
    public List<Player> players;
    public List<int> playerOrder;

    public List<Player> Deadplayers;
    //The active player is to identify which player is currently meant to be doing something. This is not related to the player ID and is
    //instead the index in the player order list
    public int activePlayer = 0;

    public List<Sprite> characterPortraits;

    public GameObject playerList;
    public List<GameObject> playerPrefabs;
    public GameObject playerStartingPostition;

    //The number of components is always equal to the number of players (if increasing number of components in a game, change here)
    public int NumComponents { get { return numPlayers; } }

    private float aiPower;
    public float AIPower { get { return aiPower; } private set { aiPower = Mathf.Clamp(value, 0, MAX_POWER); } }
    //Changes in AI power per round
    public float basePower;
    public float playerPower;
    public float aiPowerChange;
    //The target score for the player when the AI attacks them
    public int aiTargetScore;

    //Action Points provided to a player when they roll during their turn
    public int actionPoints;

    //The ID of a player who has newly been selected as traitor
    public int newTraitor;
    //The ID of the Player being targeted during an AI attack
    public int targetPlayer;
    //Boolean to determine if a traitor was selected in the previous round
    private bool traitorDelay;

    // Constants used in calculating the changes in AI Power during a surge. Increasing either of these values will increase the power gain per turn
    private const float BASE_POWER_MOD = 24.0f;
    private const float PLAYER_POWER_MOD = 0.2f;

    //The base target score for when the AI begins to attack the players
    private const int BASE_TARGET_SCORE = 5;
    //The increase the AI target score every round
    private const int AI_TARGET_INCREASE = 1;
    //The increase in a traitors corruption every round during a surge
    private const int TRAITOR_CORRUPTION_MOD = 15;

    //The modifier for spec scores when one player counters another in combat
    private const int COUNTER_MOD = 2;

    public GameObject roomList;

    public int installedComponents;
    //boolean used in the sabotage ability to detect if a player has sabotaged the escape shuttle
    public int sabotageCharges;

    public enum TurnPhases { Default, Abilities, ActionPoints, Movement, Interaction, BasicSurge, AttackSurge };
    public TurnPhases currentPhase;

    //Constants used to determine how many "dice" to roll when calculating action points and how many
    //"sides" the dice are to have
    public const int AP_NUM_DICE = 1;
    public const int AP_DICE_SIDES = 4;

    //The conversion factor for a player when they have leftover action points
    private const float AP_CONVERSION = 0.5f;

    //Variables used during the movement phase
    //roomSelection is true if the player is needing to select a room to move to. Only used in serverless version of the game
    //playerMoving is true if the player model is moving across the map.
    //playerGoalIndex is the target room index the active player is moving towards.
    public bool roomSelection;
    public bool playerMoving;
    public int playerGoalIndex;

    //Used for outputting if any of the victory conditions, traitor or non-traitor have been met
    public enum VictoryTypes { None, Traitor, NonTraitor };
    public VictoryTypes CurrentVictory;

    //The ID of the last player alive. If there is more than one player alive, then this should be the default case.
    public int traitorWinID;

    public int ReadyPlayers = 0;

    private void Update()
    {
        //Only need to detect if the player is clicking on a room on the host system if the server is inactive
        if (roomSelection && !serverActive)
        {
            ClickRoom();
        }
        else if (roomSelection && serverActive) {
            ClientClickRoom();
        }

        if (playerMoving)
        {
            playerList.GetComponent<PlayerMovement>().PlayerMoveViaNodes(playerGoalIndex);
        }
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
        //return players.Find(x => x.playerID == playerID);
        return players[playerID - 1];
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

        return players.Find(x => x.playerID == playerOrder[orderID-1]);
    }

    /// <summary>
    /// 
    /// Returns the active player
    /// 
    /// </summary>
    /// <returns>The active player</returns>
    public Player GetActivePlayer()
    {
        return GetOrderedPlayer(activePlayer);
    }

    /// <summary>
    /// 
    /// Get the turn position of a particular player ID
    /// 
    /// </summary>
    public int GetPlayerOrder(int playerID)
    {
        return playerOrder.Find(x => x == playerID) - 1;
    }

    /// <summary>
    /// 
    /// Returns the player of a particular character type
    /// 
    /// </summary>
    /// <param name="characterType">The character type to retrieve</param>
    /// <returns>The player of the character type</returns>
    public Player GetPlayer(Character.CharacterTypes characterType)
    {
        return players.Find(x => x.Character.CharacterType == characterType);
    }

    /// <summary>
    /// 
    /// Gets the character portrait for a character of a particular type
    /// 
    /// </summary>
    /// <param name="characterType">The required character type</param>
    /// <returns>Returns the sprite image of the relevant character type</returns>
    public Sprite GetCharacterPortrait(Character.CharacterTypes characterType)
    {
        switch (characterType)
        {
            case (Character.CharacterTypes.Brute):
                return characterPortraits[0];
            case (Character.CharacterTypes.Butler):
                return characterPortraits[1];
            case (Character.CharacterTypes.Chef):
                return characterPortraits[2];
            case (Character.CharacterTypes.Engineer):
                return characterPortraits[3];
            case (Character.CharacterTypes.Singer):
                return characterPortraits[4];
            case (Character.CharacterTypes.Techie):
                return characterPortraits[5];
            default:
                throw new NotImplementedException("Not a valid character type.");
        }
    }

    #endregion

    #region Room Retrieval

    public Room GetRoom(int roomIndex)
    {
        return roomList.GetComponent<ChoiceRandomiser>().rooms[roomIndex].GetComponent<Room>();
    }

    /// <summary>
    /// 
    /// Returns a list of all the room scripts on the rooms adjacent to the room of the given index
    /// 
    /// </summary>
    /// <param name="roomIndex">The index of the room to get adjacent to</param>
    /// <returns>The list of adjacent rooms</returns>
    public List<Room> GetAdjacentRooms(int roomIndex)
    {
        GameObject[] adjacentRoomObjects = roomList.GetComponent<ChoiceRandomiser>().rooms[roomIndex].GetComponent<LinkedNodes>().linkedNodeObjects;
        List<Room> adjacentRooms = new List<Room>();

        foreach (GameObject roomObject in adjacentRoomObjects)
        {
            adjacentRooms.Add(roomObject.GetComponent<Room>());
        }

        return adjacentRooms;
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
            
            if (scene.name == MainMenuScene)
            {
                InitialiseGame();
                MusicManager.instance.ChangeMusicClip(MusicManager.instance.menuMusic);
            }
            else if (scene.name == LobbyScene)
            {

            }
            else if (scene.name == CharacterScene )
            {
                //Character Selection should be done in the reverse order to the way the game is played, so should start at the end of the player order list
                activePlayer = numPlayers;
                RandomiseOrder();
  

            }
            else if (scene.name == MainGameScene)
            {
                Server.Instance.sendplayerIDS();
                Server.Instance.sendallCharacterTypes();
                Server.Instance.sendAllPlayerNames();
                StartGame();
                MusicManager.instance.ChangeMusicClip(MusicManager.instance.gameMusic);

            }
        }
        else
        {
            if (scene.name == MainMenuScene)
            {
                InitialiseGame();
            }
            else
            {
                //Reset the game initialisation so if the main menu is returned to, redoes initialisation
                gameInit = false;

                if (scene.name == NoServerLobbyScene)
                {
                    numPlayers = 0;
                    ResetPlayers();
                }
                else if (scene.name == NoServerCharacterScene)
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
                else if (scene.name == NoServerMainGameScene)
                {
                    //For debugging if wanting to go into game level immediately, generates a default player list with characters
                    if (players.Count == 0)
                    {
                        GenerateDefaultPlayers(DEFAULT_NUM_PLAYERS, true);
                        RandomiseOrder();
                    }

                    StartGame();
                }
            }
            
        }
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
            if (scene.name == MainMenuScene)
            {

            }
            else if (scene.name == LobbyScene)
            {

            }
            else if (scene.name == CharacterScene)
            {

            }
            else if (scene.name == MainGameScene)
            {

            }
        }
        else
        {
            if (scene.name == MainMenuScene)
            {
                
            }
            else if (scene.name == NoServerLobbyScene)
            {

            }
            else if (scene.name == NoServerCharacterScene)
            {

            }
            else if (scene.name == NoServerMainGameScene)
            {
                //Unload the room list
                roomList = null;
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
            return;
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

            gameInit = true;
            Debug.Log("Game Initialised");

            currentPhase = TurnPhases.Default;
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
        ServerSize = numPlayers;

        //Reset variables to their default state
        activePlayer = 1;
        installedComponents = 0;

        aiPower = 0;
        aiTargetScore = BASE_TARGET_SCORE;
        traitorDelay = false;

        //Sets a default player traitor and target
        newTraitor = DEFAULT_PLAYER_ID;
        targetPlayer = DEFAULT_PLAYER_ID;

        roomList = GameObject.FindWithTag("RoomList");
        roomList.GetComponent<ChoiceRandomiser>().ChoiceSetup();

        InstantiatePlayers();

        currentPhase = TurnPhases.Default;

        roomSelection = false;
        playerMoving = false;

        CurrentVictory = VictoryTypes.None;

        traitorWinID = DEFAULT_PLAYER_ID;

        sabotageCharges = 0;
        Debug.Log("Sent Start Game to " + GetActivePlayer().playerID);
        Server.Instance.SendActivePlayer(GetActivePlayer().playerID);
        
    }

    /// <summary>
    /// 
    /// Instantiate the player objects in the game world
    /// 
    /// </summary>
    public void InstantiatePlayers()
    {
        playerList = GameObject.FindWithTag("PlayerList");
        playerStartingPostition = GameObject.FindWithTag("StartPosition");

        Vector3 positionOffset = new Vector3(0.0f, 22.5f, 0.0f);

        
        //Quaternion playerRotation = roomList.GetComponent<ChoiceRandomiser>().rooms[Player.STARTING_ROOM_ID].transform.rotation;
        Quaternion playerRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        foreach (Player player in players)
        {
            GameObject playerModel = playerPrefabs.Find(x => x.GetComponent<PlayerObject>().CharacterType == player.Character.CharacterType);
            Vector3 playerStart = playerStartingPostition.transform.GetChild(player.playerID).transform.position + positionOffset;
            player.playerObject = Instantiate(playerModel, playerStart, playerRotation, playerList.transform);
        }
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
    public static float SpecChallengeChance(float playerScore, float targetScore)
    {
        if (targetScore == 0)
        {
            throw new DivideByZeroException("Target Score cannot be zero in a Spec Challenge.");
        }

        return Mathf.Min(100.0f, 50.0f + ((float)playerScore - (float)targetScore) * (50.0f / targetScore));
    }

    /// <summary>
    /// 
    /// Determines if a player was successful in their spec challenge or not
    /// 
    /// </summary>
    /// <param name="PlayerScore">The player performing the spec challenge's relevant spec score. Also the attacker's score in a combat</param>
    /// <param name="targetScore">The target score of the spec challenge, or the defender's relevant spce score</param>
    /// <returns>True if the player suceeded. False otherwise</returns>
    public static bool PerformSpecChallenge(float PlayerScore, float targetScore)
    {
        //Pick a random number between 0 and 100. This determines if the player is successful in the spec challenge or not
        float successFactor = UnityEngine.Random.Range(0f, 100f);

        return successFactor <= SpecChallengeChance(PlayerScore, targetScore);
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
                randomPlayer = UnityEngine.Random.Range(1, numPlayers+1);

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
    public bool CheckCharacterSelected(Character.CharacterTypes characterType)
    {
        return players.Exists(x => x.Character.CharacterType == characterType);
    }

    /// <summary>
    /// 
    /// Selects the character type of the active player
    /// 
    /// </summary>
    /// <param name="characterType">The character type to be given to the player</param>
    public void SelectCharacter(Character.CharacterTypes characterType)
    {
        GetActivePlayer().Character = new Character(characterType);
        GetActivePlayer().GenerateAbilityList();
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

    #region Round and Phase Management

    /// <summary>
    /// 
    /// Increment the turn order to the next player. Starts a new round if the current player is the last player in the order
    /// 
    /// </summary>
    public void IncrementTurn()
    {
        activePlayer++;

        //If the active player reaches the maximum number of players, the round has ended and a surge will occur
        if (activePlayer == numPlayers + 1) {
            activePlayer = 1;
            if (CheckNonTraitorVictory()) {
                CurrentVictory = VictoryTypes.NonTraitor;
                Server.Instance.SendNonTraitorVictory();
            }
            else {
                ActivateSurge();
            }
        }
        else {

            Debug.Log("ABILITY TEST " + GetActivePlayer().PreviousTarget + " " + GetActivePlayer().PreviousAbility);

            if (GetActivePlayer().PreviousTarget != 0) {
                GetPlayer(GetActivePlayer().PreviousTarget).DisableActiveAbility(GetActivePlayer().PreviousAbility);

                GetActivePlayer().PreviousTarget = 0;
                GetActivePlayer().PreviousAbility = null;

            }



            Debug.Log("Incremenet turn, Active player " + activePlayer);

            currentPhase = TurnPhases.Default;

            Server.Instance.SendActivePlayer(GetActivePlayer().playerID);
        }
       

  
        

       
    }

    public void EndRound() {

        ReadyPlayers++;

        if(ReadyPlayers == numPlayers) {

            Server.Instance.SendActivePlayer(GetActivePlayer().playerID);
            ReadyPlayers = 0;

        }

    }

    /// <summary>
    /// 
    /// Shifts the players current phase of their turn from one to the next. Order should be:
    /// Abilities -> ActionPoints -> Movement -> Interaction -> Abilities
    /// If after moving out of the interaction phase it is the last player in the turn order's phase,
    /// will move into either one of the surge states, after which will move back into the first players
    /// abilities phase
    /// 
    /// </summary>
    public void IncrementPhase()
    {
        switch (currentPhase)
        {
            case (TurnPhases.Default):
                CameraSystem.instance.ZoomIn(GetActivePlayer().playerObject);
                currentPhase += 1;
                break;
            case (TurnPhases.Abilities):
                currentPhase += 1;
                break;               
            case (TurnPhases.Movement):
                currentPhase += 1;
                break;
            case (TurnPhases.ActionPoints):
                currentPhase += 1;
                roomSelection = true;
                //Apply the active player model to be moved
                Debug.Log("This Happens");
                playerList.GetComponent<PlayerMovement>().Player = GetActivePlayer().playerObject;
                CameraSystem.instance.ZoomOut();
                break;
            case (TurnPhases.Interaction):
                currentPhase += 1;
                break;
            case (TurnPhases.BasicSurge):
            case (TurnPhases.AttackSurge):
                CameraSystem.instance.ZoomOut();
                currentPhase = TurnPhases.Abilities;
                aiPowerChange = 0;
                newTraitor = DEFAULT_PLAYER_ID;
                targetPlayer = DEFAULT_PLAYER_ID;
                break;
            default:
                throw new NotImplementedException("Not a valid phase");
        }
    }

    #endregion

    #region Traitor Handling

    /// <summary>
    /// 
    /// Performs a surge. Increments AI Power or if the AI Power is at 100% choose a random target for the AI Attacks
    /// 
    /// </summary>
    private void ActivateSurge()
    {
        if (AIPower < 100)
        {
            currentPhase = TurnPhases.BasicSurge;

            basePower = BASE_POWER_MOD / numPlayers;
            playerPower = PLAYER_POWER_MOD * (TotalCorruption(true) / numPlayers);

            AIPower += AIPowerIncrease();

            Server.Instance.SendSurge();
        }
        else
        {
            currentPhase = TurnPhases.AttackSurge;
            //Chooses a random target if the AI Power is at 100%. To update the UI will need to do a check in the UI Manager
            //to see if the target is not the default case
            targetPlayer = AIChooseTarget();
            Debug.Log(targetPlayer);

            Server.Instance.SendAIAttack(targetPlayer);
        }

        //Test if a traitor needs to be selected, then picks a traitor if so, returning the new traitors ID
        if (IsTraitorSelected())
        {
            Debug.Log("This is activated and a traitor has been selected ------------------------------------");
            newTraitor = ChooseTraitor();
            Server.Instance.SendIsTraitor();
        }

        //Increase corruption for all traitors
        RoundCorruptionIncrease();

        if (GetActivePlayer().PreviousTarget != 0) {
            Debug.Log("ABILITY TEST " + GetActivePlayer().PreviousTarget + " " + GetActivePlayer().PreviousAbility.abilityType);

            GetPlayer(GetActivePlayer().PreviousTarget).DisableActiveAbility(GetActivePlayer().PreviousAbility);
            GetActivePlayer().PreviousTarget = 0;
            GetActivePlayer().PreviousAbility = null;

        }

    }

    public float AIPowerIncrease()
    {
        return basePower + playerPower + aiPowerChange;
    }

    /// <summary>
    /// 
    /// Calculates the sum of corruption from all players. Can include only players which are not traitors or all players in the sum as desired.
    /// 
    /// </summary>
    /// <param name="includeTraitors">Whether or not to consider the corruption of traitor characters when calculating the total</param>
    /// <returns>The sum of corruption</returns>
    private int TotalCorruption(bool includeTraitors)
    {
        int totalCorruption = 0;

        foreach (Player player in players)
        {
            //Do not want to include a players corruption if traitor corruption is not to be considered
            //and the player is a traitor
            if (!(!includeTraitors && player.isTraitor))
            {
                totalCorruption += player.Corruption;
            }
        }

        return totalCorruption;
    }

    /// <summary>
    /// 
    /// Increases the corruption per round for all traitors
    /// 
    /// </summary>
    private void RoundCorruptionIncrease()
    {
        foreach (Player player in players)
        {
            if (player.isTraitor)
            {
                player.Corruption += TRAITOR_CORRUPTION_MOD;
            }
        }
    }

    /// <summary>
    /// 
    /// Determines if there is to be a traitor selected, returning true if so and false otherwise
    /// 
    /// </summary>
    /// <returns>If a traitor is to be selected, returns true. Otherwise returns false</returns>
    private bool IsTraitorSelected()
    {
        //First checks if there are still avaialable slot for a player to become the traitor. If there is an available slot, continues to determine if a traitor is to be selected
        if (TraitorCountCheck())
        {
            //Then checks if there has been a delay in rounds since the last traitor was selected.
            //If there hasn't, resets the delay and returns false
            if (!traitorDelay)
            {
                //Determines a random number between 0 and 100 and then checks if the random number is less than the AI Power. If it is, sets up the traitorDelay to
                //prevent a traitor being selected next round and returns true. Otherwise, resets the traitor delay and returns false.
                float randomChance = UnityEngine.Random.Range(0.0f, 100.0f);
                if (randomChance <= aiPower)
                {
                    traitorDelay = true;
                    return true;
                }
                else
                {
                    traitorDelay = false;
                    return false;
                }
            }
            else
            {
                traitorDelay = false;
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 
    /// Checks if the number of traitors in the game is less than the number of players by 1, preventing the last player
    /// from being selected as the traitor
    /// 
    /// </summary>
    /// <returns>If there are still available slots for a traitor, returns true. Otherwise, returns false</returns>
    private bool TraitorCountCheck()
    {
        int traitorCount = 0;

        //Count up the number of traitors
        foreach (Player player in players)
        {
            if (player.isTraitor)
            {
                traitorCount += 1;
            }
        }

        return traitorCount < numPlayers - 1;
    }

    /// <summary>
    /// 
    /// Select a player ID at random from the non-traitors to be traitor. Randomness is based on the player's corruption
    /// 
    /// </summary>
    /// <returns></returns>
    private int ChooseTraitor()
    {
        //Want the total corruption without traitors as a scaling for each players corruption
        int totalCorruption = TotalCorruption(false);

        if (totalCorruption == 0)
        {
            int randomPlayerID;

            //Need to ensure that the new traitor has not already been selected, so repeats the random selection until finds a
            //character which is not a traitor.
            do
            {
                randomPlayerID = UnityEngine.Random.Range(0, numPlayers);

            } while (GetPlayer(randomPlayerID).isTraitor);

            AssignTraitor(randomPlayerID);

            return randomPlayerID;
        }
        else
        {
            float chanceCounter = 0;
            float randomChance = UnityEngine.Random.Range(0.0f, 100.0f);

            foreach (Player player in players)
            {
                //Cannot consider players which are traitors, so will ignore them in the summation
                if (!player.isTraitor)
                {
                    //Add the player's proability, then determine if the random number falls in that probability range.
                    //If it does, that player is to be selected as traitor
                    chanceCounter += (float)player.Corruption / totalCorruption * 100.0f;
                    if (randomChance <= chanceCounter)
                    {
                        //Increase the traitor corruption and sets them as a traitor
                        AssignTraitor(player.playerID);
                        Debug.Log("IsTraitorSelected");
                        return player.playerID;
                    }
                }
            }
        }

        //Provides a dummy output for the function. Should never reach this point
        return DEFAULT_PLAYER_ID;
    }

    /// <summary>
    /// 
    /// Assign a particular player to be traitor
    /// 
    /// </summary>
    /// <param name="playerID">The ID of the player becoming the traitor</param>
    private void AssignTraitor(int playerID)
    {
        GetPlayer(playerID).isTraitor = true;
        //GetPlayer(playerID).Corruption += TRAITOR_CORRUPTION_MOD;
    }

    /// <summary>
    /// 
    /// The AI chooses a random player target for its attacks
    /// 
    /// </summary>
    /// <returns>The random target's player index</returns>
    private int AIChooseTarget()
    {
        return UnityEngine.Random.Range(1, numPlayers+1);
    }

    /// <summary>
    /// 
    /// Determines if the player wins a combat against the AI attacking it. Returns true if the player wins, false otherwise
    /// 
    /// </summary>
    /// <param name="specScore">The name of the spec score the player wants to use against the AI</param>
    /// <returns>True if the player wins the combat. False otherwise</returns>
    public bool AIAttackPlayer(SpecScores specScore)
    {
        bool playerWin;

        float targetSpecScore = GetPlayer(targetPlayer).GetScaledSpecScore(specScore);

        //Determines if the target player wins the combat against the AI. If they do, there is no change. However if they lose, then
        //the target player loses a life point
        if (PerformSpecChallenge(targetSpecScore, aiTargetScore))
        {
            playerWin = true;
        }
        else
        {
            playerWin = false;
            GetPlayer(targetPlayer).ChangeLifePoints(COMBAT_DAMAGE);
        }

        //The AI attacks should get harder to beat every round, so this will increment after an attack (regardless of the player winning or losing the combat)
        aiTargetScore += AI_TARGET_INCREASE;       

        return playerWin;
    }
    #endregion

    #region Combat Handling and Traitor Victory Conditions

   /// <summary>
   /// 
   /// Checks if the active player is able to attack any of the other players and returns a list of all valid players player IDs that they can attack
   /// 
   /// </summary>
   /// <returns>A list of all the player IDs that the active player can attack</returns>
    public List<int> CheckCombat()
    {
        //List of IDs which the active player is able to attack
        List<int> validIDs = new List<int>();

        Player attackingPlayer = GetActivePlayer();

        foreach(Player defendingPlayer in players)
        {
            if(defendingPlayer.playerID == attackingPlayer.playerID)
            {
                continue;
            }
            else
            {
                //Checks if the players are in the same room as well as if either the attacking player is a traitor, or the defending player has been revealed as a traitor
                if (attackingPlayer.roomPosition == defendingPlayer.roomPosition) 
                    //(attackingPlayer.isTraitor || defendingPlayer.isRevealed))
                {
                    validIDs.Add(defendingPlayer.playerID);
                }
            }
        }

        return validIDs;
    }

    /// <summary>
    /// 
    /// Performs a combat scenario between two players, considering the counters for each player based on the given spec score. The attacker
    /// will always be the active player
    /// 
    /// </summary>
    /// <param name="attackerSpec">The name of the spec score the attacker is using</param>
    /// <param name="defenderID">The playerID of the defender</param>
    /// <param name="defenderSpec">The name of the spec score the attacker is using</param>
    /// <returns>If the attacker wins the combat, returns true. If the defender wins, returns false</returns>
    public bool PerformCombat(SpecScores attackerSpec, int defenderID, SpecScores defenderSpec)
    {
        Player attackingPlayer = GetActivePlayer();
        Player defendingPlayer = GetPlayer(defenderID);

        float attackerScore = attackingPlayer.GetScaledSpecScore(attackerSpec);
        float defenderScore = defendingPlayer.GetScaledSpecScore(defenderSpec);

        //If the attacking player is a traitor but has not been revealed, that player is revealed as the traitor
        if (attackingPlayer.isTraitor && !attackingPlayer.isRevealed)
        {
            GetActivePlayer().isRevealed = true;
        }

        //Below statements determine if the attacker or defender counters the other combat participant. If they do, their
        //score is multiplied by the counter multiplier. If there are no counters, leaves scores untouched.
        if (DetermineCounter(attackerSpec, defenderSpec))
        {
            attackerScore *= COUNTER_MOD;
        }
        else if (DetermineCounter(defenderSpec, attackerSpec))
        {
            defenderScore *= COUNTER_MOD;
        }

        //Once the counters have been determined, performs the combat between the two players. If the spec challenge is
        //successful, then the attacker wins, otherwise the defender loses.
        if (PerformSpecChallenge(attackerScore, defenderScore))
        {
            GetPlayer(defenderID).ChangeLifePoints(COMBAT_DAMAGE);
            return true;
        }
        else
        {
            GetActivePlayer().ChangeLifePoints(COMBAT_DAMAGE);
            return false;
        }
    }

    private class SpecComparison
    {
        public SpecScores Spec1 { get; set; }
        public SpecScores Spec2 { get; set; }
    }

    private readonly SpecComparison[] counters = 
    {
        new SpecComparison { Spec1 = SpecScores.Brawn, Spec2 = SpecScores.Charm },
        new SpecComparison { Spec1 = SpecScores.Charm, Spec2 = SpecScores.Tech },
        new SpecComparison { Spec1 = SpecScores.Tech, Spec2 = SpecScores.Skill },
        new SpecComparison { Spec1 = SpecScores.Skill, Spec2 = SpecScores.Brawn }
    };

    /// <summary>
    /// 
    /// Determines if a given spec score counters another. If spec1 counters spec2, returns true. Otherwise returns false
    /// 
    /// </summary>
    /// <param name="spec1">The name of the spec score to be countering</param>
    /// <param name="spec2">The name of the spec score to be countered</param>
    /// <returns>If spec1 counters spec2, returns true. Otherwise false</returns>
    private bool DetermineCounter(SpecScores spec1, SpecScores spec2)
    {
        return counters.Any(x => spec1 == x.Spec1 && spec2 == x.Spec2);
    }

    /// <summary>
    /// 
    /// Checks if there is only one player left alive in the game. Returns the player ID of the last player if there is only one player left.
    /// If not returns the default player ID. Will return the default ID if there are no players left alive, however this should never be the case.
    /// 
    /// </summary>
    public void CheckTraitorVictory()
    {
        foreach (Player player in players) {

            if(traitorWinID == DEFAULT_PLAYER_ID) {

                traitorWinID = player.playerID;

            }
            else {
                traitorWinID = DEFAULT_PLAYER_ID;

            }

            Debug.Log(traitorWinID);
        }
        if(traitorWinID != DEFAULT_PLAYER_ID) {

            CurrentVictory = VictoryTypes.Traitor;
        }
       
    }

    #endregion

    #region Non-Traitor Victory Handling

    /// <summary>
    /// 
    /// Checks if the active player can install a component or not
    /// 
    /// </summary>
    /// <returns>True if the player has a component, false otherwise</returns>
    public bool CanInstallComponent()
    {
        return GetActivePlayer().hasComponent;
    }

    /// <summary>
    /// 
    /// Installs a component from the active player. If the component was installed successfully, returns true.
    /// If they fail to install due to a sabotage, returns false
    /// 
    /// </summary>
    /// <returns>Returns true if the player installed the component successfully. False otherwise</returns>
    public bool InstallComponent()
    {
        if (sabotageCharges == 0)
        {
            GetActivePlayer().hasComponent = false;
            installedComponents += 1;
            SFXManager.instance.PlaySoundEffect(SFXManager.instance.notificationSound);
            return true;
        }
        else
        {
            GetActivePlayer().ChangeLifePoints(COMBAT_DAMAGE);
            sabotageCharges--;
            return false;
        }
    }

    /// <summary>
    /// 
    /// Checks whether there have been enough components installed to trigger a non-traitor victory
    /// 
    /// </summary>
    /// <returns>If the non-traitors can win, returns true. Otherwise false</returns>
    public bool CheckInstalledComponents()
    {
        return installedComponents == NumComponents;
    }

    /// <summary>
    /// 
    /// Checks that all the non-traitors are in the escape room and returns true if so. If there is 
    /// a non traitor that is not in the escape room, returns false. If not all the components have been installed
    /// also returns false
    /// 
    /// </summary>
    /// <returns></returns>
    private bool CheckNonTraitorVictory()
    {
        if (CheckInstalledComponents())
        {
            foreach (Player player in players)
            {
                if (!player.IsDead && !player.isTraitor && player.roomPosition != Player.STARTING_ROOM_ID)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    #endregion

    #region Action Points

    /// <summary>
    /// 
    /// Roll a random dice roll to determine how many action points a player will have to move with this turn
    /// 
    /// </summary>
    /// <returns>The rolled number of action points</returns>
    public static int RollActionPoints()
    {
        int actionPointRoll = 0;

        //For loop loops through each "dice"
        for (int dice = 0; dice < AP_NUM_DICE; dice++)
        {
            //Adds the random roll of the "dice" to the total roll
            //Need to add 1 since random.range is inclusive of lower bound and exclusive of upper bound
            actionPointRoll += UnityEngine.Random.Range(0, AP_DICE_SIDES) + 1;
        }

        return actionPointRoll;
    }

    /// <summary>
    /// 
    /// Exchanges a players remaining action points for scrap
    /// 
    /// </summary>
    /// <param name="playerID">The player rolling the action points</param>
    /// <param name="remainingPoints">The remaining number of action points the player has</param>
    public void ExchangeActionPoints(int playerID, int remainingPoints)
    {
        players[playerID].scrap += (int)Mathf.Round(remainingPoints * AP_CONVERSION);
    }

    #endregion

    #region Movement Handling

    /// <summary>
    /// 
    /// Detect if one of the rooms is clicked on for the player to move to based on a raycast system.
    /// 
    /// </summary>
    private void ClickRoom()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.root.gameObject == roomList && hit.transform.tag != "Bridges")
                {
                    playerGoalIndex = hit.transform.parent.gameObject.GetComponent<LinkedNodes>().index;
                    roomSelection = false;
                    GetActivePlayer().roomPosition = playerGoalIndex;
                    playerMoving = true;
                }
            }
        }
    }

    private void ClientClickRoom() {

        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.root.gameObject == roomList && hit.transform.tag != "Bridges" && hit.transform.tag != "Unavailable") {

                    playerGoalIndex = hit.transform.parent.gameObject.GetComponent<LinkedNodes>().index;
                    roomSelection = false;
                    Server.Instance.SendRoomChoiceForCost(playerGoalIndex);
                }
            }
        }

    }

    #endregion
}
