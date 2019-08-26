using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

/* 
Most if not all networking from the host's (computer's) side is handled within this script,
listening if a client connects or disconnects and reacts accordingly, as well as handling orders to
the players based on inputs. 
Also, sets up the game.
*/

public class IPManager
{
    public static string GetIP(ADDRESSFAM Addfam)
    {
        //Return null if ADDRESSFAM is Ipv6 but Os does not support it
        if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
        {
            return null;
        }

        string output = "";

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif 
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    //IPv4
                    if (Addfam == ADDRESSFAM.IPv4)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }

                    //IPv6
                    else if (Addfam == ADDRESSFAM.IPv6)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
        }
        return output;
    }

}

public enum ADDRESSFAM
{
    IPv4, IPv6
}

public class Server : MonoBehaviour
{
    //Networking variables
    public static Server Instance { get; set; }
    private byte reliableChannel;
    private int hostID;
    private int connectionID;
    private int webHostID;
    public bool connected;

    private const int maxUser = 100;
    private const int port = 26000;
    private const int webPort = 26001;
    private const int byteSize = 1024;

    private bool isStarted = false;
    private byte error;
    public string serverIP = IPManager.GetIP(ADDRESSFAM.IPv4);
    private bool isServer = false;

    //Other
    public AudioSource connectSound;
    public List<GameObject> players = new List<GameObject>();
    private List<GameObject> ElminiatedPlayers = new List<GameObject>();
    private List<GameObject> playersRemoved = new List<GameObject>();
    public GameObject[] ScrapTotals;
    public GameObject[] Components;
    private GameObject setter;
    private Scene currentScene;
    public Text connectText;
    public Sprite[] portraits;

    public int[] playerIDs = new int[6];
    public int playersJoined;
    private int portraitID = -1;
    public int tempPlayerID;
    private string sceneName;

    //Player Variables
    private int PlayerActionPoints;
    private GameManager.SpecScores attackerSpec;
    private GameManager.SpecScores defenderSpec;






    // Use this for initialization
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);


    }

    #region Connection Handling
    public void HostInitialise()
    {
        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        reliableChannel = config.AddChannel(QosType.Reliable);
        isServer = true;

        HostTopology topo = new HostTopology(config, maxUser);

        //Server only code
        hostID = NetworkTransport.AddHost(topo, port, null);
        webHostID = NetworkTransport.AddWebsocketHost(topo, port, null);

        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", port, webPort));
        isStarted = true;
    }

    public void ClientInitialise()
    {
        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        reliableChannel = config.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(config, maxUser);

        //Client only code
        hostID = NetworkTransport.AddHost(topo, 0);

#if !UNITY_WEBGL && UNITY_EDITOR
        //Standalone Client
        Debug.Log(serverIP);
        connectionID = NetworkTransport.Connect(hostID, serverIP, port, 0, out error);
        Debug.Log(string.Format("Connecting from standalone"));
#else
        //Web Client
        connectionID = NetworkTransport.Connect(hostID, serverIP, port, 0, out error);
        Debug.Log(string.Format("Connecting from Web"));
#endif

        Debug.Log(string.Format("Attempting to connect on {0}...", serverIP));
        isStarted = true;
    }



    public void ShutDown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }


    // Update is called once per frame
    void Update()
    {


        //Keep track of the current scene
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;


        //NetworkMessaging
        if (isServer)
        {
            ServerUpdateMessagePump();
        }
        ClientUpdateMessagePump();
    }



    private void ServerUpdateMessagePump()
    {
        if (!isStarted)
        {
            return;
        }
        int recHostID;  //checks whether this is from Web or standalone
        int connectionID; //checks which user is sending info
        int channelID;  //checks lane infor is being sent from

        byte[] recBuffer = new byte[byteSize];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostID, out connectionID, out channelID, recBuffer, byteSize, out dataSize, out error);
        switch (type)
        {
            case NetworkEventType.Nothing:
                break;

            //When user connects to game
            case NetworkEventType.ConnectEvent:
                connectSound.Play();
                //Loop through to find a player not already connected, and assign them their ID
                foreach (GameObject player in players)
                {
                    if (player.GetComponent<Player>().isConnected == false)
                    {
                        if (sceneName == "ServerLobby")
                            LobbyConnectOrDisconnect(player, true, connectionID, true);
                        else if (sceneName == "GameLevel")
                            GameConnectOrDisconnect(player, true, connectionID);
                        Debug.Log(player.name + " has connected through host " + recHostID);
                        break;
                    }
                }
                break;

            //When user disconnects from game
            case NetworkEventType.DisconnectEvent:
                //Loop through to find player that is disconnecting, based on their ID
                foreach (GameObject player in players)
                {
                    if (player.GetComponent<Player>().playerID == connectionID)
                    {
                        if (sceneName == "LobbyLan")
                            //Reset player variables
                            LobbyConnectOrDisconnect(player, false, 0, false);
                        else if (sceneName == "GameLevel")
                            GameConnectOrDisconnect(player, false, 0);
                        Debug.Log(player.name + " has disconnected");
                        break;
                    }
                }
                Debug.Log(connectionID + " has disconnected");
                break;

            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMessage msg = (NetMessage)formatter.Deserialize(ms);

                OnData(connectionID, channelID, recHostID, msg);
                break;

            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;
        }
    }

    private void ClientUpdateMessagePump()
    {

        if (!isStarted)
        {
            return;
        }
        int recHostID;  //checks whether this is from Web or standalone
        int connectionID; //checks which user is sending info
        int channelID;  //checks lane infor is being sent from

        byte[] recBuffer = new byte[byteSize];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostID, out connectionID, out channelID, recBuffer, byteSize, out dataSize, out error);
        switch (type)
        {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                connected = true;
                Debug.Log(string.Format("Connected to server"));
                //Disable the connect button so player can't have multiple instances
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("You were disconnected"));
                break;

            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMessage msg = (NetMessage)formatter.Deserialize(ms);

                OnData(connectionID, channelID, recHostID, msg);

                break;

            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;
        }
    }

    #endregion 

    private void OnData(int conID, int chanID, int rHostID, NetMessage msg)
    {
        switch (msg.OperationCode)
        {
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.PlayerDetails:
                AssignPlayerDetails(conID, chanID, rHostID, (PlayerDetails)msg);
                break;
            case NetOP.CharacterSelection:
                AssignCharacterSelection(conID, chanID, rHostID, (CharacterSelection)msg);
                break;
            case NetOP.AbilityUsage:
                AbilityUsed(conID, chanID, rHostID, (AbilityUsage)msg);
                break;
            case NetOP.ActionPoints:
                ActionPoints(conID, chanID, rHostID, (ActionPoints)msg);
                break;
            case NetOP.Movement:
                AssignRoomMovement(conID, chanID, rHostID, (Movement)msg);
                break;
            case NetOP.SelectedChoice:
                ChoiceSelection(conID, chanID, rHostID, (SelectedChoice)msg);
                break;
            case NetOP.InventoryChanges:
                Inventory(conID, chanID, rHostID, (InventoryChanges)msg);
                break;
            case NetOP.SpecSelection:
                SpecSelection(conID, chanID, rHostID, (SpecSelection)msg);
                break;
            case NetOP.CombatAttackingTarget:
                SpecCombat(conID, chanID, rHostID, (CombatAttackingTarget)msg);
                break;
            case NetOP.ItemSelection:
                ItemSelection(conID, chanID, rHostID, (ItemSelection)msg);
                break;
            case NetOP.InstallComponent:
                InstallComponent(conID, chanID, rHostID, (InstallComponent)msg);
                break;
            case NetOP.ChangeScenes:
                GetSceneChange(conID, chanID, rHostID, (SceneChange)msg);
                break;
            case NetOP.IsActivePlayer:
                AmActivePlayer(conID, chanID, rHostID, (IsActivePlayer)msg);
                break;
            case NetOP.ChangeCharacter:
                NeedToChangeCharacter(conID, chanID, rHostID, (ChangeCharacter)msg);
                break;
            case NetOP.NewPhase:
                NewPhase(conID, chanID, rHostID, (NewPhase)msg);
                break;

        }

    }
    //Not sure which sendClient to use
    public void SendClient(int recHost, int conID, NetMessage msg)
    {
        //This is where data is held
        byte[] buffer = new byte[byteSize];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        if (recHost == 0)
        {
            NetworkTransport.Send(hostID, conID, reliableChannel, buffer, byteSize, out error);
        }
        else
        {
            NetworkTransport.Send(webHostID, conID, reliableChannel, buffer, byteSize, out error);
        }

    }



    //This needs to be updated

    private List<GameObject> playerArray()
    {

        if ((sceneName == "LobbyLan") || (sceneName == "Character Select"))
            return players;
        //else if (sceneName == "server")
        //  return playerStorage.GetComponent<RoundManager>().playersInGame;
        else
        {
            Debug.Log("Could not get the correct scene");
            return null;
        }

    }

    private void LobbyConnectOrDisconnect(GameObject player, bool connect, int conID, bool imageEnable)
    {
        player.GetComponent<Player>().isConnected = connect;
        player.GetComponent<Player>().playerID = conID;

    }

    private void GameConnectOrDisconnect(GameObject player, bool connect, int conID)
    {
        player.GetComponent<Player>().isConnected = connect;
        player.GetComponent<Player>().playerID = conID;
    }

    //This needs to be updated
    private void SetPortraits()
    {
        setter = GameObject.FindGameObjectWithTag("Setter");
        for (int i = 0; i < playerArray().Count; i++)
        {
            switch (players[i].GetComponent<PlayerConnect>().characterName)
            {
                case "Brute":
                    portraitID = 0;
                    break;

                case "Butler":
                    portraitID = 1;
                    break;

                case "Singer":
                    portraitID = 2;
                    break;

                case "Techie":
                    portraitID = 3;
                    break;

                case "Engineer":
                    portraitID = 4;
                    break;

                case "Chef":
                    portraitID = 5;
                    break;
            }
            if (portraitID >= 0)
            {
                setter.GetComponent<ImageSetter>().images[i].sprite = portraits[portraitID];
            }


        }

    }

    //This needs to be updated
    public void StartGame() //This is called when a game is started in lobby
    {
        //TODO: cannot start game unless at least 3 (1 for purposes of testing) players are connected

        //If a player hasn't been assigned to one of the player objects, remove it from the server's array of players
        for (int k = 0; k < players.Count; k++)
        {
            if (!players[k].GetComponent<Player>().isConnected)
            {
                playersRemoved.Add(players[k]);
            }
        }



        if (playersRemoved != null)
        {
            foreach (GameObject player in playersRemoved)
            {
                players.Remove(player);
                Destroy(player);
            }
        }

        //Get the number of players based on how many remain
        playersJoined = players.Count;
        int i = 0;
        foreach (GameObject player in players)
        {
            playerIDs[i] = player.GetComponent<Player>().playerID;
            player.transform.parent = null;
            i++;
        }

        //Send message to every player's client to move onto next scene
        SendChangeScene("Character Selection");
        //Change to the character select
        SceneManager.LoadScene("Server Character Selection");
    }

    public void ClientNextScene()
    {
        foreach (GameObject player in players)
        {
            tempPlayerID = player.GetComponent<Player>().playerID;
            SendChangeScene("Lobby");
        }
    }
    public void SendClient(NetMessage msg)
    {
        //This is where data is held
        byte[] buffer = new byte[byteSize];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        int connectionID = tempPlayerID;

        Debug.Log("sent");
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, byteSize, out error);

    }

    public void SendServer(NetMessage msg)
    {
        //This is where data is held
        byte[] buffer = new byte[byteSize];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        Debug.Log("sent");
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, byteSize, out error);

    }

    #region Server Sent Messages

    public void SendChangeScene(string SceneName)
    {

        SceneChange scene = new SceneChange();
        scene.SceneName = SceneName;
        foreach (GameObject player in players)
        {
            tempPlayerID = player.GetComponent<Player>().playerID;
            SendClient(scene);
        }
    }
    public void SendActivePlayer(int player)
    {

        IsActivePlayer Activeplayer = new IsActivePlayer();
        tempPlayerID = player;

        SendClient(Activeplayer);
    }

    public void SendChangeCharacter(int player, bool istaken)
    {

        ChangeCharacter change = new ChangeCharacter();
        tempPlayerID = player;

        change.AlreadySelected = istaken;


        SendClient(change);

    }


    public void SendCharacterBaseStats(int player, int brawn, int charm, int skill, int tech, string AbilityName, string AbilityDescription)
    {

        CharacterInformation information = new CharacterInformation();
        tempPlayerID = player;

        information.Basebrawn = brawn;
        information.Basecharm = charm;
        information.Baseskill = skill;
        information.Basetech = tech;
        information.name = AbilityName;
        information.AbilityDescription = AbilityDescription;

        SendClient(information);

    }

    public void SendAbilityInformation(int player)
    {

        AbilityInformation abilityInformation = new AbilityInformation();
        tempPlayerID = player;
        int numAbilities = Player.NUM_ABILITIES;

        abilityInformation.AbilityTypes = new int[numAbilities];
        abilityInformation.CheckCorruption = new bool[numAbilities];
        abilityInformation.CheckScrap = new bool[numAbilities];

        for (int abilityID = 0; abilityID < numAbilities; abilityID++)
        {
            Ability ability = GameManager.instance.GetPlayer(player).GetAbility(abilityID);

            abilityInformation.AbilityTypes[abilityID] = (int)ability.abilityType;
            abilityInformation.CheckCorruption[abilityID] = ability.CheckCorruption();
            abilityInformation.CheckScrap[abilityID] = ability.CheckScrap();
        }

        SendClient(abilityInformation);

    }

    public void SendAvailableRooms(int player, List<int> Roomnumbers)
    {

        AvailableRooms rooms = new AvailableRooms();
        tempPlayerID = player;

        rooms.AvailableRoomsIDs = Roomnumbers;

        SendClient(rooms);

    }

    public void SendRoomChoices(int player, int roomIndex)
    {

        RoomChoices choices = new RoomChoices();
        tempPlayerID = player;

        int choicesPerRoom = ChoiceRandomiser.CHOICES_PER_ROOM;

        Room selectedRoom = GameManager.instance.GetRoom(roomIndex);

        choices.ChoiceNames = new string[choicesPerRoom];
        choices.SuccessTexts = new string[choicesPerRoom];
        choices.FailTexts = new string[choicesPerRoom];
        choices.IsAvailables = new int[choicesPerRoom];
        choices.SpecScores = new int[choicesPerRoom];
        choices.SuccessChances = new float[choicesPerRoom];

        for (int choiceIndex = 0; choiceIndex < choicesPerRoom; choiceIndex++)
        {
            Choice choice = selectedRoom.roomChoices[choiceIndex];

            choices.ChoiceNames[choiceIndex] = choice.choiceName;
            choices.SuccessTexts[choiceIndex] = choice.SuccessText();
            choices.FailTexts[choiceIndex] = choice.FailText();
            choices.IsAvailables[choiceIndex] = (int)choice.IsAvailable();
            choices.SpecScores[choiceIndex] = (int)choice.specChallenge;
            //If the choice is not a spec challenge, choice's target score is 0 which will cause
            //a math error in GameManager.SpecChallengeChance, so need to be specific for those
            //cases (since this will not be displayed regardless can arbitrarily set to 0).
            if (choice.specChallenge == GameManager.SpecScores.Default)
            {
                choices.SuccessChances[choiceIndex] = 0;
            }
            else
            {
                float playerScore = GameManager.instance.GetPlayer(player).GetScaledSpecScore(choice.specChallenge);
                choices.SuccessChances[choiceIndex] = GameManager.SpecChallengeChance(playerScore, choice.targetScore);
            }
        }

        choices.AttackablePlayers = GameManager.instance.CheckCombat();

        SendClient(choices);
    }

    public void SendSpecChallenge(int player, bool specChallengeResult)
    {

        SpecChallenge challenge = new SpecChallenge();
        tempPlayerID = player;

        challenge.result = specChallengeResult;
        SendClient(challenge);

    }

    public void SendServerPlayerInformation(int player, int Scaledbrawn, int Scaledskill, int Scaledcharm, int Scaledtech, int scrap, float corruption, int lifepoints, List<string> EquippedItems, List<string> UnEquippedItems, bool isTraitor)
    {

        PlayerInformation playerInformation = new PlayerInformation();
        tempPlayerID = player;
        //does not send base stats, use other method to do so
        playerInformation.scaledbrawn = Scaledbrawn;
        playerInformation.scaledskill = Scaledskill;
        playerInformation.scaledcharm = Scaledcharm;
        playerInformation.scaledtech = Scaledtech;
        playerInformation.scrap = scrap;
        playerInformation.corruption = lifepoints;
        playerInformation.EquippedItems = EquippedItems;
        playerInformation.UnEquippedItems = UnEquippedItems;
        playerInformation.isTraitor = isTraitor;

        SendClient(playerInformation);
    }

    public void SendIsTraitor(int player)
    {

        TraitorSelection traitor = new TraitorSelection();
        tempPlayerID = player;

        SendClient(traitor);

    }

    public void SendSurge()
    {

        SurgeInformation surge = new SurgeInformation();
        surge.NewAiPower = GameManager.instance.AIPower;
        surge.PowerIncrease = GameManager.instance.AIPowerIncrease();

        foreach (GameObject player in players)
        {
            tempPlayerID = player.GetComponent<Player>().playerID;
            SendClient(surge);
        }
    }

    public void SendAIAttack(int player, int damage, int spec, string spectype)
    {

        AiAttacks ai = new AiAttacks();
        tempPlayerID = player;
        ai.damage = damage;
        ai.specAmount = spec;
        ai.spec = spectype;

        SendClient(ai);
    }

    public void SendCombatResolution(int player, bool winorLoose, int damagetaken, List<string> Inventory)
    {

        CombatResolution resolution = new CombatResolution();
        tempPlayerID = player;
        resolution.winbattle = winorLoose;
        resolution.damagetaken = damagetaken;
        resolution.LoserInventory = Inventory;

        SendClient(resolution);

    }

    /// <summary>
    /// 
    /// Message can be removed- handled through the SendRoomChoices message.
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Targets"></param>
    public void SendCombatAvailablity(int player, List<int> Targets)
    {

        CombatAvailability combat = new CombatAvailability();
        tempPlayerID = player;

        combat.Players = Targets;

        SendClient(combat);
    }

    public void SendBeingAttacked(int player, int attackerID, int defenderID)
    {

        CombatBeingAttacked attacked = new CombatBeingAttacked();
        tempPlayerID = player;

        attacked.AttackerID = attackerID;
        attacked.DefenderID = defenderID;

        SendClient(attacked);

    }

    public void SendPlayerElimination(int player)
    {

        PlayerElimination elimination = new PlayerElimination();
        tempPlayerID = player;

        SendClient(elimination);
    }

    public void SendNonTraitorVictory()
    {

        InnocentVictory innocent = new InnocentVictory();
        foreach (GameObject player in players)
        {
            tempPlayerID = player.GetComponent<Player>().playerID;
            SendClient(innocent);
        }
    }

    public void SendTraitorVictory(int winnerID)
    {
        TraitorVictory traitor = new TraitorVictory();
        traitor.WinnerID = winnerID;
        foreach (GameObject player in players)
        {
            tempPlayerID = player.GetComponent<Player>().playerID;
            SendClient(traitor);
        }
    }

    public void SyncPlayerData(int playerID)
    {
        PlayerDataSync playerData = new PlayerDataSync();
        tempPlayerID = playerID;

        Player player = GameManager.instance.GetPlayer(playerID);

        playerData.Scrap = player.scrap;
        playerData.Corruption = player.Corruption;
        playerData.HasComponent = player.hasComponent;
        playerData.LifePoints = player.lifePoints;
        playerData.MaxLifePoints = player.maxLifePoints;
        playerData.ScaledBrawn = player.GetScaledSpecScore(GameManager.SpecScores.Brawn);
        playerData.ScaledSkill = player.GetScaledSpecScore(GameManager.SpecScores.Skill);
        playerData.ScaledTech = player.GetScaledSpecScore(GameManager.SpecScores.Tech);
        playerData.ScaledCharm = player.GetScaledSpecScore(GameManager.SpecScores.Charm);
        playerData.ModBrawn = player.GetModdedSpecScore(GameManager.SpecScores.Brawn);
        playerData.ModSkill = player.GetModdedSpecScore(GameManager.SpecScores.Skill);
        playerData.ModTech = player.GetModdedSpecScore(GameManager.SpecScores.Tech);
        playerData.ModCharm = player.GetModdedSpecScore(GameManager.SpecScores.Charm);

        foreach (Item item in player.items)
        {
            playerData.Items.Add((int)item.ItemType);
            playerData.ItemEquipped.Add(item.isEquipped);
        }

        SendClient(playerData);
    }

    public void SendAbilityActivated(int playerID, Ability.AbilityTypes abilityType, bool isTraitor)
    {
        AbilityActivated abilityActivated = new AbilityActivated();
        tempPlayerID = playerID;

        abilityActivated.AbilityType = (int)abilityType;
        abilityActivated.IsTraitor = isTraitor;

        SendClient(abilityActivated);
    }

    public void SendComponentInstalled(int installerID, bool successfulInstall)
    {
        foreach (GameObject player in players)
        {
            tempPlayerID = player.GetComponent<Player>().playerID;

            bool allComponentsCheck = GameManager.instance.CheckInstalledComponents();

            if (tempPlayerID == installerID)
            {
                ComponentInstalled componentInstalled = new ComponentInstalled();

                componentInstalled.SuccessfulInstall = successfulInstall;
                componentInstalled.AllComponentsInstalled = allComponentsCheck;

                SendClient(componentInstalled);
            }
            else
            {
                NumComponentsInstalled numComponentsInstalled = new NumComponentsInstalled();

                numComponentsInstalled.InstalledComponents = GameManager.instance.installedComponents;
                numComponentsInstalled.AllComponentsInstalled = allComponentsCheck;

                SendClient(numComponentsInstalled);
            }
        }
    }

    #endregion

    #region Client Received Messages

    private void GetSceneChange(int conID, int chanID, int rHostID, SceneChange scene)
    {

        SceneManager.LoadScene(scene.SceneName);

    }

    private void AmActivePlayer(int conID, int chanID, int rHostID, IsActivePlayer isActive)
    {

        if (SceneManager.GetActiveScene().name == "Character Selection")
        {

            GameObject Canvas = GameObject.Find("Canvas");
            Canvas.GetComponent<CharacterSelectUIManager>().DisplayActivePlayer();
        }
    }

    private void NeedToChangeCharacter(int conID, int chanID, int rHostID, ChangeCharacter character)
    {

        GameObject Canvas = GameObject.Find("Canvas");

        if (character.AlreadySelected == false)
        {

            Canvas.GetComponent<CharacterSelectUIManager>().EndSelection();
        }
        else
        {
            Debug.Log("Choose another Character");
        }


    }

    private void GetCharacterInformation(int conID, int chanID, int rHostID, CharacterInformation information)
    {



    }

    private void SyncClientData(int conID, int chanID, int rHostID, PlayerDataSync playerData)
    {
        ClientManager.instance.scrap = playerData.Scrap;
        ClientManager.instance.corruption = playerData.Corruption;
        ClientManager.instance.hasComponent = playerData.HasComponent;
        ClientManager.instance.lifePoints = playerData.LifePoints;
        ClientManager.instance.maxLifePoints = playerData.MaxLifePoints;
        ClientManager.instance.scaledBrawn = playerData.ScaledBrawn;
        ClientManager.instance.scaledSkill = playerData.ScaledSkill;
        ClientManager.instance.scaledTech = playerData.ScaledTech;
        ClientManager.instance.scaledCharm = playerData.ScaledCharm;
        ClientManager.instance.modBrawn = playerData.ModBrawn;
        ClientManager.instance.modSkill = playerData.ModSkill;
        ClientManager.instance.modTech = playerData.ModTech;
        ClientManager.instance.modCharm = playerData.ModCharm;

        for (int itemIndex = 0; itemIndex < playerData.Items.Count; itemIndex++)
        {
            ClientManager.instance.inventory.Add(new Item((Item.ItemTypes)playerData.Items[itemIndex]));
            ClientManager.instance.inventory[itemIndex].isEquipped = playerData.ItemEquipped[itemIndex];
        }
    }

    private void StoreRoomChoices(int conID, int chanID, int rHostID, RoomChoices roomChoices)
    {
        for (int choiceIndex = 0; choiceIndex < ChoiceRandomiser.CHOICES_PER_ROOM; choiceIndex++)
        {
            //Need to store values from this message on interaction manager. Variables are all set up however
            //need to figure out best way to communicate with interaction manager.
        }
    }

    private void GetAbilityInfo(int conID, int chanID, int rHostID, AbilityInformation abilityInformation)
    {
        for (int abilityID = 0; abilityID < Player.NUM_ABILITIES; abilityID++)
        {
            Ability ability = ClientManager.instance.GetAbilityInfo(abilityInformation.AbilityTypes[abilityID]);

            //Need to send this information to the UI Manager to display to the player
            //Displayed information would be the ability name, the scrap cost, the 
            //corruption cost (stored within ability) as well as the booleans in
            //abiltyInformation, CheckCorruptio and CheckScrap, which can determine
            //whether or not the ability can be selected
        }
    }

    private void ReceiveCombat(int conID, int chanID, int rHostID, CombatBeingAttacked beingAttacked)
    {
        //Need to display attacked and defender info to players and allow them to select spec score for combat
    }

    private void AbilityActivated(int conID, int chanID, int rHostID, AbilityActivated abilityActivated)
    {
        Ability.AbilityTypes abilityType = (Ability.AbilityTypes)abilityActivated.AbilityType;

        if (abilityType == Ability.AbilityTypes.Code_Inspection)
        {
            //Need to display that the ability is activated
            //In the case of code inspection, also need to display if the player is a traitor or not
        }
        else
        {
            //Need to display that the ability is activated
        }
    }

    /// <summary>
    /// 
    /// Message for the player who installed the component to recieve when a component is installed
    /// 
    /// </summary>
    private void GetComponentInstalled(int conID, int chanID, int rHostID, ComponentInstalled componentInstalled)
    {
        if (componentInstalled.SuccessfulInstall)
        {
            ClientManager.instance.componentsInstalled += 1;
            //Need to display to the player that a component has been installed

            if (componentInstalled.AllComponentsInstalled)
            {
                //Need to display to the player that all components have been installed and they can escape
            }
        }
        else
        {
            //Need to display to the player that they have been sabotaged and lost life points equal to
            //GameManager.COMBAT_DAMAGE.
        }
    }

    /// <summary>
    /// 
    /// Message for other players when a component is installed
    /// 
    /// </summary>
    private void GetNumComponentsInstalled(int conID, int chanID, int rHostID, NumComponentsInstalled numComponentsInstalled)
    {
        ClientManager.instance.componentsInstalled = numComponentsInstalled.InstalledComponents;

        if (numComponentsInstalled.AllComponentsInstalled)
        {
            if (ClientManager.instance.isTraitor)
            {
                //Need to display to the player that they need to stop the non-traitors from escaping
            }
            else
            {
                //Need to display to the player that all components are installed and they need to get to the escape shuttle
            }
        }
    }

    #endregion

    #region Client Sent Messages

    public void SendPlayerInformation(string playerName)
    {

        PlayerDetails details = new PlayerDetails();
        details.PlayerName = playerName;

        SendServer(details);
    }

    public void SendCharacterSelected(int Character)
    {

        CharacterSelection selection = new CharacterSelection();
        selection.SelectedCharacter = Character;

        SendServer(selection);
    }

    /// <summary>
    /// 
    /// Send a particular ability being used back to the server.
    /// Target ID is only required for SecretPaths, PowerBoost, Encouraging Song
    /// Muddle Sensors, Code Inspection and Supercharge as they can target other players.
    /// Should be set to GameManager.DEFAULT_PLAYER_ID if any other ability type
    /// 
    /// scanResource is only used for the Sensor Scan ability. Should be set to default
    /// case for any other ability type
    /// 
    /// </summary>
    public void SendAbilityUsed(Ability.AbilityTypes abilityType, int targetID, Ability.ScanResources scanResource)
    {

        AbilityUsage ability = new AbilityUsage();

        ability.AbilityType = (int)abilityType;

        switch (abilityType)
        {
            //Case for non-targeted abilities
            case (Ability.AbilityTypes.Sabotage):
                ability.TargetID = GameManager.DEFAULT_PLAYER_ID;
                ability.ScanResource = (int)Ability.ScanResources.Default;
                break;
            //Case for targetted abilities
            case (Ability.AbilityTypes.Secret_Paths):
            case (Ability.AbilityTypes.Power_Boost):
            case (Ability.AbilityTypes.Encouraging_Song):
            case (Ability.AbilityTypes.Muddle_Sensors):
            case (Ability.AbilityTypes.Code_Inspection):
            case (Ability.AbilityTypes.Supercharge):
                ability.TargetID = targetID;
                ability.ScanResource = (int)Ability.ScanResources.Default;
                break;
            //Case for resource targetted abilities
            case (Ability.AbilityTypes.Sensor_Scan):
                ability.TargetID = GameManager.DEFAULT_PLAYER_ID;
                ability.ScanResource = (int)scanResource;
                break;
        }

        SendServer(ability);
    }

    public void SendActionPoints()
    {

        ActionPoints actionPoints = new ActionPoints();

        SendServer(actionPoints);
    }

    public void SendRoomChoice(int Room)
    {

        Movement movement = new Movement();
        movement.SelectedRoom = Room;

        SendServer(movement);
    }

    public void SendSelectedChoice(int ChoiceId)
    {

        SelectedChoice choice = new SelectedChoice();
        choice.ChoiceId = ChoiceId;

        SendServer(choice);
    }

    public void SendInventoryChanges(List<string> EquiptItems, List<string> UnEquiptItems, List<string> DiscardItems)
    {

        InventoryChanges inventory = new InventoryChanges();
        inventory.equipedItems = EquiptItems;
        inventory.UnequipedItems = UnEquiptItems;
        inventory.discardItems = DiscardItems;

        SendServer(inventory);
    }

    public void SendSpecSelection(GameManager.SpecScores specScore)
    {

        SpecSelection specSelection = new SpecSelection();
        specSelection.SelectedSpec = (int)specScore;

        SendServer(specSelection);
    }

    public void SendCombat(int targetID)
    {

        CombatAttackingTarget combat = new CombatAttackingTarget();
        combat.TargetPlayer = targetID;

        SendServer(combat);
    }

    public void ItemSelection(string Item)
    {

        ItemSelection selection = new ItemSelection();
        selection.SelectedItem = Item;

        SendServer(selection);
    }

    public void InstallComponent()
    {

        InstallComponent installComponent = new InstallComponent();

        SendServer(installComponent);

    }

    public void SendNewPhase()
    {
        NewPhase newPhase = new NewPhase();

        SendServer(newPhase);
    }
    #endregion

    #region Server Received Messages

    private void AssignPlayerDetails(int conID, int chanID, int rHostID, PlayerDetails details)
    {

        Debug.Log("Recieved Player Name");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<Player>().playerID == conID)
            {
                //Find the correct player

                player.GetComponent<Player>().playerName = details.PlayerName;
                GameObject LobbyUiHandler = GameObject.Find("Canvas");
                LobbyUiHandler.GetComponent<LobbyUIManager>().AddPlayerNames();



            }
        }
    }
    private void AssignCharacterSelection(int conID, int chanID, int rHostID, CharacterSelection character)
    {

        foreach (GameObject player in players)
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {

                if (GameManager.instance.CheckCharacterSelected((Character.CharacterTypes)character.SelectedCharacter))
                {
                    SendChangeCharacter(player.GetComponent<Player>().playerID, true);
                }
                else
                {
                    GameObject canvas = GameObject.Find("Canvas");
                    canvas.GetComponent<ServerCharacterSelection>().tempCharacterType = (Character.CharacterTypes)character.SelectedCharacter;
                    canvas.GetComponent<ServerCharacterSelection>().UpdatePlayerCharacter();
                    GameManager.instance.SelectCharacter((Character.CharacterTypes)character.SelectedCharacter);
                    SendChangeCharacter(player.GetComponent<Player>().playerID, false);
                    SendActivePlayer(GameManager.instance.GetActivePlayer().playerID);
                }

            }
        }
    }
    private void AbilityUsed(int conID, int chanID, int rHostID, AbilityUsage ability)
    {

        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {

                Ability.AbilityTypes abilityType = (Ability.AbilityTypes)ability.AbilityType;
                Ability selectedAbility = GameManager.instance.GetActivePlayer().GetAbility(abilityType);
                //Unless the ability is code inspection, state of isTraitor is irrelevant, so sets to dummy case
                bool isTraitor = false;

                switch (abilityType)
                {
                    case (Ability.AbilityTypes.Sabotage):
                        selectedAbility.Activate();
                        break;
                    case (Ability.AbilityTypes.Secret_Paths):
                    case (Ability.AbilityTypes.Power_Boost):
                    case (Ability.AbilityTypes.Encouraging_Song):
                    case (Ability.AbilityTypes.Muddle_Sensors):
                    case (Ability.AbilityTypes.Supercharge):
                        selectedAbility.Activate(ability.TargetID);
                        break;
                    case (Ability.AbilityTypes.Code_Inspection):
                        selectedAbility.Activate(ability.TargetID, out isTraitor);
                        break;
                    case (Ability.AbilityTypes.Sensor_Scan):
                        selectedAbility.Activate((Ability.ScanResources)ability.ScanResource);
                        break;
                }

                SendAbilityActivated(conID, abilityType, isTraitor);
            }
        }
    }
    private void ActionPoints(int conID, int chanID, int rHostID, ActionPoints points)
    {

        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {

                player.GetComponent<Player>().ActionPoints = GameManager.instance.RollActionPoints();
                //Needs to call send Active rooms to the client here


            }
        }
    }
    private void AssignRoomMovement(int conID, int chanID, int rHostID, Movement moveTo)
    {

        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {

                //Need to assign the player gameobject here
                PlayerMovement.instance.PlayerMoveViaNodes(moveTo.SelectedRoom);


            }
        }
    }
    private void ChoiceSelection(int conID, int chanID, int rHostID, SelectedChoice choice)
    {

        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {





            }
        }
    }
    private void Inventory(int conID, int chanID, int rHostID, InventoryChanges changes)
    {

        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {





            }
        }
    }
    private void SpecSelection(int conID, int chanID, int rHostID, SpecSelection selection)
    {

        //Only the active player can be the attacker in a combat. If this message is coming from the attacking
        if (conID == GameManager.instance.GetActivePlayer().playerID)
        {
            attackerSpec = (GameManager.SpecScores)selection.SelectedSpec;
        }
        //Only the defending player which has the combat screen open can send this message so don't need to check which specific
        //player sent this message as long as it is not the active player.
        else
        {
            defenderSpec = (GameManager.SpecScores)selection.SelectedSpec;
        }

        //If both the attacker and defender scores are set, can proceed with the combat
        if (attackerSpec != GameManager.SpecScores.Default && defenderSpec != GameManager.SpecScores.Default)
        {
            //Need to add display of winner on main screen

            //Need to send outcome of combat to combatant players here
        }
    }
    private void SpecCombat(int conID, int chanID, int rHostID, CombatAttackingTarget attack)
    {

        //Resets spec scores for combat in order to setup check that both players have sent combat spec
        //scores back to server
        attackerSpec = GameManager.SpecScores.Default;
        defenderSpec = GameManager.SpecScores.Default;

        //Need to display the state of the combat on the main screen

        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {

                SendBeingAttacked(conID, conID, attack.TargetPlayer);

            }
            else if (player.GetComponent<Player>().playerID == attack.TargetPlayer)
            {

                SendBeingAttacked(attack.TargetPlayer, conID, attack.TargetPlayer);

            }
        }
    }
    private void ItemSelection(int conID, int chanID, int rHostID, ItemSelection selection)
    {

        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {





            }
        }
    }
    private void InstallComponent(int conID, int chanID, int rHostID, InstallComponent component)
    {

        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {

                bool outcome = GameManager.instance.InstallComponent();


                SendComponentInstalled(conID, outcome);
            }
        }
    }

    private void NewPhase(int conID, int chanID, int rHostID, NewPhase phase)
    {
        GameManager.instance.IncrementPhase();

        int activePlayerID = GameManager.instance.GetActivePlayer().playerID;

        switch (GameManager.instance.CurrentVictory)
        {
            case (GameManager.VictoryTypes.NonTraitor):
                //Display non-traitor victory screen
                SendNonTraitorVictory();
                break;
            case (GameManager.VictoryTypes.Traitor):
                //Display traitor victory screen
                SendTraitorVictory(GameManager.instance.traitorWinID);
                break;
            case (GameManager.VictoryTypes.None):

                //Need to insert UI elements into this
                switch (GameManager.instance.currentPhase)
                {
                    case (GameManager.TurnPhases.Abilities):
                        SendAbilityInformation(activePlayerID);
                        break;
                    case (GameManager.TurnPhases.ActionPoints):
                        //Not sure if needing to send anything here- maybe just a ping to say start rolling action points
                        break;
                    case (GameManager.TurnPhases.Movement):
                        //Send List of rooms
                        break;
                    case (GameManager.TurnPhases.Interaction):
                        //Send choice information
                        break;
                    case (GameManager.TurnPhases.BasicSurge):
                        //Need to display surge information on main screen
                        SendSurge();
                        break;
                    case (GameManager.TurnPhases.AttackSurge):
                        //Need to display surge information on main screen

                        break;
                    default:
                        throw new NotImplementedException("Not a valid phase");
                }

                break;
        }
    }
    #endregion
}