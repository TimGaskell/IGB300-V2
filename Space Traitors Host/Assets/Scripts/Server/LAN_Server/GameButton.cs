﻿using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameButton: MonoBehaviour
{
    private string gameIP = "";
    private string gameInfo = "";

    private GameObject textIP;
    private GameObject textInfo;
    private NetworkManager nm;
    
 

    public void Setup(string IP, string info)
    {
        textIP = gameObject.transform.Find("GameIP").gameObject;
        textInfo = gameObject.transform.Find("GameName").gameObject;
        nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        gameIP = IP;
        gameInfo = info;
        textInfo.GetComponent<Text>().text = gameInfo;
        textIP.GetComponent<Text>().text = gameIP;
    }

    public void ConnectToGame()
    {
        NetworkManager.singleton.StopAllCoroutines();
        string ipAddress = gameIP;
        NetworkManager.singleton.networkAddress = ipAddress;
        NetworkManager.singleton.networkPort = 7777;
        NetworkManager.singleton.StartClient();
    }

    // Update is called once per frame
    
}