using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkStuff : NetworkBehaviour {


    private NetworkManager networkManager;
    public int Test = 0;

    // Start is called before the first frame update
    void Start() {


        networkManager = NetworkManager.singleton;
       
        

    }

    // Update is called once per frame
    void Update() {

       
        if (Input.GetKey("k")) {

            CmdTakeDamage();

           
            
        }
       
      
            

      
    }

    [Command]
    public void CmdTakeDamage() {

        TargetSendStuff(connectionToClient);

    }


    [TargetRpc]
    public void TargetSendStuff(NetworkConnection connection) {

        
        Debug.Log("Send Target Message");

        

    }

    

    

}
