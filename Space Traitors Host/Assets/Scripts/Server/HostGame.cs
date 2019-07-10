using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 6;

    private string roomName;

    private NetworkManager networkManager;


    void Start() {

        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }
    }

    public void setRoomName(string name) {

        roomName = name;

    }

    public void CreateRoom() {

        Debug.Log("help");
        if(roomName != "" && roomName != null) {
            Debug.Log("Creating Room: " + roomName + "with room for " + roomSize);
            //create room
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "","",0,0,networkManager.OnMatchCreate);
        }

    }



}
