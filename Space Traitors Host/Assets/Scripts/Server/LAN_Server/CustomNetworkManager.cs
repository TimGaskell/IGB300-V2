using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private string ipAddress;

    public void HostGame()
    {
        
        NetworkManager.singleton.StopAllCoroutines();
        SetPort();
        NetworkManager.singleton.StartHost();
        NetworkManager.singleton.networkAddress = GameObject.Find("GameNameText").GetComponent<Text>().text.ToString();
    }
    public void JoinGame(string ip)
    {
        NetworkManager.singleton.StopAllCoroutines();
        SetIPAddress(ip);
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    public void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    public void SetIPAddress(string ip)
    {
        string ipAddress = ip;
        NetworkManager.singleton.networkAddress = ipAddress;
    }
}
