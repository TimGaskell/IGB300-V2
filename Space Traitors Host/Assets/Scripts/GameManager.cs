using System;
using System.Linq;
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

    //A default player ID for dummy players (i.e. when a target is not needed). -1 should never be a valid player ID so it used here
    public const int DEFAULT_PLAYER_ID = -1;

    //Used for generating default player information if loading into a scene later than the lobby
    private const int DEFAULT_NUM_PLAYERS = 4;
    private static readonly string[] DEFAULT_NAMES = { "BruteTest", "ButlerTest", "ChefTest", "EngineerTest", "SingerTest", "TechieTest" };
    private static readonly Character.CharacterTypes[] CHARACTER_TYPES = { Character.CharacterTypes.Brute, Character.CharacterTypes.Butler, Character.CharacterTypes.Chef,
        Character.CharacterTypes.Engineer, Character.CharacterTypes.Singer, Character.CharacterTypes.Techie };

    public enum SpecScores { Default, Brawn, Skill, Tech, Charm };

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

    //The number of components is always equal to the number of players (if increasing number of components in a game, change here)
    public int NumComponents { get { return numPlayers; } }

    private float aiPower;
    public float AIPower { get { return aiPower; } private set { aiPower = Math.Min(MAX_POWER, value); } }
    public float aiPowerChange;
    private int aiTargetScore;

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

    //The modifier for spec scores when one player counters another in combat
    private const int COUNTER_MOD = 2;

    public GameObject roomList;

    public int installedComponents;

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
            else if (scene.name == "Game LevelV2")
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
        //Reset variables to their default state
        activePlayer = 0;
        installedComponents = 0;

        aiPower = 0;
        aiTargetScore = BASE_TARGET_SCORE;
        traitorDelay = false;

        //Sets a default player traitor and target
        newTraitor = DEFAULT_PLAYER_ID;
        targetPlayer = DEFAULT_PLAYER_ID;

        roomList = GameObject.FindWithTag("RoomList");
        roomList.GetComponent<ChoiceRandomiser>().ChoiceSetup();

        //foreach (Player player in players)
        //{
        //    Debug.Log(player.playerID);
        //    Debug.Log(player.playerName);
        //    Debug.Log(player.CharacterType);
        //}
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

    /// <summary>
    /// 
    /// Determines if a player was successful in their spec challenge or not
    /// 
    /// </summary>
    /// <param name="PlayerScore">The player performing the spec challenge's relevant spec score. Also the attacker's score in a combat</param>
    /// <param name="targetScore">The target score of the spec challenge, or the defender's relevant spce score</param>
    /// <returns>True if the player suceeded. False otherwise</returns>
    public bool PerformSpecChallenge(int PlayerScore, int targetScore)
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
        players[playerOrder[activePlayer]].Character = new Character(characterType);
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
        if (activePlayer == numPlayers)
        {
            activePlayer = 0;
            ActivateSurge();
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
            float basePower = BASE_POWER_MOD / numPlayers;
            float playerPower = PLAYER_POWER_MOD * (TotalCorruption(true) / numPlayers);

            AIPower += basePower + playerPower + aiPowerChange;
            aiPowerChange = 0;
        }
        else
        {
            //Chooses a random target if the AI Power is at 100%. To update the UI will need to do a check in the UI Manager
            //to see if the target is not the default case
            targetPlayer = AIChooseTarget();
        }

        if (IsTraitorSelected())
        {
            ChooseTraitor();
        }
    }

    /// <summary>
    /// 
    /// Calculates the sum of corruption from all players. Can include only players which are not traitors or all players in the sum as desired.
    /// 
    /// </summary>
    /// <param name="includeTraitor">Whether or not to consider the corruption of traitor characters when calculating the total</param>
    /// <returns>The sum of corruption</returns>
    private int TotalCorruption(bool includeTraitor)
    {
        int totalCorruption = 0;

        foreach (Player player in players)
        {
            //Do not want to include a players corruption if traitor corruption is not to be consider
            //and the player is a traitor
            if (!(!includeTraitor && player.isTraitor))
            {
                totalCorruption += player.corruption;
            }
        }

        return totalCorruption;
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
        if (!TraitorCountCheck())
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
        float chanceCounter = 0;
        float randomChance = UnityEngine.Random.Range(0.0f, 100.0f);

        //Want the total corruption without traitors as a scaling for each players corruption
        int totalCorruption = TotalCorruption(false);

        foreach (Player player in players)
        {
            //Cannot consider players which are not traitors, so will ignore them in the summation
            if (!player.isTraitor)
            {
                //Add the player's proability, then determine if the random number falls in that probability range.
                //If it does, that player is to be selected as traitor
                chanceCounter += player.corruption / totalCorruption;
                if (randomChance <= chanceCounter)
                {
                    players[player.playerID].isTraitor = true;
                    return player.playerID;
                }
            }
        }

        //Provides a dummy output for the function. Should never reach this point
        return DEFAULT_PLAYER_ID;
    }

    /// <summary>
    /// 
    /// The AI chooses a random player target for its attacks
    /// 
    /// </summary>
    /// <returns>The random target's player index</returns>
    private int AIChooseTarget()
    {
        return UnityEngine.Random.Range(0, numPlayers);
    }

    /// <summary>
    /// 
    /// Determines if the player wins a combat against the AI attacking it. Returns true if the player wins, false otherwise
    /// 
    /// </summary>
    /// <param name="specScore">The name of the spec score the player wants to use against the AI</param>
    /// <returns>True if the player wins the combat. False otherwise</returns>
    private bool AIAttackPlayer(SpecScores specScore)
    {
        bool playerWin;

        int targetSpecScore = ObtainSpecScore(players[targetPlayer], specScore);

        //Determines if the target player wins the combat against the AI. If they do, there is no change. However if they lose, then
        //the target player loses a life point
        if (PerformSpecChallenge(targetSpecScore, aiTargetScore))
        {
            playerWin = true;
        }
        else
        {
            playerWin = false;
            players[targetPlayer].lifePoints -= 1;
        }

        //The AI attacks should get harder to beat every round, so this will increment after an attack (regardless of the player winning or losing the combat)
        aiTargetScore += AI_TARGET_INCREASE;
        //Resets target player back to the default case
        targetPlayer = DEFAULT_PLAYER_ID;

        return playerWin;
    }
    #endregion

    #region Combat Handling and Traitor Victory Conditions

    /// <summary>
    /// 
    /// Checks if combat is viable between two players based on them being in the same room as well either the attacker being a traitor
    /// or the defender being revealed as the traitor
    /// 
    /// </summary>
    /// <param name="attackerID">The ID of the attacking player</param>
    /// <param name="defenderID">The ID of the defending player</param>
    /// <returns>If combat is viable between the two players, returns true. Otherwise, returns false</returns>
    public bool CheckCombat(int attackerID, int defenderID)
    {
        Player attackingPlayer = GetPlayer(attackerID);
        Player defendingPlayer = GetPlayer(defenderID);

        //Checks if the players are in the same room as well as if either the attacking player is a traitor, or the defending player has been revealed as a traitor
        return attackingPlayer.roomPosition == defendingPlayer.roomPosition && (attackingPlayer.isTraitor || defendingPlayer.isRevealed);
    }

    /// <summary>
    /// 
    /// Performs a combat scenario between two players, considering the counters for each player based on the given spec score
    /// 
    /// </summary>
    /// <param name="attackerID">The playerID of the attacker</param>
    /// <param name="attackerSpec">The name of the spec score the attacker is using</param>
    /// <param name="defenderID">The playerID of the defender</param>
    /// <param name="defenderSpec">The name of the spec score the attacker is using</param>
    /// <returns>If the attacker wins the combat, returns true. If the defender wins, returns false</returns>
    public bool PerformCombat(int attackerID, SpecScores attackerSpec, int defenderID, SpecScores defenderSpec)
    {
        Player attackingPlayer = GetPlayer(attackerID);
        Player defendingPlayer = GetPlayer(defenderID);

        int attackerScore = ObtainSpecScore(attackingPlayer, attackerSpec);
        int defenderScore = ObtainSpecScore(defendingPlayer, defenderSpec);

        //If the attacking player is a traitor but has not been revealed, that player is revealed as the traitor
        if (attackingPlayer.isTraitor && !attackingPlayer.isRevealed)
        {
            players[attackerID].isRevealed = true;
        }

        //Below statements determine the victory of a combat
        //First set of statements consider whether the attacker counters the defender, or vice versa, or if there are no counters and applys modifiers to the relevant spec scores accordingly
        //Next set of statements determines who wins the combat based on the relevant spec scores, updating life points and returning outcome accordingly
        if (DetermineCounter(attackerSpec, defenderSpec))
        {
            if (PerformSpecChallenge(attackerScore * COUNTER_MOD, defenderScore))
            {
                players[defenderID].lifePoints -= 1;
                return true;
            }
            else
            {
                players[attackerID].lifePoints -= 1;
                return false;
            }
        }
        else if (DetermineCounter(defenderSpec, attackerSpec))
        {
            if (PerformSpecChallenge(attackerScore, defenderScore * COUNTER_MOD))
            {
                players[defenderID].lifePoints -= 1;
                return true;
            }
            else
            {
                players[attackerID].lifePoints -= 1;
                return false;
            }
        }
        else
        {
            if (PerformSpecChallenge(attackerScore, defenderScore))
            {
                players[defenderID].lifePoints -= 1;
                return true;
            }
            else
            {
                players[attackerID].lifePoints -= 1;
                return false;
            }

        }
    }

    /// <summary>
    /// 
    /// Obtains the relevant spec score to be utilised from a particular player
    /// 
    /// </summary>
    /// <param name="player">The player who is being tested</param>
    /// <param name="specScore">The name of the spec score to be utilised</param>
    /// <returns>The value of the relevant spec score for that player</returns>
    private int ObtainSpecScore(Player player, SpecScores specScore)
    {
        switch (specScore)
        {
            case (SpecScores.Brawn):
                return player.ScaledBrawn;
            case (SpecScores.Skill):
                return player.ScaledSkill;
            case (SpecScores.Tech):
                return player.ScaledTech;
            case (SpecScores.Charm):
                return player.ScaledCharm;
            default:
                throw new NotImplementedException("Not a valid Spec Score");
        }
    }

    private class SpecComparison
    {
        public SpecScores Spec1 { get; set; }
        public SpecScores Spec2 { get; set; }
    }

    private SpecComparison[] counters = 
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

    #endregion

    #region Component Handling

    /// <summary>
    /// 
    /// Checks if a particular player can install a component or not
    /// 
    /// </summary>
    /// <param name="playerID">The ID of player to be checked</param>
    /// <returns>True if the player has a component, false otherwise</returns>
    public bool CanInstallComponent(int playerID)
    {
        return players[playerID].hasComponent;
    }

    /// <summary>
    /// 
    /// Installs a component and checks if this is the last component to be installed. If it is returns true.
    /// Otherwise returns false.
    /// 
    /// </summary>
    /// <param name="playerID">The ID of the player installing the component</param>
    /// <returns>If this is the last component to be installed, returns true</returns>
    public bool InstallComponent(int playerID)
    {
        players[playerID].hasComponent = false;
        installedComponents += 1;

        //The number of components in the game is equal to the number of players
        return installedComponents == NumComponents;
    }

    #endregion
}
