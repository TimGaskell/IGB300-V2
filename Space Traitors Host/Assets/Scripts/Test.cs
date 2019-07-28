using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Test : NetworkBehaviour {

    public Text testText;
    public GameObject[] Players;

    // Start is called before the first frame update
    void Start()
    {

       

    }

    // Update is called once per frame
    void Update()
    {

        Players = GameObject.FindGameObjectsWithTag("Gamer");
        Players[1].GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);

    }

    [Command]
    public void CmdTakeDamage() {


       
        Debug.Log("HELP ME");


    }

    [ClientRpc]
    public void RpcSendInfo(int amount) {

        testText.text = amount.ToString();
        Debug.Log("I want to Die");

    }

    //Doesnt do anything from the client
    //If done from the server it does both.

    public void TakeDamage() {

        CmdTakeDamage();

    }

    
}
