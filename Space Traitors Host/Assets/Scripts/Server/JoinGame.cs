using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour {
    List<GameObject> RoomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    void Start() {

        networkManager = NetworkManager.singleton;

        if (networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList() {

        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading...";

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
            Destroy(RoomList[i]);
        }
        RoomList.Clear();

    }

    public void JoinRoom(MatchInfoSnapshot match) {

        Debug.Log("Joining " + match.name);
        networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";

    }

    
}
