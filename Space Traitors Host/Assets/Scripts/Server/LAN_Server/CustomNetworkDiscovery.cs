using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomNetworkDiscovery : NetworkDiscovery
{
    // Start is called before the first frame update



    public void Start()
    {

        
    }

    public void StartHost()
    {
        
        StopBroadcast();
        Initialize();
        
        StartAsServer();
    }

    // Update is called once per frame


    public void StartClient()
    {

        StopBroadcast();
        Initialize();

        StartAsClient();
        //RefreshAvailableGames();
    }

    //public void RefreshAvailableGames()
    //{
    //    uiManager.GetComponent<UIManager>().RemoveLanGames();

    //    foreach (var value in broadcastsReceived.Values)
    //    {
    //        string ip = value.serverAddress.Substring(7);
    //        string name = System.Text.Encoding.Unicode.GetString(value.broadcastData);
    //        name = name.Substring(name.IndexOf(":") + 1, name.IndexOf(":7") - name.IndexOf(":") - 1);

    //        Debug.Log(value.serverAddress);
    //        Debug.Log(System.Text.Encoding.Unicode.GetString(value.broadcastData));
    //        uiManager.GetComponent<UIManager>().AddLanGameButton(ip, name);
    //    }


    //}


}
