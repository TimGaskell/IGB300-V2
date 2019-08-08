using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class DiscoveryGrabber : MonoBehaviour
{
    // Start is called before the first frame update
    NetworkDiscovery nd;
    NetworkManager nm;
    GameObject button;
    NetworkClient nc;
    void Start()
    {
        nd = gameObject.GetComponent<NetworkDiscovery>();
        nm = gameObject.GetComponent<NetworkManager>();
        button = GameObject.Find("JoinButton");
        nc = new NetworkClient();
    }

    // Update is called once per frame
    void Update()
    {
        FindGames();
    }

    void FindGames()
    {
        foreach (var key in nd.broadcastsReceived.Keys)
        {
            NetworkBroadcastResult value;
            nd.broadcastsReceived.TryGetValue(key, out value);
            string stringValue = System.Text.Encoding.Unicode.GetString(value.broadcastData, 0, value.broadcastData.Length);
            button.GetComponentInChildren<Text>().text = stringValue;
            JoinGame(key);
            

        }
    }

    void JoinGame(string address)
    {
        nc.Connect(address, 7777);
    }

}
