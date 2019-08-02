﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private byte reliableChannel;
    private int hostID;
    private int webHostID;

    private const int maxUser = 100;
    private const int port = 26000;
    private const int webPort = 26001;
    private const int byteSize = 1024;

    private bool isStarted = false;
    private byte error;
    private string serverIP = IPManager.GetIP(ADDRESSFAM.IPv4);

    //Other
    public AudioSource connectSound;
    public List<GameObject> players = new List<GameObject>();
    private List<GameObject> ElminiatedPlayers = new List<GameObject>();
    private List<GameObject> playersRemoved = new List<GameObject>();
    public GameObject[] ScrapTotals;
    public GameObject[] Components;
    public GameObject AiPowerSliderUI;
    public GameObject playerStorage;
    private GameObject setter;
    private Scene currentScene;
    public Text connectText;
    public Sprite[] portraits;

    public int[] playerIDs = new int[6];
    public int playersJoined;
    private int portraitID = -1;
    public int tempPlayerID;
    private string sceneName;
    public int InstalledComponents = 0;
    private bool SentMessage = false;

    public enum WinLossConditions {

        InnocentsWin ,
        Eliminated,
        TraitorsWin
    }
  


    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Initialise();
       
    }

    public void Initialise()
    {
        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        reliableChannel = config.AddChannel(QosType.Reliable);


        HostTopology topo = new HostTopology(config, maxUser);

        //Server only code
        hostID = NetworkTransport.AddHost(topo, port, null);
        webHostID = NetworkTransport.AddWebsocketHost(topo, port, null);

        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", port, webPort));
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

        if (sceneName == "LobbyTest")
        {
            connectText.text = serverIP;
        }

        if (sceneName == "Character Select")
        {
            SetPortraits();
        }
        if(sceneName == "server") {

            SetScrapText();
            SetComponentsText();
            SetAiPowerSlider();
            VictoryConditions();

        }

        //Networking messages
        UpdateMessagePump();
    }



    private void UpdateMessagePump()
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
                foreach (GameObject player in playerArray())
                {
                    if (!player.GetComponent<PlayerConnect>().connected)
                    {
                        if (sceneName == "LobbyTest")
                            LobbyConnectOrDisconnect(player, true, connectionID, true);
                        else if (sceneName == "server")
                            GameConnectOrDisconnect(player, true, connectionID);

                        Debug.Log(player.name + " has connected through host " + recHostID);
                        break;
                    }
                }
                break;

            //When user disconnects from game
            case NetworkEventType.DisconnectEvent:
                //Loop through to find player that is disconnecting, based on their ID
                foreach (GameObject player in playerArray())
                {
                    if (player.GetComponent<PlayerConnect>().playerID == connectionID)
                    {
                        if (sceneName == "LobbyTest")
                            //Reset player variables
                            LobbyConnectOrDisconnect(player, false, 0, false);
                        else if (sceneName == "server")
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

    private void OnData(int conID, int chanID, int rHostID, NetMessage msg)
    {
        switch (msg.OperationCode)
        {
            //////////////////Temporary placement of new netmessages, will move to proper server script later/////////////////

            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.CharacterInformation:
                //SendCharacterInfo(conID, chanID, rHostID, (CharacterInformation)msg);
                break;
            case NetOP.AbilityInformation:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.AvailableRooms:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.RoomChoices:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.SpecChallenge:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.PlayerInformation:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.TraitorSelction:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.SurgeInformation:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.AiAttacks:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.CombatResolution:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.CombatAvailablity:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.CombatBeingAttacked:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.PlayerElimination:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.NonTraitorVictory:
                Debug.Log("Unexpected NETOP");
                break;


                /*
                case NetOP.None:
                    Debug.Log("Unexpected NETOP");
                    break;
                case NetOP.ChangeRoom:
                    ChangeRoom(conID, chanID, rHostID, (Net_ChangeRoom)msg);
                    break;
                case NetOP.SendPoints:
                    SendPoints(conID, chanID, rHostID, (Net_SendPoints)msg);
                    break;
                case NetOP.SendTurnEnd:
                    SendTurnEnd(conID, chanID, rHostID, (Net_SendTurnEnd)msg);
                    break;
                case NetOP.SendScrap:
                    AssignScrap(conID, chanID, rHostID, (Net_SendScrap)msg);
                    break;
                case NetOP.SendComponents:
                    AssignComponents(conID, chanID, rHostID, (Net_SendComponents)msg);
                    break;
                case NetOP.SendAIPower:
                    AssignAiPower(conID, chanID, rHostID, (Net_SendAiPower)msg);
                    break;
                case NetOP.RoomNumber:
                    SendRoomCost(conID, chanID, rHostID, (Net_SendRoomNumber)msg);
                    break;
                case NetOP.AssignTraitor:
                    AssignTraitor(conID, chanID, rHostID, (Net_AssignTraitor)msg);
                    break;
                    */
        }
        //Debug.Log("Recieved a message of type " + msg.OperationCode);

    }

    //////////////////Receiving methods- methods sent from client/////////////////
    private void RecieveAbilityInfo(int conID, int chanID, int rHostID, AbilityInformation ai)
    {
        SendClient(ai);
    }

    private void ReceiveAvailableRooms(int conID, int chanID, int rHostID, AvailableRooms ar)
    {
        SendClient(ar);
    }

    private void RecieveRoomChoices(int conID, int chanID, int rHostID, RoomChoices rc)
    {
        SendClient(rc);
    }

    private void RecieveSpecChallenge(int conID, int chanID, int rHostID, SpecChallenge sc)
    {
        SendClient(sc);
    }

    private void RecievePlayerInfo(int conID, int chanID, int rHostID, PlayerInformation pi)
    {
        SendClient(pi);
    }

    private void RecieveTraitor(int conID, int chanID, int rHostID, TraitorSelection ts)
    {
        SendClient(ts);
    }

    private void RecieveSurgeInfo(int conID, int chanID, int rHostID, SurgeInformation si)
    {
        SendClient(si);
    }

    private void RecieveAiAttack(int conID, int chanID, int rHostID, AiAttacks aa)
    {
        SendClient(aa);
    }

    private void RecieveResolution(int conID, int chanID, int rHostID, CombatResolution cr)
    {
        SendClient(cr);
    }

    private void RecieveAvailability(int conID, int chanID, int rHostID, CombatAvailability ca)
    {
        SendClient(ca);
    }

    private void RecieveBeingAttacked(int conID, int chanID, int rHostID, CombatBeingAttacked ba)
    {
        SendClient(ba);
    }

    private void RecievePlayerElimination(int conID, int chanID, int rHostID, PlayerElimination pe)
    {
        SendClient(pe);
    }

    private void RecieveVictory(int conID, int chanID, int rHostID, InnocentVictory nv)
    {
        SendClient(nv);
    }


    private void AssignScrap(int conID, int chanID, int rHostID, Net_SendScrap scrap) {

        foreach (GameObject player in playerArray()) {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID) {

               ScrapTotals[conID - 1].GetComponent<Text>().text = scrap.ScrapTotal.ToString();

            }
        }
    }

    private void AssignTraitor(int conID, int chanID, int rHostID, Net_AssignTraitor scrap)
    {
    }

    private void AssignAiPower(int conID, int chanID, int rHostID, Net_SendAiPower aiPower) {

        AiPowerSliderUI.GetComponent<AiPower>().power += aiPower.AIpowerAmountGained;
       
    }

    private void AssignComponents(int conID, int chanID, int rHostID, Net_SendComponents components) {

        foreach (GameObject player in playerArray()) {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID) {

                Components[conID - 1].GetComponent<Text>().text = components.ComponentNumber.ToString();

                if (components.Installed == true) {

                    InstalledComponents += 1;

                }
            }
            
        }
    }

 
    private void ChangeRoom(int conID, int chanID, int rHostID, Net_ChangeRoom ca)
    {
        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {
                //player.GetComponent<Player>().goalIndex = ca.Location;
                //player.GetComponent<Player>().Begin = true;
                //player.GetComponent<Player>().startMoving = true;

              
                Debug.Log(player.name + " is in " + ca.Location);
                break;
            }
        }
    }

    private void SendPoints(int conID, int chanID, int rHostID, Net_SendPoints lr)
    {
        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<PlayerConnect>().playerID == conID)
            {
                if ((sceneName == "Character Select"))
                {
                    switch (lr.Influence)
                    {
                        case "Brute":
                            player.GetComponent<PlayerConnect>().characterName = "Brute";
                            break;

                        case "Butler":
                            player.GetComponent<PlayerConnect>().characterName = "Butler";
                            break;

                        case "Singer":
                            player.GetComponent<PlayerConnect>().characterName = "Singer";
                            break;

                        case "Techie":
                            player.GetComponent<PlayerConnect>().characterName = "Techie";
                            break;

                        case "Engineer":
                            player.GetComponent<PlayerConnect>().characterName = "Engineer";
                            break;

                        case "Chef":
                            player.GetComponent<PlayerConnect>().characterName = "Chef";
                            break;
                    }

                }
                break;
            }
        }
    }

    public void SendTurnEnd(int conID, int chanID, int rHostID, Net_SendTurnEnd te)
    {
        foreach (GameObject player in playerArray())
        {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID)
            {
                playerStorage.GetComponent<RoundManager>().IncrementTurn();
            }
        }
    }

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

    public void roomLocation(Net_ChangeRoom ca)
    {




    }

    private List<GameObject> playerArray()
    {

        if ((sceneName == "LobbyTest") || (sceneName == "Character Select"))
            return players;
        else if (sceneName == "server")
            return playerStorage.GetComponent<RoundManager>().playersInGame;
        else
        {
            Debug.Log("Could not get the correct scene");
            return null;
        }

    }

    private void LobbyConnectOrDisconnect(GameObject player, bool connect, int conID, bool imageEnable)
    {
        player.GetComponent<PlayerConnect>().connected = connect;
        player.GetComponent<PlayerConnect>().playerID = conID;
        player.GetComponent<PlayerConnect>().playerImage.enabled = imageEnable;
    }

    private void GameConnectOrDisconnect(GameObject player, bool connect, int conID)
    {
       // player.GetComponent<Player>().connected = connect;
       // player.GetComponent<Player>().playerID = conID;
    }

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

    public void StartGame() //This is called when a game is started in lobby
    {
        //TODO: cannot start game unless at least 3 (1 for purposes of testing) players are connected

        //If a player hasn't been assigned to one of the player objects, remove it from the server's array of players
        for (int k = 0; k < players.Count; k++)
        {
            if (!players[k].GetComponent<PlayerConnect>().connected)
            {
                playersRemoved.Add(players[k]);
            }
        }

        if (playersRemoved != null)
        {
            foreach (GameObject player in playersRemoved)
            {
                players.Remove(player);
            }
        }


        //Get the number of players based on how many remain
        playersJoined = players.Count;
        int i = 0;
        foreach (GameObject player in players)
        {
            playerIDs[i] = player.GetComponent<PlayerConnect>().playerID;
            player.GetComponent<PlayerConnect>().transform.parent = null;
            DontDestroyOnLoad(player);
            i++;
        }

        //Send message to every player's client to move onto next scene
        ClientNextScene();

        //Change to the character select
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    //////////////////Sending methods- sending to client/////////////////
    private void SendCharacterInfo(int brawn, int skill, int tech, int charm)
    {
        CharacterInformation ci = new CharacterInformation();

        ci.Basebrawn = brawn;
        ci.Baseskill = skill;
        ci.Basetech = tech;
        ci.Basecharm = charm;

        SendClient(ci);
    }

    private void SendAbilityInfo(string description)
    {
        AbilityInformation ai = new AbilityInformation();

        ai.AbilityDescription = description;

        SendClient(ai);
    }

    private void SendAvailableRooms(int conID, int chanID, int rHostID, AvailableRooms ar)
    {
        SendClient(ar);
    }

    private void SendRoomChoices(int conID, int chanID, int rHostID, RoomChoices rc)
    {
        SendClient(rc);
    }

    private void SendSpecChallenge(int conID, int chanID, int rHostID, SpecChallenge sc)
    {
        SendClient(sc);
    }

    private void SendPlayerInfo(int conID, int chanID, int rHostID, PlayerInformation pi)
    {
        SendClient(pi);
    }

    private void SendTraitor(int conID, int chanID, int rHostID, TraitorSelection ts)
    {
        SendClient(ts);
    }

    private void SurgeInfo(int conID, int chanID, int rHostID, SurgeInformation si)
    {
        SendClient(si);
    }

    private void SendAiAttack(int conID, int chanID, int rHostID, AiAttacks aa)
    {
        SendClient(aa);
    }

    private void SendResolution(int conID, int chanID, int rHostID, CombatResolution cr)
    {
        SendClient(cr);
    }

    private void SendAvailability(int conID, int chanID, int rHostID, CombatAvailability ca)
    {
        SendClient(ca);
    }

    private void SendBeingAttacked(int conID, int chanID, int rHostID, CombatBeingAttacked ba)
    {
        SendClient(ba);
    }

    private void SendPlayerElimination(int conID, int chanID, int rHostID, PlayerElimination pe)
    {
        SendClient(pe);
    }

    private void SendVictory(int conID, int chanID, int rHostID, InnocentVictory nv)
    {
        SendClient(nv);
    }

    public void ClientNextScene()
    {
        foreach (GameObject player in players)
        {
            tempPlayerID = player.GetComponent<PlayerConnect>().playerID;
            SendLocation(0);
        }
    }

    public void ChooseTraitor()
    {
        int randomIndex = Random.Range(0, players.Count + 1);
        tempPlayerID = players[randomIndex].GetComponent<PlayerConnect>().playerID;
        SendTraitor(0);
    }

    public void ClientTurnChange(int playerID, bool playerturn)
    {
        tempPlayerID = playerID;
        SendTurnEnd(playerturn);
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

    //Sends the location to the server, references the get,set from Net_Change Room
    public void SendLocation(int location)
    {
        Net_ChangeRoom ca = new Net_ChangeRoom();

        ca.Location = location;

        SendClient(ca);

    }

    public void SendTraitor(int blank)
    {
        Net_AssignTraitor ca = new Net_AssignTraitor();

        SendClient(ca);

    }

    //Sends the location to the server, references the get,set from Net_Change Room
    public void SendTurnEnd(bool playerturn)
    {
        Net_SendTurnEnd ca = new Net_SendTurnEnd();

        ca.Ended = playerturn;

        SendClient(ca);

    }

    public void SendAllowMovement(int player, bool yes_no) {

        NetAllowMovement ca = new NetAllowMovement();

        tempPlayerID = player;

        ca.AllowToMove = yes_no;

        SendClient(ca);

    }




    private void SendRoomCost(int conID, int chanID, int rHostID, Net_SendRoomNumber roomNumber) {

        foreach (GameObject player in playerArray()) {
            //Find the correct player
            if (player.GetComponent<Player>().playerID == conID) {

                Net_SendCostOfRoom costOfRoom = new Net_SendCostOfRoom();
                //player.GetComponent<Player>().goalIndex = roomNumber.Room;
                //player.GetComponent<Player>().startMoving = false;
                //player.GetComponent<Player>().Begin = true;


                //I know this method is disgusting but ill fix it later.
                //player.GetComponent<Player>().currentPath = player.GetComponent<Player>().AStarSearch(player.GetComponent<Player>().currentPath[player.GetComponent<Player>().currentPathIndex], roomNumber.Room);


                //costOfRoom.RoomCost = player.GetComponent<Player>().currentPath.Count -1 ;
                Debug.Log("EnergyCost: " + costOfRoom.RoomCost);
                SendClient(costOfRoom);
            }
        }
    }

    public void SetScrapText() {

        ScrapTotals = GameObject.FindGameObjectsWithTag("Scrap Text");

    }

    public void SetComponentsText() {

        Components = GameObject.FindGameObjectsWithTag("Components Text");

    }

    public void SetAiPowerSlider() {

        AiPowerSliderUI = GameObject.FindGameObjectWithTag("Ai Power");

    }

    private void VictoryConditions() {


        if (SentMessage == false) {

            tempPlayerID = 1;

            if (InstalledComponents == 5) {

                foreach(GameObject player in playerArray()) {


                    Net_SendWinLoss VictoryMet = new Net_SendWinLoss();
                    VictoryMet.WinOrLossCondition = (int)WinLossConditions.InnocentsWin;
                    SendClient(VictoryMet);
                    tempPlayerID++;
                 

                }
                SentMessage = true;


            }
            else if (players.Count == 0) {



            }

        }
    }

}
