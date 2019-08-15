using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour{

    /* Script that disables functionalites on other objects other than the local player.
     * This stops it from accessing scripts that don't belong to it        
    */

    [SerializeField]
    Behaviour[] componentsToDisable;

    void Start() {

        if (!isLocalPlayer) {
            for(int i = 0; i < componentsToDisable.Length; i++) {

                componentsToDisable[i].enabled = false;
            }
        }     
    }
}
