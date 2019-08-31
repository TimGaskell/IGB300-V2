using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 6;

    private string roomName;

    private NetworkManager networkManager;

    public Scene NextScene;


    void Start() {
        networkManager = NetworkManager.singleton;
        if (SceneManager.GetActiveScene().name == "LobbyLan")
        {
            return;
        }
        else
        {
            
            if(networkManager.matchMaker == null) {
                networkManager.StartMatchMaker();
            }
              
        }

      
    }

    public void setRoomName(string name) {

        roomName = name;

    }

    public void CreateRoom() {

        if(roomName != "" && roomName != null) {
            Debug.Log("Creating Room: " + roomName + " with room for " + roomSize);
            //create room
            if (SceneManager.GetActiveScene().name == "LobbyLan")
            {
                networkManager.StopAllCoroutines();
                networkManager.networkPort = 7777;
               // networkManager.StartHost();
                networkManager.networkAddress = roomName;
                Server.Instance.HostInitialise();
                SceneManager.LoadScene("ServerLobby");
                GameManager.instance.ResetPlayers();
            }
            else
            {
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "","",0,0,networkManager.OnMatchCreate);

            }
        }

    }



}
