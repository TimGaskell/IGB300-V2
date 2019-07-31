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
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
           
        }
        //Debug.Log("Recieved a message of type " + msg.OperationCode);

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

    //This needs to be updated

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

    //This needs to be updated
    private void LobbyConnectOrDisconnect(GameObject player, bool connect, int conID, bool imageEnable)
    {
        player.GetComponent<PlayerConnect>().connected = connect;
        player.GetComponent<PlayerConnect>().playerID = conID;
        player.GetComponent<PlayerConnect>().playerImage.enabled = imageEnable;
    }

    //This needs to be updated
    private void GameConnectOrDisconnect(GameObject player, bool connect, int conID)
    {
       // player.GetComponent<Player>().connected = connect;
       // player.GetComponent<Player>().playerID = conID;
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

  

}
