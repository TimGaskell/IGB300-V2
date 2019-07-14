using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkStuff : NetworkBehaviour {


    private NetworkManager networkManager;


    // Start is called before the first frame update
    void Start() {


        networkManager = NetworkManager.singleton;

        

    }

    // Update is called once per frame
    void Update() {

        // If we're not the server, we don't have any business in sending data to players.
        if (!isServer) {
           
            return;
            
        }
        if (Input.GetKey("k")) {
           
                TargetSendStuff(NetworkServer.connections[0]);

           
            
        }
       
      
            

      
    }


    [TargetRpc]
    public void TargetSendStuff(NetworkConnection connection) {

        Debug.Log("Send Target Message");

        

    }

    

    

}
