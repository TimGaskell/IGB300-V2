using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using TMPro;

public class JoinGame : MonoBehaviour {
    List<GameObject> RoomList = new List<GameObject>();

    [SerializeField]
    private TextMeshProUGUI status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    public NetworkManager networkManager;

    private CustomNetworkDiscovery cnd;

    void Start() {

        networkManager = NetworkManager.singleton;
        if (SceneManager.GetActiveScene().name == "LobbyLan Client"){
            cnd = GameObject.Find("NetworkManager").GetComponent<CustomNetworkDiscovery>();
            cnd.Initialize();
            cnd.StartAsClient();
            status.text = "No rooms at the moment. Click refresh to search for one";
        }
        else{

            if (networkManager.matchMaker == null) {
                networkManager.StartMatchMaker();
             }
        }

        RefreshRoomList();     
    }

    public void RefreshRoomList() {

        ClearRoomList();
        if (SceneManager.GetActiveScene().name == "LobbyLan Client")
        {
            if (cnd.broadcastsReceived.Values != null)
            {
                foreach (var value in cnd.broadcastsReceived.Values)
                {
                    string ip = value.serverAddress.Substring(7);
                    string name = System.Text.Encoding.Unicode.GetString(value.broadcastData);
                    name = name.Substring(name.IndexOf(":") + 1, name.IndexOf(":7") - name.IndexOf(":") - 1);

                    GameObject RoomListItemGO = Instantiate(roomListItemPrefab);
                    RoomListItemGO.transform.SetParent(roomListParent, false);

                    RoomListItemGO.GetComponent<GameButton>().Setup(ip, name);

                    RoomList.Add(RoomListItemGO);
                    status.text = "";
                }
            }
            else
            {
                status.text = "No rooms at the moment";
            }
            
        }
        else
        {
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);

        }
        

    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList) {
        status.text = "";

        if (!success || matchList == null) {
            status.text = "Couldn't get room list.";
            return;
        }

        ClearRoomList();

        foreach (MatchInfoSnapshot match in matchList) {

            GameObject RoomListItemGO = Instantiate(roomListItemPrefab);
            RoomListItemGO.transform.SetParent(roomListParent, false);

            RoomListItem roomListItem = RoomListItemGO.GetComponent<RoomListItem>();
            if (roomListItem != null) {
                roomListItem.setup(match, JoinRoom);
            }

            RoomList.Add(RoomListItemGO);


        }

        if (RoomList.Count == 0) {

            status.text = "No rooms at the moment";

        }
    }

    void ClearRoomList() {

        for (int i = 0; i < RoomList.Count; i++) {
            Debug.Log(RoomList[i].transform.name);
            Destroy(RoomList[i]);
        }
        RoomList.Clear();

    }

    public void JoinRoom(MatchInfoSnapshot match) {

        Debug.Log("Joining " + match.name);
        Server.Instance.match = match;
        Server.Instance.networkManager = networkManager;
        Server.Instance.DC = true;

        networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";

    }

    
}
