using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 6;

    private string roomName;

    private NetworkManager networkManager;

    public Scene NextScene;

    public GameObject roomNameInput;
    public GameObject errorText;

    void Start() {
        networkManager = NetworkManager.singleton;
        errorText.SetActive(false);
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

        roomName = roomNameInput.GetComponent<TMP_InputField>().text;

        if(roomName != "" && roomName != null) {
            Debug.Log("Creating Room: " + roomName + " with room for " + roomSize);
            //create room
            if (SceneManager.GetActiveScene().name == "LobbyLan Host")
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
        else
        {   
            errorText.SetActive(true);
        }

    }



}
