using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

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

    //Combat Variables
    private GameManager.SpecScores attackerSpec;
    private GameManager.SpecScores defenderSpec;
    private int defenderID;





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


                GameManager.instance.GeneratePlayer(connectionID, "default");
                Debug.Log("playerconnection of " + connectionID);
                break;

            //When user disconnects from game
            case NetworkEventType.DisconnectEvent:
                //Loop through to find player that is disconnecting, based on their ID

                Debug.Log(connectionID + " has disconnected");
                break;

            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                ms.Position = 0;
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
                ms.Position = 0;
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
            case NetOP.AvailableRooms:
                AvailableRooms(conID, chanID, rHostID, (AvailableRooms)msg);
                break;
            case NetOP.SelectRoom:
                RoomCost(conID, chanID, rHostID, (SelectRoom)msg);
                break;
            case NetOP.PlayerDataSync:
                SyncClientData(conID, chanID, rHostID, (PlayerDataSync)msg);
                break;
            case NetOP.AbilityInformation:
                GetAbilityInfo(conID, chanID, rHostID, (AbilityInformation)msg);
                break;
            case NetOP.SendRoomCost:
                RecieveRoomCost(conID, chanID, rHostID, (SendRoomCost)msg);
                break;
            case NetOP.RoomChoices:
                StoreRoomChoices(conID, chanID, rHostID, (RoomChoices)msg);
                break;
            case NetOP.SendAllPlayerIDS:
                GetAllPlayerIDS(conID, chanID, rHostID, (SendAllPlayerIDS)msg);
                break;
            case NetOP.SendAllPlayerNames:
                GetAllPlayersNames(conID, chanID, rHostID, (SendAllPlayerNames)msg);
                break;
            case NetOP.SendAllPlayerCharacterTypes:
                GetAllPlayersCharacters(conID, chanID, rHostID, (SendAllPlayerCharacters)msg);
                break;
            case NetOP.SpecChallenge:
                SpecResult(conID, chanID, rHostID, (SpecChallenge)msg);
                break;
            case NetOP.SurgeInformation:
                GetSurgeInformation(conID, chanID, rHostID, (SurgeInformation)msg);
                break;
            case NetOP.NextTurn:
                EndSurge(conID, chanID, rHostID, (NextTurn)msg);
                break;
            case NetOP.ComponentInstalled:
                GetComponentInstalled(conID, chanID, rHostID, (ComponentInstalled)msg);
                break;
            case NetOP.CanInstallComponent:
                GetCanInstallComponent(conID, chanID, rHostID, (CanInstallComponent)msg);
                break;
            case NetOP.NonTraitorVictory:
                GetNonTraitorVictory(conID, chanID, rHostID, (InnocentVictory)msg);
                break;
            case NetOP.CombatBeingAttacked:
                ReceiveCombat(conID, chanID, rHostID, (CombatBeingAttacked)msg);
                break;
            case NetOP.CombatWinner:
                GetCombatWinner(conID, chanID, rHostID, (CombatWinner)msg);
                break;
            case NetOP.CombatLoser:
                GetCombatLoser(conID, chanID, rHostID, (CombatLoser)msg);
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
        ms.Position = 0;
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
    public void StartGame() //This is called when a game is started in lobby
    {


        //Send message to every player's client to move onto next scene
        SendChangeScene(ClientManager.CHARACTER_SELECTION_SCENE);
        //Change to the character select
        SceneManager.LoadScene(GameManager.CharacterScene);
    }


    public void SendClient(NetMessage msg)
    {
        //This is where data is held
        byte[] buffer = new byte[byteSize * 25];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        ms.Position = 0;
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
        ms.Position = 0;
        formatter.Serialize(ms, msg);

        Debug.Log("sent");
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, byteSize, out error);

    }

    #region Server Sent Messages

    public void SendChangeScene(string SceneName)
    {

        SceneChange scene = new SceneChange();
        scene.SceneName = SceneName;
        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {

            Debug.Log("Send Change Scene to " + i);
            tempPlayerID = GameManager.instance.GetPlayer(i).playerID;
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

        if (GameManager.instance.GetActivePlayer().roomPosition == Player.STARTING_ROOM_ID) {

            CanInstallComponent(tempPlayerID);

        }
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

    public void SendIsTraitor()
    {
        if (GameManager.instance.newTraitor != GameManager.DEFAULT_PLAYER_ID)
        {
            TraitorSelection traitor = new TraitorSelection();
            tempPlayerID = GameManager.instance.newTraitor;

            SendClient(traitor);
        }
    }

    public void SendSurge()
    {
        SurgeInformation surge = new SurgeInformation();
        surge.NewAiPower = GameManager.instance.AIPower;
        surge.PowerIncrease = GameManager.instance.AIPowerIncrease();
        surge.PlayerIncrease = GameManager.instance.playerPower;
        surge.ChoiceIncrease = GameManager.instance.aiPowerChange;
        surge.baseIncrease = GameManager.instance.basePower;



        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            tempPlayerID = GameManager.instance.GetPlayer(i).playerID;
            SendClient(surge);
        }
    }

    public void SendAIAttack(int targetPlayer)
    {
        //Need to inform all players that someone is under attack by the AI
        //IsTarget specifies which player actually is the target


        for (int i = 0; i < GameManager.instance.numPlayers; i++)
        {
            AiAttacks ai = new AiAttacks();

            ai.TargetID = targetPlayer;
            ai.IsTarget = (GameManager.instance.GetPlayer(i).playerID == targetPlayer);
            SendClient(ai);
        }

    }

    public void SendCombatWinner(int winnerID, int loserID)
    {

        CombatWinner combatWinner = new CombatWinner();
        tempPlayerID = winnerID;

        combatWinner.LoserID = loserID;

        List<Item> loserInventory = GameManager.instance.GetPlayer(loserID).items;

        combatWinner.LoserInventory = new List<int>();

        foreach (Item item in loserInventory)
        {
            int itemID = (int)item.ItemType;

            combatWinner.LoserInventory.Add(itemID);
        }

        SendClient(combatWinner);
    }

    public void SendCombatLoser(int loserID, int winnerID)
    {
        CombatLoser combatLoser = new CombatLoser();
        tempPlayerID = loserID;

        combatLoser.WinnerID = winnerID;

        SendClient(combatLoser);
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
        Debug.Log("attacking " + tempPlayerID);

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

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            tempPlayerID = GameManager.instance.GetPlayer(i).playerID;
            SendClient(innocent);
        }

    }

    public void SendTraitorVictory(int winnerID)
    {
        TraitorVictory traitor = new TraitorVictory();
        traitor.WinnerID = winnerID;

        for (int i = 0; i < GameManager.instance.numPlayers; i++)
        {
            tempPlayerID = GameManager.instance.GetPlayer(i).playerID;
            SendClient(traitor);
        }
    }

    public void SyncPlayerData(int playerID)
    {
        PlayerDataSync playerData = new PlayerDataSync();
        tempPlayerID = playerID;

        Player player = GameManager.instance.GetPlayer(playerID);

        if(SceneManager.GetActiveScene().name == "Server GameLevel") {
            PlayerCardManager.instance.UpdatePlayerCard(playerID);
            Debug.Log("Updated Card");

        }
        playerData.ID = playerID;
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

        //playerData.Items = new List<int>();
        //playerData.ItemEquipped = new List<bool>();

        //foreach (Item item in player.items)
        //{
        //    playerData.Items.Add((int)item.ItemType);
        //    playerData.ItemEquipped.Add(item.isEquipped);
        //}

        SendClient(playerData);
        Debug.Log("send data to" + tempPlayerID);
    }

    public void SendRoomCost(int playerID, int RoomCost)
    {

        SendRoomCost roomCost = new SendRoomCost();
        tempPlayerID = playerID;

        roomCost.RoomCost = RoomCost;

        SendClient(roomCost);

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

        for (int i = 1; i < GameManager.instance.numPlayers+1; i++)
        {
            tempPlayerID = GameManager.instance.GetPlayer(i).playerID;

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

    public void CanInstallComponent(int playerID)
    {
        CanInstallComponent canInstallComponent = new CanInstallComponent();
        tempPlayerID = playerID;

        canInstallComponent.CanInstall = GameManager.instance.CanInstallComponent();

        SendClient(canInstallComponent);
    }

    /// <summary>
    /// 
    /// Sends the relevant information about the other players to all clients
    /// Relevant information is their ID, name and Character Type
    /// Should be called after Character selection is complete, since this information
    /// will not change from then
    /// 
    /// </summary>
    //public void SendAllPlayerData()
    //{
    //    AllPlayerData allPlayerData = new AllPlayerData();

    //    allPlayerData.numPlayers = GameManager.instance.numPlayers;
    //    allPlayerData.PlayerIDs = new List<int>();
    //    allPlayerData.PlayerNames = new List<string>();
    //    allPlayerData.CharacterTypes = new List<int>();

    //    //Setup the player data (need to clarify this is working properly since it pulls from the
    //    //players list in the server rather than the game manager)

    //    for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
    //    {

    //        Player player = GameManager.instance.GetPlayer(i);
    //        allPlayerData.PlayerIDs.Add(player.playerID);
    //        allPlayerData.PlayerNames.Add(player.playerName);
    //        allPlayerData.CharacterTypes.Add((int)player.Character.CharacterType);


    //    }

    //    for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
    //    {
    //        tempPlayerID = GameManager.instance.GetPlayer(i).playerID;
    //        Debug.Log("Sent all player Data " + tempPlayerID);
    //        SendClient(allPlayerData);

    //    }

    //}


    public void sendplayerIDS()
    {

        SendAllPlayerIDS ids = new SendAllPlayerIDS();
        ids.PlayerIDS = new List<int>();

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {

            Player player = GameManager.instance.GetPlayer(i);
            ids.PlayerIDS.Add(player.playerID);


        }

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            tempPlayerID = GameManager.instance.GetPlayer(i).playerID;
            Debug.Log("Sent all player Data " + tempPlayerID);
            SendClient(ids);

        }



    }

    public void sendAllPlayerNames()
    {
        SendAllPlayerNames names = new SendAllPlayerNames();
        names.PlayerNames = new List<string>();

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {

            Player player = GameManager.instance.GetPlayer(i);
            names.PlayerNames.Add(player.playerName);


        }

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            tempPlayerID = GameManager.instance.GetPlayer(i).playerID;
            Debug.Log("Sent all player Data " + tempPlayerID);
            SendClient(names);

        }

    }


    public void sendallCharacterTypes()
    {
        SendAllPlayerCharacters characters = new SendAllPlayerCharacters();
         characters.CharacterTypes = new List<int>();

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {

            Player player = GameManager.instance.GetPlayer(i);
            characters.CharacterTypes.Add((int)player.Character.CharacterType);


        }

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            tempPlayerID = GameManager.instance.GetPlayer(i).playerID;
            Debug.Log("Sent all player Data " + tempPlayerID);
            SendClient(characters);

        }


    }

    public void SendUnequipSuccess(int playerID)
    {
        UnequipSuccess unequipSuccess = new UnequipSuccess();
        tempPlayerID = playerID;

        SendClient(unequipSuccess);
    }

    public void SendEquipState(int playerID, Player.EquipErrors equipError)
    {
        EquipState equipState = new EquipState();
        tempPlayerID = playerID;

        equipState.EquipError = (int)equipError;

        SendClient(equipState);
    }

    public void SendDiscardSuccess(int playerID)
    {
        DiscardSuccess discardSuccess = new DiscardSuccess();
        tempPlayerID = playerID;

        SendClient(discardSuccess);
    }

    public void SendStealSuccess(int playerID, bool isSuccessful)
    {
        StealSuccess stealSuccess = new StealSuccess();
        tempPlayerID = playerID;

        stealSuccess.IsSuccessful = isSuccessful;

        SendClient(stealSuccess);
    }

    public void SendStealDiscardSuccess(int playerID)
    {
        StealDiscardSuccess stealDiscardSuccess = new StealDiscardSuccess();
        tempPlayerID = playerID;

        SendClient(stealDiscardSuccess);
    }

    public void SendItemStolen(int playerID, string itemName)
    {
        ItemStolen itemStolen = new ItemStolen();
        tempPlayerID = playerID;

        itemStolen.ItemName = itemName;

        SendClient(itemStolen);
    }

    public void SendComponentStealSuccess(int playerID, bool isSuccessful)
    {
        ComponentStealSuccess componentStealSuccess = new ComponentStealSuccess();
        tempPlayerID = playerID;

        componentStealSuccess.IsSuccessful = isSuccessful;

        SendClient(componentStealSuccess);
    }

    public void SendComponentStolen(int playerID)
    {
        ComponentStolen componentStolen = new ComponentStolen();
        tempPlayerID = playerID;

        SendClient(componentStolen);
    }

    public void SendAIAttackResult(int playerID, bool wonAttack)
    {
        AIAttackResult aiAttackResult = new AIAttackResult();
        tempPlayerID = playerID;

        aiAttackResult.WonAttack = wonAttack;

        SendClient(aiAttackResult);
    }

    #endregion

    #region Client Received Messages

    private void GetSceneChange(int conID, int chanID, int rHostID, SceneChange scene)
    {

        SceneManager.LoadScene(scene.SceneName);

 

    }

    private void AmActivePlayer(int conID, int chanID, int rHostID, IsActivePlayer isActive)
    {

        if (SceneManager.GetActiveScene().name == "Client CharacterSelect")
        {
            GameObject Canvas = GameObject.Find("Canvas");
            Canvas.GetComponent<CharacterSelectUIManager>().DisplayActivePlayer();
        }
        else if (SceneManager.GetActiveScene().name == "Client GameLevel")
        {

            Debug.Log("is your turn");
            GameManager.instance.currentPhase = GameManager.TurnPhases.Abilities;
            SendNewPhase();
            ClientUIManager.instance.DisplayCurrentPhase();

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
            CharacterSelectUIManager charSelect = Canvas.GetComponent<CharacterSelectUIManager>();
            charSelect.SetErrorText("Please Select Another Character.");
            charSelect.ResetCharacterSelection();
        }


    }

    private void GetCharacterInformation(int conID, int chanID, int rHostID, CharacterInformation information)
    {



    }

    private void SyncClientData(int conID, int chanID, int rHostID, PlayerDataSync playerData)
    {
        ClientManager.instance.playerID = playerData.ID;
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

        //for (int itemIndex = 0; itemIndex < playerData.Items.Count; itemIndex++)
        //{
        //    ClientManager.instance.inventory.Add(new Item((Item.ItemTypes)playerData.Items[itemIndex]));
        //    ClientManager.instance.inventory[itemIndex].isEquipped = playerData.ItemEquipped[itemIndex];
        //}
    }

    private void StoreRoomChoices(int conID, int chanID, int rHostID, RoomChoices roomChoices)
    {
        Debug.Log("recieved choices");
        GameManager.instance.currentPhase = GameManager.TurnPhases.Interaction;

        ClientUIManager.instance.interactionPanel.SetActive(true);
        InteractionManager manager = GameObject.Find("GamePanels").transform.Find("InteractionPanel").gameObject.GetComponent<InteractionManager>();

        Choice.IsAvailableTypes[] isAvailables = new Choice.IsAvailableTypes[ChoiceRandomiser.CHOICES_PER_ROOM];
        GameManager.SpecScores[] specScores = new GameManager.SpecScores[ChoiceRandomiser.CHOICES_PER_ROOM];

        for (int choiceIndex = 0; choiceIndex < ChoiceRandomiser.CHOICES_PER_ROOM; choiceIndex++)
        {
            isAvailables[choiceIndex] = (Choice.IsAvailableTypes)roomChoices.IsAvailables[choiceIndex];
            specScores[choiceIndex] = (GameManager.SpecScores)roomChoices.SpecScores[choiceIndex];

        }
        
        manager.choiceNames = roomChoices.ChoiceNames;
        manager.successTexts = roomChoices.SuccessTexts;
        manager.failTexts = roomChoices.FailTexts;
        manager.isAvailables = isAvailables;
        manager.specScores = specScores;
        manager.successChances = roomChoices.SuccessChances;
        manager.attackablePlayers = roomChoices.AttackablePlayers;
        ClientUIManager.instance.DisplayCurrentPhase();

    }




    private void AvailableRooms(int conID, int chanID, int rHostID, AvailableRooms rooms)
    {

        Debug.Log("recieved available rooms");
        for (int i = 0; i < rooms.AvailableRoomsIDs.Count; i++)
        {

            GameObject room = GameManager.instance.roomList.GetComponent<WayPointGraph>().graphNodes[rooms.AvailableRoomsIDs[i]];
            Debug.Log(room.name);
            room.transform.GetChild(1).gameObject.SetActive(false);

        }
        GameManager.instance.currentPhase = GameManager.TurnPhases.Movement;
        GameManager.instance.roomSelection = true;
        ClientUIManager.instance.DisplayCurrentPhase();
        

    }

    private void RecieveRoomCost(int conID, int chanID, int rHostID, SendRoomCost cost)
    {

        MovementManager.instance.roomCost = cost.RoomCost;
        MovementManager.instance.roomID = GameManager.instance.playerGoalIndex;
        MovementManager.instance.scrapReturn = cost.ScrapReturn;
        MovementManager.instance.SetupMoveToRoom(); 


    }


    private void GetAbilityInfo(int conID, int chanID, int rHostID, AbilityInformation abilityInformation)
    {
        Debug.Log("recieved ability information");
        ClientManager.instance.abilities = new List<Ability>();

        for (int abilityID = 0; abilityID < Player.NUM_ABILITIES; abilityID++)
        {
            Ability ability = ClientManager.instance.GetAbilityInfo(abilityInformation.AbilityTypes[abilityID]);
            Debug.Log(ability.abilityDescription);
            ClientManager.instance.abilities.Add(ability);

            //Need to send this information to the UI Manager to display to the player
            //Displayed information would be the ability name, the scrap cost, the 
            //corruption cost (stored within ability) as well as the booleans in
            //abiltyInformation, CheckCorruptio and CheckScrap, which can determine
            //whether or not the ability can be selected

        }
        AbilityManager.instance.CheckCorruption = abilityInformation.CheckCorruption;
        AbilityManager.instance.CheckScrap = abilityInformation.CheckScrap;
        AbilityManager.instance.SetupAbilities();
    }

    private void ReceiveCombat(int conID, int chanID, int rHostID, CombatBeingAttacked beingAttacked)
    {
        //Need to display attacked and defender info to players and allow them to select spec score for combat

        Debug.Log("Recieved being attacked");
        Debug.Log(beingAttacked.AttackerID);
        ClientUIManager.instance.interactionPanel.SetActive(true);
        ClientUIManager.instance.interactionPanel.GetComponent<InteractionManager>().SetupDefence(beingAttacked.AttackerID);


    }

    private void AbilityActivated(int conID, int chanID, int rHostID, AbilityActivated abilityActivated)
    {
        Debug.Log("Recieved ability activated");

        Ability.AbilityTypes abilityType = (Ability.AbilityTypes)abilityActivated.AbilityType;

        if (abilityType == Ability.AbilityTypes.Code_Inspection)
        {
            //Set up the modifier to the traitor string
            string traitorString = "";
            if (!abilityActivated.IsTraitor) {
                traitorString = "not ";
            }
            AbilityManager.instance.abilityInfoText.SetActive(true);
            AbilityManager.instance.abilityInfoText.GetComponent<TextMeshProUGUI>().text = string.Format("{0} is {1}a traitor", ClientManager.instance.GetPlayerData(AbilityManager.instance.selectedPlayer).PlayerName, traitorString);

            AbilityManager.instance.DisplayActiveAbility();

        }
        else
        {

           AbilityManager.instance.DisplayActiveAbility();

        }
    }

    /// <summary>
    /// 
    /// Message for the player who installed the component to recieve when a component is installed
    /// 
    /// </summary>
    private void GetComponentInstalled(int conID, int chanID, int rHostID, ComponentInstalled componentInstalled)
    {
        InteractionManager.instance.ResultPanel.SetActive(true);

        if (componentInstalled.SuccessfulInstall)
        {
            ClientManager.instance.componentsInstalled += 1;
            GameObject canvas = GameObject.Find("Canvas");
            canvas.GetComponent<ClientUIManager>().UpdateComponentTracker();
            //Need to display to the player that a component has been installed
            InteractionManager.instance.ResultText.GetComponent<TextMeshProUGUI>().text = "Installed Component";

            if (componentInstalled.AllComponentsInstalled)
            {
                //Need to display to the player that all components have been installed and they can escape
                InteractionManager.instance.ResultText.GetComponent<TextMeshProUGUI>().text = "All Components Installed";
            }
        }
        else
        {
            //Need to display to the player that they have been sabotaged and lost life points equal to
            //GameManager.COMBAT_DAMAGE.

            InteractionManager.instance.ResultText.GetComponent<TextMeshProUGUI>().text = "You have been sabotaged. " + GameManager.COMBAT_DAMAGE + " point of damage taken";
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

    private void GetCanInstallComponent(int conID, int chanID, int rHostID, CanInstallComponent canInstallComponent)
    {
        //Activate whatever is neccessary for the player to install a component

        InteractionManager.instance.installButton.GetComponent<Button>().interactable = canInstallComponent.CanInstall;

    }

    private void GetCombatWinner(int conID, int chanID, int rHostID, CombatWinner combatWinner)
    {
        //Need to display that they won the combat and who they won it against using combatWinner.loserID
        //Also need to store the loser ID to send back to the server when stealing the items

        InteractionManager.instance.combatPanel.GetComponent<CombatComponentsClient>().WinnerPanel.SetActive(true);
        InteractionManager.instance.combatPanel.GetComponent<CombatComponentsClient>().LoserText.GetComponent<TextMeshProUGUI>().text = "You won against " + ClientManager.instance.GetPlayerData(combatWinner.LoserID).PlayerName;

        //Following converts the IDs for the losers inventory into Item objects, allowng the player to inspect the objects
        //Need to display the items on the stealing panel
        List<int> loserItemIDs = combatWinner.LoserInventory;
        List<Item> loserInventory = new List<Item>();

        foreach (int itemID in loserItemIDs)
        {
            Item item = ClientManager.instance.GetItemInfo(itemID);
            loserInventory.Add(item);
            Debug.Log(item.ItemName);
        }
        ClientUIManager.instance.interactionPanel.GetComponent<InteractionManager>().stealPanel.GetComponent<StealingManager>().loserID = combatWinner.LoserID;
        ClientUIManager.instance.interactionPanel.GetComponent<InteractionManager>().stealPanel.GetComponent<StealingManager>().losersItems = loserInventory;
      
    }

    private void GetCombatLoser(int conID, int chanID, int rHostID, CombatLoser combatLoser)
    {
        //Need to display that they lost the combat and who they lost it against using combatLoser.winnerID

        InteractionManager.instance.combatPanel.GetComponent<CombatComponentsClient>().LoserPanel.SetActive(true);
        InteractionManager.instance.combatPanel.GetComponent<CombatComponentsClient>().LoserText.GetComponent<TextMeshProUGUI>().text = "You lost to " + ClientManager.instance.GetPlayerData(combatLoser.WinnerID).PlayerName;


    }

    private void GetAllPlayerIDS(int conID, int chanID, int rHostID, SendAllPlayerIDS allPlayerData)
    {
        ClientManager.instance.playerData = new List<PlayerData>();
        
        Debug.Log("recieved player ids");

        for (int playerIndex = 0; playerIndex < allPlayerData.PlayerIDS.Count; playerIndex++)
        {
            int playerID = allPlayerData.PlayerIDS[playerIndex];
            ClientManager.instance.playerIDS.Add(playerID);
           
        }

        ClientManager.instance.numPlayers = ClientManager.instance.playerIDS.Count;

        ClientUIManager clientUIManager = GameObject.Find("Canvas").GetComponent<ClientUIManager>();
        clientUIManager.interactionPanel.SetActive(true);
        clientUIManager.interactionPanel.GetComponent<InteractionManager>().InitComponentPanel();
        clientUIManager.interactionPanel.SetActive(false);
    }

    private void GetAllPlayersNames(int conID, int chanID, int rHostID, SendAllPlayerNames allPlayerData)
    {

        Debug.Log("recieved player names");

        for (int playerIndex = 0; playerIndex < allPlayerData.PlayerNames.Count; playerIndex++)
        {
            string playername = allPlayerData.PlayerNames[playerIndex];
            ClientManager.instance.PlayerNames.Add(playername);

        }

        for (int playerIndex = 0; playerIndex < allPlayerData.PlayerNames.Count; playerIndex++)
        {

            Character.CharacterTypes character = (Character.CharacterTypes)ClientManager.instance.CharacterTypes[playerIndex];

            Debug.Log("Creating playerData");
            ClientManager.instance.playerData.Add(new PlayerData(ClientManager.instance.playerIDS[playerIndex], ClientManager.instance.PlayerNames[playerIndex], character));

        }
        //sets up target panels to include only players in game.
        ClientUIManager.instance.SetupTargets(ClientUIManager.instance.interactionPanel.GetComponent<InteractionManager>().targetButtons);
        ClientUIManager.instance.SetupTargets(ClientUIManager.instance.abilityPanel.GetComponent<AbilityManager>().targetButtons);







    }

    private void GetAllPlayersCharacters(int conID, int chanID, int rHostID, SendAllPlayerCharacters allPlayerData)
    {
        
        Debug.Log("recieved player characterIDS");

        for (int playerIndex = 0; playerIndex < allPlayerData.CharacterTypes.Count; playerIndex++)
        {
            int playercharacter = allPlayerData.CharacterTypes[playerIndex];
            ClientManager.instance.CharacterTypes.Add(playercharacter);

        }


       

    }


    private void GetUnequipSuccess(int conID, int chanID, int rHostID, UnequipSuccess unequipSuccess)
    {
        //Need to update the UI for the inventory and spec scores (could be done using SyncClientData however)
    }

    private void GetEquipState(int conID, int chanID, int rHostID, EquipState equipState)
    {
        switch ((Player.EquipErrors)equipState.EquipError)
        {
            case (Player.EquipErrors.Default):
                //Update Inventory UI and spec scores (could be done using SyncClientData however)
                break;
            case (Player.EquipErrors.AlreadyEquipped):
                //Display to the player that the item is already equipped
                break;
            case (Player.EquipErrors.TooManyEquipped):
                //Display to the player that they have too many items equipped
                break;
        }
    }

    private void GetDiscardSuccess(int conID, int chanID, int rHostID, DiscardSuccess discardSuccess)
    {
        //Need to update the UI for the inventory and spec scores (could be done using SyncClientData however)
    }

    private void GetStealSuccess(int conID, int chanID, int rHostID, StealSuccess stealSuccess)
    {
        if (stealSuccess.IsSuccessful)
        {
            //Update the UI for the inventory and spec scores (could be done using SyncClientData however)
            //Prevent them from stealing any more items
        }
        else
        {
            //Display to player that they cannot hold any more items
        }
    }

    private void GetStealDiscardSuccess(int conID, int chanID, int rHostID, StealDiscardSuccess stealDiscardSuccess)
    {
        //Display that they successfully discarded the item
        //Prevent them from stealing any more items
    }

    private void GetItemStolen(int conID, int chanID, int rHostID, ItemStolen itemStolen)
    {
        //Tell the player that one of their items was stolen (the name is stored in itemStolen)
        //Also need to update the UI for their inventory and spec scores (could be done using SyncClientData however
    }

    private void GetIsTraitor(int conID, int chanID, int rHostID, TraitorSelection traitorSelection)
    {
        //Update the UI to display to the player that they have been selected as traitor
        ClientManager.instance.isTraitor = true;
    }

    private void GetSurgeInformation(int conID, int chanID, int rHostID, SurgeInformation information) {

        Debug.Log("updating surge");

        ClientBasicSurgeManager manager = GameObject.Find("GamePanels").transform.Find("BasicSurgePanel").gameObject.GetComponent<ClientBasicSurgeManager>();

        manager.power = information.NewAiPower;
        manager.basepower = information.baseIncrease;
        manager.choiceIncreaseUnit = information.ChoiceIncrease;
        manager.playerpower = information.PlayerIncrease;
        manager.powerchange = information.PowerIncrease;

        ClientUIManager.instance.DisplaySurge();


    }


    private void GetAIAttack(int conID, int chanID, int rHostID, AiAttacks aiAttacks)
    {
        if (aiAttacks.IsTarget)
        {
            //Display to the player the AI Attack UI so they can choose a spec score to defend themselves
        }
        else
        {
            //Need to display which player is under attack using aiAttacks.targetID
        }
    }

    private void GetAIAttackResult(int conID, int chanID, int rHostID, AIAttackResult aiAttackResult)
    {
        if (aiAttackResult.WonAttack)
        {
            //Display that the player won the attack
        }
        else
        {
            //Display that they lost the attack
        }
    }

    private void GetNonTraitorVictory(int conID, int chanID, int rHostID, InnocentVictory innocentVictory)
    {
        if (ClientManager.instance.isTraitor)
        {
            //If the player is a traitor, display the loss screen
            ClientUIManager.instance.TraitorsLosePanel.SetActive(true);
        }
        else
        {
            //If the player is not a traitor, display the victory screen
            ClientUIManager.instance.nonTraitorVictoryPanel.SetActive(true);
        }
    }

    private void GetTraitorVictory(int conID, int chanID, int rHostID, TraitorVictory traitorVictory)
    {
        if (traitorVictory.WinnerID == ClientManager.instance.playerID)
        {
            //If this player is the winner, display the victory screen
            ClientUIManager.instance.traitorVictoryPanel.SetActive(true);
        }
        else
        {
            //Otherwise, display the loss screen as well as w
            ClientUIManager.instance.nonTraitorLosePanel.SetActive(true);
        }
    }

    private void SpecResult(int conID, int chanID, int rHostID, SpecChallenge challenge)
    {

        ClientUIManager.instance.ShowResult(challenge.result);

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

    public void SendActionPoints(int HowManyPoints)
    {

        ActionPoints actionPoints = new ActionPoints();
        actionPoints.actionPoints = HowManyPoints;

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

    /// <summary>
    /// 
    /// To be deleted- broken down into unique messages for each action
    /// 
    /// </summary>
    public void SendInventoryChanges(List<string> EquiptItems, List<string> UnEquiptItems, List<string> DiscardItems)
    {

        InventoryChanges inventory = new InventoryChanges();
        inventory.equipedItems = EquiptItems;
        inventory.UnequipedItems = UnEquiptItems;
        inventory.discardItems = DiscardItems;

        SendServer(inventory);
    }

    public void SendSpecSelection(GameManager.SpecScores specScore, bool attacker)
    {
        //Used in regular combat
        SpecSelection specSelection = new SpecSelection();
        specSelection.SelectedSpec = (int)specScore;
        specSelection.Attacker = attacker;

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
        Debug.Log("sent newphase");
        SendServer(newPhase);
    }

    public void EquipItem(int itemID)
    {
        EquipItem equipItem = new EquipItem();
        equipItem.ItemID = itemID;

        SendServer(equipItem);
    }

    public void DiscardItem(int itemID)
    {
        DiscardItem discardItem = new DiscardItem();
        discardItem.ItemID = itemID;

        SendServer(discardItem);
    }

    public void StealItem(int itemID, int loserID, bool stealComponent)
    {
        StealItem stealItem = new StealItem();
        stealItem.ItemID = itemID;
        stealItem.LoserID = loserID;
        //Used if the player is stealing a component
        stealItem.StealComponent = stealComponent;

        SendServer(stealItem);
    }

    public void StealDiscardItem(int itemID, int loserID)
    {
        StealDiscardItem stealDiscardItem = new StealDiscardItem();
        stealDiscardItem.ItemID = itemID;
        stealDiscardItem.LoserID = loserID;

        SendServer(stealDiscardItem);
    }

    public void AISpecSelection(GameManager.SpecScores specScore)
    {
        //Used to defend against AI Attacks
        AISpecSelection aiSpecSelection = new AISpecSelection();
        aiSpecSelection.SelectedSpec = (int)specScore;

        SendServer(aiSpecSelection);
    }

    public void SendRoomChoiceForCost(int roomId)
    {

        SelectRoom select = new SelectRoom();

        select.roomID = roomId;

        SendServer(select);

    }

    public void SendEndRound() {

        NextTurn turn = new NextTurn();

        SendServer(turn);

    }

    #endregion

    #region Server Received Messages

    private void AssignPlayerDetails(int conID, int chanID, int rHostID, PlayerDetails details)
    {

        GameManager.instance.GetPlayer(conID).playerName = details.PlayerName;

        Debug.Log("Recieved Player Name");
        Debug.Log("Set name to " + GameManager.instance.GetPlayer(conID).playerName);

        GameObject LobbyUiHandler = GameObject.Find("Canvas");
        LobbyUiHandler.GetComponent<LobbyUIManager>().AddPlayerNames(conID);




    }
    private void AssignCharacterSelection(int conID, int chanID, int rHostID, CharacterSelection character)
    {

        GameManager.instance.GetPlayer(conID);

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);
            

            //Find the correct player
            if (player.playerID == conID)
            {

                if (GameManager.instance.CheckCharacterSelected((Character.CharacterTypes)character.SelectedCharacter))
                {
                    SendChangeCharacter(player.playerID, true);
                }
                else
                {
                    GameObject canvas = GameObject.Find("Canvas");
                    canvas.GetComponent<ServerCharacterSelection>().tempCharacterType = (Character.CharacterTypes)character.SelectedCharacter;
                    canvas.GetComponent<ServerCharacterSelection>().UpdatePlayerCharacter();
                    player.Character = new Character((Character.CharacterTypes)character.SelectedCharacter);
                    //Assign Character Stats to player
                    SyncPlayerData(player.playerID);

                    GameManager.instance.SelectCharacter((Character.CharacterTypes)character.SelectedCharacter);
                    SendChangeCharacter(player.playerID, false);
                    if (GameManager.instance.activePlayer > 0)
                    {
                        SendActivePlayer(GameManager.instance.GetActivePlayer().playerID);
                    }

                    string charName = character.ToString();

                    //GameObject charModel;
                    //charModel = GameObject.Find(charName);
                    //charModel.GetComponent<AnimationSwitcher>().IntroAnimation(charName);
                }

            }


        }




    }
    private void AbilityUsed(int conID, int chanID, int rHostID, AbilityUsage ability)
    {
        Debug.Log("recieved ability");
        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {

            Player player = GameManager.instance.GetPlayer(i);

            //Find the correct player
            if (player.playerID == conID)
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

                PlayerCardManager.instance.UpdateAllCards();
                SendAbilityActivated(conID, abilityType, isTraitor);
                SyncPlayerData(conID);
            }
        }




    }

    /// <summary>
    /// It goes through the players to find out who sent the original message.
    /// It then assigns that players current room positon to the navigation.
    /// It cycles through all the rooms in the map and sees if they are the same or below the amount of points the player has.
    /// Returns all room ids that fit that criteria.
    /// </summary>
    /// <param name="conID">Connection ID of the player</param>
    /// <param name="chanID">Channel ID of the player</param>
    /// <param name="rHostID">Host ID of the server</param>
    /// <param name="points">message sent over from client, holds the amount of points the player rolled</param>
    private void ActionPoints(int conID, int chanID, int rHostID, ActionPoints points)
    {

        Debug.Log("recieved actionpoints");

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);

            //Find the correct player
            if (player.playerID == conID)
            {
                GameManager.instance.actionPoints = points.actionPoints;
                player.ActionPoints = points.actionPoints;

                PlayerMovement Playermovement = GameObject.Find("Players").GetComponent<PlayerMovement>();
                Playermovement.StartMoving = false;
                Playermovement.currentNodeIndex = player.roomPosition;
                Playermovement.Player = player.playerObject;

                List<int> roomIds = new List<int>();

                for (int j = 0; j < GameManager.instance.roomList.GetComponent<WayPointGraph>().graphNodes.Length; j++)
                {

                    int roomCost;

                    Playermovement.PlayerMoveViaNodes(j);

                    if (PlayerMovement.instance.SecretPathActivated) {

                       roomCost = Playermovement.currentPath.Count - 2;

                    }
                    else {

                       roomCost = Playermovement.currentPath.Count - 1;
                    }
                    
                    Debug.Log("Cost to get to room " + j + " " + roomCost);

                    if (roomCost <= points.actionPoints)
                    {
                        roomIds.Add(j);
                    }
                }

                SendAvailableRooms(player.playerID, roomIds);


            }

        }
    }
    /// <summary>
    /// Once the player selects a room it calculates the amount of rooms it needs to cross to get there.
    /// </summary>
    /// <param name="conID">Connection ID of the player</param>
    /// <param name="chanID">Channel ID of the player</param>
    /// <param name="rHostID">Host ID of the server</param>
    /// <param name="room">int id of the room the player wants to move to.
    private void RoomCost(int conID, int chanID, int rHostID, SelectRoom room)
    {
        Debug.Log("recieved room " + room.roomID);
        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);

            if (player.playerID == conID)
            {

                PlayerMovement Playermovement = GameObject.Find("Players").GetComponent<PlayerMovement>();

                Playermovement.currentNodeIndex = player.roomPosition;
                Playermovement.goalIndex = room.roomID;
                Playermovement.StartMoving = false;

                Playermovement.PlayerMoveViaNodes(room.roomID);

                int roomCost = Playermovement.currentPath.Count - 1;
                if (PlayerMovement.instance.SecretPathActivated) {

                    roomCost -= 1;
                }

                SendRoomCost(player.playerID, roomCost);
            }
        }

    }


    private void AssignRoomMovement(int conID, int chanID, int rHostID, Movement moveTo)
    {

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);
            //Find the correct player
            if (player.playerID == conID)
            {
 


                PlayerMovement.instance.Player = player.playerObject;
                PlayerMovement.instance.currentNodeIndex = player.roomPosition;
                PlayerMovement.instance.StartMoving = true;
                GameManager.instance.playerGoalIndex = moveTo.SelectedRoom;
                GameManager.instance.playerMoving = true;

                
                PlayerMovement.instance.SecretPathActivated = false;

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="conID"></param>
    /// <param name="chanID"></param>
    /// <param name="rHostID"></param>
    /// <param name="choice"></param>
    private void ChoiceSelection(int conID, int chanID, int rHostID, SelectedChoice choice)
    {
        Debug.Log("Recieved choice");

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);
            //Find the correct player

            if (player.playerID == conID)
            {
                int choiceID = choice.ChoiceId;

                Room currentRoom = GameManager.instance.GetRoom(player.roomPosition);

                bool result = currentRoom.roomChoices[choiceID].SelectChoice();
                SendSpecChallenge(player.playerID, result);
                SyncPlayerData(player.playerID);

            }
        }
    }

    /// <summary>
    /// 
    /// Can be deleted
    /// 
    /// </summary>
    private void Inventory(int conID, int chanID, int rHostID, InventoryChanges changes)
    {

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);
            if (player.playerID == conID)
            {

            }
        }


    }


    private void SpecSelection(int conID, int chanID, int rHostID, SpecSelection selection)
    {
        int activePlayerID = GameManager.instance.GetActivePlayer().playerID;

        //Only the active player can be the attacker in a combat. If this message is coming from the attacking
        if (conID == activePlayerID)
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

            bool combatOutcome = GameManager.instance.PerformCombat(attackerSpec, defenderID, defenderSpec);

            //If combat outcome is ture, means that the attacker won
            //Need to add in main screen UI
            if (combatOutcome)
            {
                SendCombatWinner(activePlayerID, defenderID);
                SendCombatLoser(defenderID, activePlayerID);
            }
            else
            {
                SendCombatWinner(defenderID, activePlayerID);
                SendCombatLoser(activePlayerID, defenderID);
            }
        }
    }
    private void SpecCombat(int conID, int chanID, int rHostID, CombatAttackingTarget attack)
    {

        //Resets spec scores for combat in order to setup check that both players have sent combat spec
        //scores back to server
        attackerSpec = GameManager.SpecScores.Default;
        defenderSpec = GameManager.SpecScores.Default;

        //Need to display the state of the combat on the main screen

        //Stores the target player ID to attack later
        defenderID = attack.TargetPlayer;


        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);

           
            if (player.playerID == defenderID)
            {
                
                SendBeingAttacked(defenderID, GameManager.instance.GetActivePlayer().playerID, defenderID);

            }

        }



    }

    /// <summary>
    /// 
    /// Can probably be deleted
    /// 
    /// </summary>
    private void ItemSelection(int conID, int chanID, int rHostID, ItemSelection selection)
    {

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);

            if (player.playerID == conID)
            {

            }

        }
    }

    private void InstallComponent(int conID, int chanID, int rHostID, InstallComponent component)
    {

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);

            if (player.playerID == conID)
            {
                bool outcome = GameManager.instance.InstallComponent();


                SendComponentInstalled(conID, outcome);
            }
        }


    }

    public void NewPhase(int conID, int chanID, int rHostID, NewPhase phase)
    {
        GameManager.instance.IncrementPhase();
        Debug.Log("current Phase " + GameManager.instance.currentPhase);
        Player activePlayer= GameManager.instance.GetActivePlayer();
        Debug.Log("Current player " + activePlayer.playerID);

        GameObject canvas = GameObject.Find("Canvas");
        //Below function will handle displaying the new panels for the phase as well as victory condition screens
        canvas.GetComponent<MainGameUIManager>().IncrementPhase();

        switch (GameManager.instance.CurrentVictory)
        {
            case (GameManager.VictoryTypes.NonTraitor):
                SendNonTraitorVictory();
                break;
            case (GameManager.VictoryTypes.Traitor):
                SendTraitorVictory(GameManager.instance.traitorWinID);
                break;
            case (GameManager.VictoryTypes.None):

                //Need to insert UI elements into this
                switch (GameManager.instance.currentPhase)
                {
                    case (GameManager.TurnPhases.Abilities):
                        PlayerCardManager.instance.UpdateAllCards();
                        switch (GameManager.instance.GetActivePlayer().activeAbility.abilityType) {
                            case (Ability.AbilityTypes.Secret_Paths):
                            case (Ability.AbilityTypes.Power_Boost):
                            case (Ability.AbilityTypes.Muddle_Sensors):
                                GameManager.instance.GetActivePlayer().activeAbility.Deactivate();
                                break;
                        }
                        SendAbilityInformation(activePlayer.playerID);
                        Debug.Log("sent ability information");
                        break;
                    case (GameManager.TurnPhases.ActionPoints):
                        //Not sure if needing to send anything here- maybe just a ping to say start rolling action points
                        break;
                    case (GameManager.TurnPhases.Movement):
                        //Dont send anything, it sends roomlost on retreaval off action points of the player
                        break;
                    case (GameManager.TurnPhases.Interaction):
                        //Send choice information
                        //Handled in player movement to send once they arrive at the room.
                        break;
                    case (GameManager.TurnPhases.BasicSurge):
                        PlayerCardManager.instance.UpdateAllCards();
                        GameManager.instance.IncrementTurn();
                        break;
                    case (GameManager.TurnPhases.AttackSurge):
                        //Need to display surge information on main screen
                        PlayerCardManager.instance.UpdateAllCards();
                        SendAIAttack(GameManager.instance.targetPlayer);
                        break;
                    default:
                        throw new NotImplementedException("Not a valid phase");
                }

                //Pings the client if they have been selected as a traitor


                break;
        }
       

    }

    private void GetEquipItem(int conID, int chanID, int rHostID, EquipItem equipItem)
    {
        int itemID = equipItem.ItemID;

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);

            if (player.playerID == conID)
            {

                Item selectedItem = player.items[itemID];

                if (selectedItem.isEquipped)
                {
                    player.UnequipItem(itemID);
                    PlayerCardManager.instance.UpdateAllCards();

                    SyncPlayerData(conID);
                    SendUnequipSuccess(conID);
                }
                else
                {
                    Player.EquipErrors equipStatus = player.EquipItem(itemID);
                    PlayerCardManager.instance.UpdateAllCards();

                    SyncPlayerData(conID);
                    SendEquipState(conID, equipStatus);
                }

            }
        }


    }

    private void GetDiscardItem(int conID, int chanID, int rHostID, DiscardItem discardItem)
    {
        int itemID = discardItem.ItemID;

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);

            if (player.playerID == conID)
            {
                player.DiscardItem(itemID);
                PlayerCardManager.instance.UpdateAllCards();

                SyncPlayerData(conID);
                SendDiscardSuccess(conID);
            }
        }


    }

    private void GetStealItem(int conID, int chanID, int rHostID, StealItem stealItem)
    {
        int itemID = stealItem.ItemID;
        int loserID = stealItem.LoserID;

        for (int i = 1; i < GameManager.instance.numPlayers + 1; i++)
        {
            Player player = GameManager.instance.GetPlayer(i);

            if (player.playerID == conID)
            {
                Player winningPlayer = GameManager.instance.GetPlayer(conID);
                Player losingPlayer = GameManager.instance.GetPlayer(loserID);

                bool successfulSteal = false;

                if (stealItem.StealComponent)
                {
                    if (!winningPlayer.hasComponent)
                    {
                        winningPlayer.hasComponent = true;
                        losingPlayer.hasComponent = false;
                        SyncPlayerData(loserID);
                        SendComponentStolen(loserID);

                        successfulSteal = true;

                        SyncPlayerData(conID);

                        PlayerCardManager.instance.UpdateAllCards();
                    }

                    SendComponentStealSuccess(conID, successfulSteal);
                }
                else
                {
                    Item selectedItem = losingPlayer.items[itemID];

                    if (winningPlayer.GiveItem(selectedItem))
                    {
                        losingPlayer.RemoveItem(itemID);
                        successfulSteal = true;
                        SyncPlayerData(loserID);
                        SendItemStolen(loserID, selectedItem.ItemName);

                        PlayerCardManager.instance.UpdateAllCards();

                        SyncPlayerData(conID);
                    }


                    SendStealSuccess(conID, successfulSteal);
                }

            }
        }

    }

    private void GetStealDiscardItem(int conID, int chanID, int rHostID, StealDiscardItem stealDiscardItem)
    {
        int itemID = stealDiscardItem.ItemID;
        int loserID = stealDiscardItem.LoserID;

        Player losingPlayer = GameManager.instance.GetPlayer(loserID);

        losingPlayer.DiscardItem(itemID);
        PlayerCardManager.instance.UpdateAllCards();

        SyncPlayerData(loserID);
        SendItemStolen(loserID, losingPlayer.items[itemID].ItemName);
        SendStealDiscardSuccess(conID);
    }

    private void GetAISpecSelection(int conID, int chanID, int rHostID, AISpecSelection aISpecSelection)
    {
        GameManager.SpecScores selectedSpec = (GameManager.SpecScores)aISpecSelection.SelectedSpec;

        bool attackOutcome = GameManager.instance.AIAttackPlayer(selectedSpec);

        GameObject canvas = GameObject.Find("Canvas");
        canvas.GetComponent<MainGameUIManager>().attackSurgePanel.GetComponent<AttackSurgeManager>().SetWinText(attackOutcome);
        canvas.GetComponent<MainGameUIManager>().attackSurgePanel.GetComponent<AttackSurgeManager>().confirmButton.GetComponent<Button>().interactable = true;

        SendAIAttackResult(conID, attackOutcome);
        SendIsTraitor();
    }


    private void EndSurge(int conID, int chanID, int rHostID, NextTurn turn) {

        GameManager.instance.EndRound();

    }

    #endregion
}